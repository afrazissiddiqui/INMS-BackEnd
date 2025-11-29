using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Domain
{
    [Index(nameof(RoleId), nameof(PermissionId), IsUnique = true)]
    public class AssignPermission : BaseEntity
    {
        public int Id { get; set; }

        
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        /// <summary>
        /// Matches PermissionModule enum value.
        /// </summary>
        [Required]
        public int PermissionId { get; set; }

        /// <summary>
        /// Stored for convenience; should mirror enum's name.
        /// </summary>
        [Required, MaxLength(200)]
        public string PermissionName { get; set; } = string.Empty;

        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanRead { get; set; }
        public bool CanDelete { get; set; }
    }
}