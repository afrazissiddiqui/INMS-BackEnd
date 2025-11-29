using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Api.Domain
{
    public class Item : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string ItemName { get; set; } = "";

        public string Base64 { get; set; } = "";

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        // 🔹 Add UnitOfMeasure relation
        [Required]
        public int UnitOfMeasureId { get; set; }

        [ForeignKey(nameof(UnitOfMeasureId))]
        public UnitOfMeasure? UnitOfMeasure { get; set; }

        public bool AllowNegativeInventory { get; set; }
    }
}
