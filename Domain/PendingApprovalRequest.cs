/*using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Domain
{
    public class PendingApprovalRequest : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // Type of request (APInvoice, ARInvoice)
        [Required]
        public string RequestType { get; set; } = "";

        // The ID of the invoice (AP or AR)
        [Required]
        public int RequestId { get; set; }

        // Status of the request: Pending / Approved / Rejected
        [Required]
        public ApprovalStatusEnum Status { get; set; } = ApprovalStatusEnum.Pending;

        // Track who requested it
        public int CreatedByUserId { get; set; }

        // Track approvals by admins
        public ICollection<ApprovalDetail> ApprovalDetails { get; set; } = new List<ApprovalDetail>();
    }

    public class ApprovalDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AdminUserId { get; set; }

        // Approved or Rejected by this admin
        [Required]
        public ApprovalStatusEnum Status { get; set; } = ApprovalStatusEnum.Pending;
        public List<ApprovalDetail> ApprovalDetails { get; set; } = new();


    }
}
*/