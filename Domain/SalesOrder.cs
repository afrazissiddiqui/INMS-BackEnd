namespace Inventory.Api.Domain
{
    public class SalesOrder : BaseEntity
    {
        public int Id { get; set; }

        // Which customer is making the order
        public int BusinessPartnerId { get; set; }
        public BusinessPartner? BusinessPartner { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        // Example: Draft, Confirmed, Cancelled
        public string Status { get; set; } = "Draft";

        public decimal DocTotal { get; set; }

        public List<SalesOrderLine> Lines { get; set; } = new();
    }

    public class SalesOrderLine : BaseEntity
    {
        public int Id { get; set; }

        public int SalesOrderId { get; set; }
        public SalesOrder? SalesOrder { get; set; }

        public int ItemId { get; set; }
        public Item? Item { get; set; }

        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal { get; set; }
    }
}
