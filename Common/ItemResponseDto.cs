namespace Inventory.Api.Common.DTOs
{
    public class ItemResponseDto
    {
        public int Id { get; set; }
        public string ItemName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string UnitAbbreviation { get; set; } = "";
        public decimal StockQuantity { get; set; }
        public bool AllowNegativeInventory { get; set; }
    }
}
