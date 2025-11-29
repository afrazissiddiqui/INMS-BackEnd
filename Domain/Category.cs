using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = "";
        public string Base64 { get; set; } = "";
    }
}