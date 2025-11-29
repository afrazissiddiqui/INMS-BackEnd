namespace Inventory.Api.Common
{
    public class SalesOrderResponse
    {
        public int Id { get; set; }

        public int BusinessPartnerId { get; set; }
        public string? BusinessPartnerName { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public string Status { get; set; } = "Draft";

        public decimal DocTotal { get; set; }

        public List<SalesOrderLineResponse> Lines { get; set; } = new();
    }

    public class SalesOrderLineResponse
    {
        public int Id { get; set; }

        public int ItemId { get; set; }
        public string? ItemName { get; set; }

        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }
}
