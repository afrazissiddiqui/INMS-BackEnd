namespace Inventory.Api.Common
{
    public class DashboardResponse
    {
        public int TotalAPInvoices { get; set; }
        public int TotalARInvoices { get; set; }
        public int TotalVendors { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalItems { get; set; }
    }
}
