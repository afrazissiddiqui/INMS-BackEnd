using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain
{
    public class StockTransaction : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Use enum instead of string
        public InvoiceTypeEnum InvoiceType { get; set; } = InvoiceTypeEnum.AP;

        public int InvoiceId { get; set; }

        public int InvoiceLineId { get; set; }

        public int ItemId { get; set; }

        // +ve for stock in, -ve for stock out
        public decimal Qty { get; set; }
    }
}