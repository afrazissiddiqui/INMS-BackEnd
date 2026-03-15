namespace Inventory.Api.Common.DTOs
{
    public class ItemRequestDto
    {
        public string ItemName { get; set; } = "";
        public int CategoryId { get; set; }
        public string UnitAbbreviation { get; set; } = "";
        public decimal StockQuantity { get; set; } = 0;

        // 🔹 Add Buy Price
        public decimal BuyPrice { get; set; }

        // 🔹 Add Sale Price
        public decimal SalePrice { get; set; }

        public bool AllowNegativeInventory { get; set; } = false;
    }
}