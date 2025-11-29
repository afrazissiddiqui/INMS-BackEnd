using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Domain
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(100)]
        public string LastName { get; set; } = "";
        [Column(TypeName = "nvarchar(max)")]
        public string base64 { get; set; } = "";

        // ✅ CNIC: 13-digit with dashes (#####-#######-#)
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$",
            ErrorMessage = "CNIC must be in #####-#######-# format.")]
        public string? CNIC { get; set; }

        [Required, MaxLength(200)]
        public string Email { get; set; } = "";

        // Phase-1: plain Password per your spec (consider hashing later)
        [Required]
        public string Password { get; set; } = "";

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }
    }
}
