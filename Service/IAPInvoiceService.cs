using Inventory.Api.Common;

namespace Inventory.Api.Services
{
    public interface IAPInvoiceService
    {
        Task<ApiResponse<IEnumerable<APInvoiceResponse>>> GetAllAsync();
        Task<ApiResponse<APInvoiceResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<APInvoiceResponse>> CreateAsync(APInvoiceCreateRequest request);
        Task<ApiResponse<APInvoiceResponse>> UpdateAsync(int id, APInvoiceCreateRequest request);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
