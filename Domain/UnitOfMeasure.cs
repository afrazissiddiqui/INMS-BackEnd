using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain
{
    public class UnitOfMeasure : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Abbreviation { get; set; } = string.Empty;
    }
}
