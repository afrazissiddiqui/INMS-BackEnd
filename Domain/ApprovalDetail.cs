/*using Inventory.Api.Domain;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement6th.Domain
{
    public class ApprovalDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdminUserId { get; set; }

        [Required]
        public ApprovalStatusEnum Status { get; set; } = ApprovalStatusEnum.Pending;

        // Missing relationship – ADD THIS
        public int PendingApprovalRequestId { get; set; }
        public PendingApprovalRequest? PendingApprovalRequest { get; set; }

    }
}
*/