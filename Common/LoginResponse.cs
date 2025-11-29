using Inventory.Api.Domain;

namespace Inventory.Api.Common
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public IList<AssignPermission>? Permissions { get; set; }
    }
}
