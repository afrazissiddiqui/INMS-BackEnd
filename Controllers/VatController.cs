using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using InventoryManagement6th.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement6th.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VATController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VATController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VAT>>> GetVATs()
        {
            return await _context.Set<VAT>().ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VAT>> GetVAT(int id)
        {
            var vat = await _context.Set<VAT>().FindAsync(id);
            if (vat == null)
            {
                return NotFound();
            }
            return vat;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<VatResponse>>> CreateVAT([FromBody] VatCreateRequest request)
        {
            try
            {
                var vat = new VAT
                {
                    Code = request.Code,
                    Description = request.Description,
                    Rate = request.Rate
                };

                _context.VATs.Add(vat);
                await _context.SaveChangesAsync();

                var response = new VatResponse
                {
                    Id = vat.Id,
                    Code = vat.Code,
                    Description = vat.Description,
                    Rate = vat.Rate
                };

                return ApiResponse.Success(response);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<VatResponse>(500, ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVAT(int id, VAT vat)
        {
            if (id != vat.Id)
            {
                return BadRequest();
            }

            _context.Entry(vat).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVAT(int id)
        {
            var vat = await _context.Set<VAT>().FindAsync(id);
            if (vat == null)
            {
                return NotFound();
            }

            _context.Set<VAT>().Remove(vat);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
