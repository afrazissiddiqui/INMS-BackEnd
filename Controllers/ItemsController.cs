using Inventory.Api.Common.DTOs;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using InventoryManagement6th.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ItemsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemResponseDto>>> GetItems(
            int pageNumber = 1, int pageSize = 25)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("PageNumber and PageSize must be greater than 0.");

            var query = _db.Items
      .Where(i => i.IsActive && !i.IsDeleted)
      .Include(i => i.Category)
      .Include(i => i.UnitOfMeasure)
      .AsNoTracking();

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var itemIds = items.Select(i => i.Id).ToList();

            var stockDict = await _db.StockTransactions
                .Where(st => itemIds.Contains(st.ItemId))
                .GroupBy(st => st.ItemId)
                .Select(g => new { ItemId = g.Key, Total = g.Sum(x => x.Qty) })
                .ToDictionaryAsync(x => x.ItemId, x => x.Total);

            var result = items.Select(item => new ItemResponseDto
            {
                Id = item.Id,
                ItemName = item.ItemName,
                CategoryName = item.Category?.Name ?? "",
                UnitAbbreviation = item.UnitOfMeasure?.Abbreviation ?? "",
                StockQuantity = stockDict.ContainsKey(item.Id) ? stockDict[item.Id] : 0,
                BuyPrice = item.BuyPrice,
                SalePrice = item.SalePrice,
                AllowNegativeInventory = item.AllowNegativeInventory
            }).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = result
            });
        }

        // GET: api/items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResponseDto>> GetItemById(int id)
        {
            var item = await _db.Items
                .Where(i => i.IsActive && !i.IsDeleted)
                .Include(i => i.Category)
                .Include(i => i.UnitOfMeasure)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (item == null)
                return NotFound($"Item with ID {id} not found.");

            var stock = await _db.StockTransactions
                .Where(st => st.ItemId == item.Id)
                .SumAsync(st => st.Qty);

            var dto = new ItemResponseDto
            {
                Id = item.Id,
                ItemName = item.ItemName,
                CategoryName = item.Category?.Name ?? "",
                UnitAbbreviation = item.UnitOfMeasure?.Abbreviation ?? "",
                StockQuantity = stock,
                BuyPrice = item.BuyPrice,
                SalePrice = item.SalePrice,
                AllowNegativeInventory = item.AllowNegativeInventory
            };

            return Ok(dto);
        }

        // POST: api/items
        [HttpPost]
        public async Task<ActionResult<ItemResponseDto>> CreateItem(ItemRequestDto request)
        {
            var uom = await _db.UnitsOfMeasure
                .FirstOrDefaultAsync(u => u.Abbreviation == request.UnitAbbreviation);

            if (uom == null)
                return NotFound($"Unit of Measure '{request.UnitAbbreviation}' not found.");

            if (request.StockQuantity < 0 && !request.AllowNegativeInventory)
                return BadRequest("Negative stock is not allowed unless 'AllowNegativeInventory' is true.");

            var item = new Item
            {
                ItemName = request.ItemName,
                CategoryId = request.CategoryId,
                UnitOfMeasureId = uom.Id,
                BuyPrice = request.BuyPrice,
                SalePrice = request.SalePrice,
                AllowNegativeInventory = request.AllowNegativeInventory
            };

            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            if (request.StockQuantity != 0)
            {
                _db.StockTransactions.Add(new StockTransaction
                {
                    InvoiceType = InvoiceTypeEnum.AP,
                    InvoiceId = 0,
                    InvoiceLineId = 0,
                    ItemId = item.Id,
                    Qty = request.StockQuantity
                });

                await _db.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, new ItemResponseDto
            {
                Id = item.Id,
                ItemName = item.ItemName,
                CategoryName = (await _db.Categories.FindAsync(item.CategoryId))?.Name ?? "",
                UnitAbbreviation = uom.Abbreviation,
                StockQuantity = request.StockQuantity,
                BuyPrice = item.BuyPrice,
                SalePrice = item.SalePrice,
                AllowNegativeInventory = item.AllowNegativeInventory
            });
        }

        [HttpGet("download-template")]
        public IActionResult DownloadTemplate([FromServices] ExcelTemplateService excelService)
        {
            var file = excelService.GenerateTemplate();
            return File(file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "BulkUploadTemplate.xlsx");
        }

        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel(IFormFile file, [FromServices] ExcelUploadService excelService)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            using var stream = file.OpenReadStream();
            var response = await excelService.ProcessExcelAsync(stream);
            return Ok(response);
        }

        // PUT: api/items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, ItemRequestDto request)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null)
                return NotFound();

            var uom = await _db.UnitsOfMeasure
                .FirstOrDefaultAsync(u => u.Abbreviation == request.UnitAbbreviation);

            if (uom == null)
                return NotFound($"Unit of Measure '{request.UnitAbbreviation}' not found.");

            item.ItemName = request.ItemName;
            item.CategoryId = request.CategoryId;
            item.UnitOfMeasureId = uom.Id;
            item.BuyPrice = request.BuyPrice;
            item.SalePrice = request.SalePrice;
            item.AllowNegativeInventory = request.AllowNegativeInventory;

            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/items/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _db.Items.FindAsync(id);

            if (item == null)
                return NotFound(new { msg = "Item not found" });

            item.IsActive = false;
            item.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { msg = "Item deactivated successfully" });
        }
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateItem(int id)
        {
            var item = await _db.Items.FindAsync(id);

            if (item == null)
                return NotFound(new { msg = "Item not found" });

            item.IsActive = true;
            item.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { msg = "Item activated successfully" });
        }
    }
}