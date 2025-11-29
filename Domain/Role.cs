using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain
{
    public class Role : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
    }
}
