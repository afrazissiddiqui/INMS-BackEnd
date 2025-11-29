using ClosedXML.Excel;
using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement6th.Service
{
    public class ExcelUploadService
    {
        private readonly AppDbContext _db;

        public ExcelUploadService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse<string>> ProcessExcelAsync(Stream stream)
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
                return ApiResponse<string>.Fail(400, "Excel file is empty or invalid.");

            var rows = worksheet.RangeUsed()?.RowsUsed().Skip(1); // skip header
            if (rows == null || !rows.Any())
                return ApiResponse<string>.Fail(400, "No data found in Excel.");

            foreach (var row in rows)
            {
                try
                {
                    var itemName = row.Cell(1).GetString().Trim();
                    var categoryName = row.Cell(2).GetString().Trim();
                    var uomName = row.Cell(3).GetString().Trim();
                    var vatCode = row.Cell(4).GetString().Trim();
                    var vatDescription = row.Cell(5).GetString().Trim();
                    var vatRate = row.Cell(6).GetValue<decimal>();

                    // ✅ Category
                    var category = await _db.Categories
                        .FirstOrDefaultAsync(c => c.Name == categoryName);
                    if (category == null && !string.IsNullOrWhiteSpace(categoryName))
                    {
                        category = new Category { Name = categoryName };
                        _db.Categories.Add(category);
                        await _db.SaveChangesAsync();
                    }

                    // ✅ UOM
                    var uom = await _db.UnitsOfMeasure
                        .FirstOrDefaultAsync(u => u.Abbreviation == uomName);
                    if (uom == null && !string.IsNullOrWhiteSpace(uomName))
                    {
                        uom = new UnitOfMeasure { Abbreviation = uomName };
                        _db.UnitsOfMeasure.Add(uom);
                        await _db.SaveChangesAsync();
                    }

                    // ✅ VAT (created if missing, but not linked to Item)
                    var vat = await _db.VATs
                        .FirstOrDefaultAsync(v => v.Code == vatCode && v.Rate == vatRate);
                    if (vat == null && !string.IsNullOrWhiteSpace(vatCode))
                    {
                        vat = new VAT
                        {
                            Code = vatCode,
                            Description = string.IsNullOrWhiteSpace(vatDescription) ? null : vatDescription,
                            Rate = vatRate
                        };
                        _db.VATs.Add(vat);
                        await _db.SaveChangesAsync();
                    }

                    // ✅ Item
                    if (!string.IsNullOrWhiteSpace(itemName) && category != null && uom != null)
                    {
                        var existingItem = await _db.Items.FirstOrDefaultAsync(i =>
                            i.ItemName == itemName &&
                            i.CategoryId == category.Id &&
                            i.UnitOfMeasureId == uom.Id);

                        if (existingItem == null)
                        {
                            var item = new Item
                            {
                                ItemName = itemName,
                                CategoryId = category.Id,
                                UnitOfMeasureId = uom.Id,
                                AllowNegativeInventory = false
                            };

                            _db.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ApiResponse<string>.Fail(500, $"Row import failed: {ex.Message}");
                }
            }

            await _db.SaveChangesAsync();
            return ApiResponse<string>.Success("Bulk upload successful!");
        }
    }
}
