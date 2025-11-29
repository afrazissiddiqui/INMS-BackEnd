using Inventory.Api.Common;
using Inventory.Api.Services;
using InventoryManagement6th.Service;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class APInvoiceController : ControllerBase
    {
        

        
        private readonly IAPInvoiceService _service;

        public APInvoiceController(IAPInvoiceService service)
        {
            _service = service;
        }

        // 🔹 GET: api/APInvoice
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<APInvoiceResponse>>>> GetAll()
        {
            // Includes VAT via service (service uses .Include(l => l.Vat))
            return await _service.GetAllAsync();
        }

        // 🔹 GET: api/APInvoice/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<APInvoiceResponse>>> GetById(int id)
        {
            return await _service.GetByIdAsync(id);
        }

        // 🔹 POST: api/APInvoice
        [HttpPost]
        public async Task<ActionResult<ApiResponse<APInvoiceResponse>>> Create([FromBody] APInvoiceCreateRequest request)
        {
            if (request == null)
                return BadRequest("Request body cannot be null.");

            // Service hydrates VAT for each line before totals
            return await _service.CreateAsync(request);
        }

        // 🔹 PUT: api/APInvoice/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<APInvoiceResponse>>> Update(
            int id, [FromBody] APInvoiceCreateRequest request)
        {
            if (request == null)
            {
                Console.WriteLine("⚠️ Request body was NULL.");
                Console.WriteLine($"Content-Type: {Request.ContentType}");

                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                Console.WriteLine($"RAW BODY: {body}");

                return BadRequest("Request body is null. Check JSON format and Content-Type.");
            }

            // Service hydrates VAT for each line before totals
            return await _service.UpdateAsync(id, request);
        }

        // 🔹 DELETE: api/APInvoice/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            return await _service.DeleteAsync(id);
        }
    }
}