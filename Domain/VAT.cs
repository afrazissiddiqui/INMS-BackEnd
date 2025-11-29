using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Api.Domain
{
    public class VAT : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]   // auto-increment PK
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rate { get; set; }
    }
}
