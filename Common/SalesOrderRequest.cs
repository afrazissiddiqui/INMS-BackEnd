namespace Inventory.Api.Common
{
    public class SalesOrderRequest
    {
        public int BusinessPartnerId { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        /// <summary>
        /// Draft / Confirmed / Cancelled
        /// </summary>
        public string Status { get; set; } = "Draft";

        public List<SalesOrderLineRequest> Lines { get; set; } = new();
    }

    public class SalesOrderLineRequest
    {
        public int ItemId { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
    }
}
