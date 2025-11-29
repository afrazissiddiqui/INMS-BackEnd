using Inventory.Api.Common;
using Inventory.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<DashboardResponse>>> GetDashboard()
        {
            try
            {
                var response = new DashboardResponse
                {
                    TotalAPInvoices = await _db.APInvoices.CountAsync(),
                    TotalARInvoices = await _db.ARInvoices.CountAsync(), // You said you already have ARInvoice model
                    TotalVendors = await _db.BusinessPartners.CountAsync(x => x.Type == "Vendor"),
                    TotalCustomers = await _db.BusinessPartners.CountAsync(x => x.Type == "Customer"),
                    TotalItems = await _db.Items.CountAsync()
                };

                return ApiResponse.Success(response);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<DashboardResponse>(500, ex.Message);
            }
        }
    }
}
