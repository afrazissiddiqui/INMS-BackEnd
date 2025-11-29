using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public RolesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Role>>>> GetAll()
        {
            try
            {
                var data = await _db.Roles.AsNoTracking().ToListAsync();
                return ApiResponse.Success<IEnumerable<Role>>(data);
            }
            catch (Exception ex) { return ApiResponse.Fail<IEnumerable<Role>>(500, ex.Message); }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<Role>>> GetById(int id)
        {
            try
            {
                var entity = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return entity == null ? ApiResponse.Fail<Role>(404, "Role not found")
                                      : ApiResponse.Success(entity);
            }
            catch (Exception ex) { return ApiResponse.Fail<Role>(500, ex.Message); }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Role>>> Create(Role model)
        {
            try
            {
                if (!ModelState.IsValid) return ApiResponse.Fail<Role>(400, "Validation failed");

                _db.Roles.Add(model);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = model.Id }, ApiResponse.Created(model));
            }
            catch (Exception ex) { return ApiResponse.Fail<Role>(500, ex.Message); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<Role>>> Update(int id, Role model)
        {
            try
            {
                if (!ModelState.IsValid) return ApiResponse.Fail<Role>(400, "Validation failed");

                var entity = await _db.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<Role>(404, "Role not found");

                entity.Name = model.Name;
                entity.IsActive = model.IsActive;
                await _db.SaveChangesAsync();

                return ApiResponse.Success(entity, "Updated");
            }
            catch (Exception ex) { return ApiResponse.Fail<Role>(500, ex.Message); }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var entity = await _db.Roles.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<object>(404, "Role not found");

                entity.IsDeleted = true;
                await _db.SaveChangesAsync();
                return ApiResponse.Success<object>(null, "Deleted");
            }
            catch (Exception ex) { return ApiResponse.Fail<object>(500, ex.Message); }
        }
    }
}
