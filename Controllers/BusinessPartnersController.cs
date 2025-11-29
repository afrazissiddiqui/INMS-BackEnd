using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessPartnersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public BusinessPartnersController(AppDbContext db) => _db = db;

        private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
        { "Vendor", "Customer" };

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<BusinessPartner>>>> GetAll()
        {
            try
            {
                var data = await _db.BusinessPartners.AsNoTracking().ToListAsync();
                return ApiResponse.Success<IEnumerable<BusinessPartner>>(data);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<IEnumerable<BusinessPartner>>(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<BusinessPartner>>> GetById(int id)
        {
            try
            {
                var entity = await _db.BusinessPartners
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                return entity == null
                    ? ApiResponse.Fail<BusinessPartner>(404, "BusinessPartner not found")
                    : ApiResponse.Success(entity);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<BusinessPartner>(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<BusinessPartner>>> Create(BusinessPartner model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse.Fail<BusinessPartner>(400, "Validation failed");

                if (!AllowedTypes.Contains(model.Type))
                    return ApiResponse.Fail<BusinessPartner>(400, "Type must be 'Vendor' or 'Customer'");

                // ✅ CNIC/NTN validation depending on filer status
                if (model.IsFiler && string.IsNullOrWhiteSpace(model.NTN))
                    return ApiResponse.Fail<BusinessPartner>(400, "Filer must provide NTN");
                if (!model.IsFiler && string.IsNullOrWhiteSpace(model.CNIC))
                    return ApiResponse.Fail<BusinessPartner>(400, "Non-filer must provide CNIC");

                _db.BusinessPartners.Add(model);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = model.Id }, ApiResponse.Created(model));
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<BusinessPartner>(500, ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<BusinessPartner>>> Update(int id, BusinessPartner model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse.Fail<BusinessPartner>(400, "Validation failed");

                if (!AllowedTypes.Contains(model.Type))
                    return ApiResponse.Fail<BusinessPartner>(400, "Type must be 'Vendor' or 'Customer'");

                var entity = await _db.BusinessPartners.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return ApiResponse.Fail<BusinessPartner>(404, "BusinessPartner not found");

                // ✅ CNIC/NTN validation
                if (model.IsFiler && string.IsNullOrWhiteSpace(model.NTN))
                    return ApiResponse.Fail<BusinessPartner>(400, "Filer must provide NTN");
                if (!model.IsFiler && string.IsNullOrWhiteSpace(model.CNIC))
                    return ApiResponse.Fail<BusinessPartner>(400, "Non-filer must provide CNIC");

                // Update entity
                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.Type = model.Type;
                entity.CNIC = model.CNIC;
                entity.NTN = model.NTN;
                entity.IsFiler = model.IsFiler;
                entity.Email = model.Email;
                entity.IsActive = model.IsActive;

                await _db.SaveChangesAsync();

                return ApiResponse.Success(entity, "Updated");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<BusinessPartner>(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var entity = await _db.BusinessPartners.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null)
                    return ApiResponse.Fail<object>(404, "BusinessPartner not found");

                entity.IsDeleted = true;
                await _db.SaveChangesAsync();

                return ApiResponse.Success<object>(null, "Deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<object>(500, ex.Message);
            }
        }
    }
}