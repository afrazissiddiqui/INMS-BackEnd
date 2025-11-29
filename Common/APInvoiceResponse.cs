using Inventory.Api.Domain;
using Inventory.Api.Data;   // ✅ needed for AppDbContext

namespace Inventory.Api.Common
{
public class APInvoiceResponse
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public string BusinessPartnerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<APInvoiceLineResponse> Lines { get; set; } = new();

    // ✅ Implement properly for APInvoice with stock lookup
    public static APInvoiceResponse FromEntity(APInvoice invoice, AppDbContext context)
    {
        return new APInvoiceResponse
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceDate = invoice.InvoiceDate,
            BusinessPartnerName = invoice.BusinessPartner?.FirstName ?? "",
            TotalAmount = invoice.TotalAmount,
            Lines = invoice.Lines.Select(l => new APInvoiceLineResponse
            {
                ItemId = l.ItemId,
                ItemName = l.Item?.ItemName ?? "",
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                VatRate = l.Vat?.Rate ?? 0,
                LineTotal = l.LineTotal,

                // ✅ fetch remaining stock directly from StockTransactions
                RemainingStock = context.StockTransactions
                    .Where(st => st.ItemId == l.ItemId)
                    .Sum(st => st.Qty)
            }).ToList()
        };
    }
}
    public class APInvoiceLineResponse
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal LineTotal { get; set; }
        public decimal RemainingStock { get; set; }   // now populated correctly
    }
}


    public class APInvoiceCreateRequest
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int BusinessPartnerId { get; set; }
        public List<APInvoiceLineCreateRequest> Lines { get; set; } = new();
    }

    public class APInvoiceLineCreateRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int? VatId { get; set; }
    }

