using Inventory.Api.Domain;

namespace Inventory.Api.Common
{
    public class ARInvoiceResponse
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public string BusinessPartnerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<ARInvoiceLineResponse> Lines { get; set; } = new();

        public static ARInvoiceResponse FromEntity(ARInvoice invoice)
        {
            return new ARInvoiceResponse
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                BusinessPartnerName = invoice.BusinessPartner?.FirstName ?? "",
                TotalAmount = invoice.TotalAmount,
                Lines = invoice.Lines.Select(l => new ARInvoiceLineResponse
                {
                    ItemId = l.ItemId,
                    ItemName = l.Item?.ItemName ?? "",
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    VatRate = l.Vat?.Rate ?? 0,
                    LineTotal = l.LineTotal,
                }).ToList()
            };
        }
    }

    public class ARInvoiceLineResponse
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal LineTotal { get; set; }
        public int RemainingStock { get; set; }
    }
}
namespace Inventory.Api.Common
{
    public class ARInvoiceCreateRequest
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public int BusinessPartnerId { get; set; }
        public List<ARInvoiceLineCreateRequest> Lines { get; set; } = new();
    }

    public class ARInvoiceLineCreateRequest
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public int? VatId { get; set; }
    }
}
