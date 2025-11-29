namespace Inventory.Api.Common.DTOs
{
    public class ItemRequestDto
    {
        public string ItemName { get; set; } = "";
        public int CategoryId { get; set; }
        public string UnitAbbreviation { get; set; } = "";
        public decimal StockQuantity { get; set; } = 0;
        public bool AllowNegativeInventory { get; set; } = false;
    }
}
