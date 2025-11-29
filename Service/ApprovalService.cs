/*using Inventory.Api.Data;
using Inventory.Api.Domain;

namespace InventoryManagement6th.Service
{

    public class ApprovalService
    {
        private readonly AppDbContext _context;

        public ApprovalService(AppDbContext context)
        {
            _context = context;
        }
        public int PendingApprovalRequestId { get; set; }
        public PendingApprovalRequest PendingApprovalRequest { get; set; }
        public List<ApprovalDetail> ApprovalDetails { get; set; } = new();

        public async Task<PendingApprovalRequest> SubmitRequest(string requestType, int requestId, int createdByUserId)
        {
            var request = new PendingApprovalRequest
            {
                RequestType = requestType,
                RequestId = requestId,
                CreatedByUserId = createdByUserId,
                Status = ApprovalStatusEnum.Pending

            };

            _context.PendingApprovalRequests.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }
    }

}*/