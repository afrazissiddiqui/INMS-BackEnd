using ClosedXML.Excel;

namespace InventoryManagement6th.Service
{
    public class ExcelTemplateService
    {
        public byte[] GenerateTemplate()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("ItemsTemplate");

            // ✅ Add headers
            worksheet.Cell(1, 1).Value = "ItemName";
            worksheet.Cell(1, 2).Value = "Category";
            worksheet.Cell(1, 3).Value = "UOM";
            worksheet.Cell(1, 4).Value = "VAT Code";
            worksheet.Cell(1, 5).Value = "VAT Description";
            worksheet.Cell(1, 6).Value = "VAT Rate";

            // ✅ Style headers
            var headerRange = worksheet.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ✅ Set column widths
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}