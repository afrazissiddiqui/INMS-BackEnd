using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Api.Domain
{
    public class BusinessPartner : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FirstName { get; set; } = "";

        [MaxLength(200)]
        public string LastName { get; set; } = "";

        [MaxLength(200)]
        public string BusinessName { get; set; } = "";

        [MaxLength(500)]
        public string BusinessAddress { get; set; } = "";

        // "Vendor" or "Customer"
        [Required, MaxLength(50)]
        public string Type { get; set; } = "";

        [Column(TypeName = "nvarchar(max)")]
        public string base64 { get; set; } = "";

        // ✅ CNIC: 13-digit with dashes (#####-#######-#)
        [RegularExpression(@"^\d{5}-\d{7}-\d{1}$",
            ErrorMessage = "CNIC must be in #####-#######-# format.")]
        public string? CNIC { get; set; }

        // ✅ NTN: 7 digits, optional -digit (e.g., 1234567 or 1234567-8)
        [RegularExpression(@"^\d{7}(-\d)?$",
            ErrorMessage = "NTN must be 7 digits, optional -digit suffix.")]
        public string? NTN { get; set; }

        [EmailAddress, MaxLength(200)]
        public string? Email { get; set; }

        // ✅ Fix: must be a string for regex and leading zeros
        [RegularExpression(@"^\d{11}$",
            ErrorMessage = "PhoneNumber must be exactly 11 digits.")]
        public string PhoneNumber { get; set; } = "";

        // ✅ Extra property to indicate filer status
        public bool IsFiler { get; set; }
    }
}
