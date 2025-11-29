using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain
{
    public class SignUp : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(100)]
        public string LastName { get; set; } = "";

        [Required, MaxLength(255), EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [MaxLength(25)]
        public string CNIC { get; set; } = "";

    }
}
