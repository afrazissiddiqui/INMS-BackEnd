using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Api.Domain
{
    public class APInvoice : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; }

        // Business Partner (Vendor / Supplier)
        [Required]
        public int BusinessPartnerId { get; set; }

        [ForeignKey(nameof(BusinessPartnerId))]
        public BusinessPartner? BusinessPartner { get; set; }

        public ICollection<APInvoiceLine> Lines { get; set; } = new List<APInvoiceLine>();

        //// Optional: Store total VAT + Grand Total at Invoice level
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
    }

    public class APInvoiceLine : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int APInvoiceId { get; set; }

        [ForeignKey(nameof(APInvoiceId))]
        public APInvoice? APInvoice { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // ✅ VAT Integration
        public int? VatId { get; set; }

        [ForeignKey(nameof(VatId))]
        public VAT? Vat { get; set; }

        // ✅ Computed field (not stored, just for response)
        [NotMapped]
        public decimal LineTotal => (Quantity * UnitPrice)
                                  + ((Quantity * UnitPrice) * (Vat?.Rate ?? 0) / 100);
    }
}
