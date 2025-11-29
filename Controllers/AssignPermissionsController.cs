using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using InventoryManagement5th.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignPermissionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AssignPermissionsController(AppDbContext db) => _db = db;

        // GET: api/AssignPermissions/permissions
        // Returns all available permissions from the enum
        [HttpGet("permissions")]
        public ActionResult<ApiResponse<IEnumerable<PermissionCatalogItem>>> GetPermissionCatalog()
        {
            try
            {
                var items = Enum.GetValues(typeof(PermissionModule))
                                .Cast<PermissionModule>()
                                .Select(e => new PermissionCatalogItem
                                {
                                    Id = (int)e,
                                    Name = e.ToString()
                                })
                                .ToList();

                return ApiResponse.Success<IEnumerable<PermissionCatalogItem>>(items);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<IEnumerable<PermissionCatalogItem>>(500, ex.Message);
            }
        }

        // GET: api/AssignPermissions/by-role/5
        [HttpGet("by-role/{roleId:int}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssignPermission>>>> GetByRole(int roleId)
        {
            try
            {
                var exists = await _db.Roles.AsNoTracking().AnyAsync(r => r.Id == roleId && !r.IsDeleted);
                if (!exists) return ApiResponse.Fail<IEnumerable<AssignPermission>>(404, "Role not found");

                var data = await _db.AssignPermissions
                                    .AsNoTracking()
                                    .Where(p => p.RoleId == roleId && !p.IsDeleted)
                                    .ToListAsync();

                return ApiResponse.Success<IEnumerable<AssignPermission>>(data);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<IEnumerable<AssignPermission>>(500, ex.Message);
            }
        }

        // GET: api/AssignPermissions/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AssignPermission>>> Get(int id)
        {
            try
            {
                var entity = await _db.AssignPermissions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return entity == null
                    ? ApiResponse.Fail<AssignPermission>(404, "Permission assignment not found")
                    : ApiResponse.Success(entity);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<AssignPermission>(500, ex.Message);
            }
        }

        // POST: api/AssignPermissions
        // Creates or upserts a single permission for a role
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AssignPermission>>> Create(AssignPermissionCreateDto dto)
        {
            try
            {
                // Validate role
                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == dto.RoleId && !r.IsDeleted);
                if (role == null) return ApiResponse.Fail<AssignPermission>(404, "Role not found");

                // Validate permission enum
                if (!Enum.IsDefined(typeof(PermissionModule), dto.PermissionId))
                    return ApiResponse.Fail<AssignPermission>(400, "Invalid PermissionId");

                var permissionName = ((PermissionModule)dto.PermissionId).ToString();

                // Upsert by (RoleId, PermissionId)
                var existing = await _db.AssignPermissions
                                        .FirstOrDefaultAsync(p => p.RoleId == dto.RoleId
                                                                && p.PermissionId == dto.PermissionId);

                if (existing == null)
                {
                    var entity = new AssignPermission
                    {
                        RoleId = dto.RoleId,
                        PermissionId = dto.PermissionId,
                        PermissionName = permissionName,
                        CanCreate = dto.CanCreate,
                        CanUpdate = dto.CanUpdate,
                        CanRead = dto.CanRead,
                        CanDelete = dto.CanDelete,
                        IsActive = true
                    };

                    _db.AssignPermissions.Add(entity);
                    await _db.SaveChangesAsync();

                    return CreatedAtAction(nameof(Get), new { id = entity.Id }, ApiResponse.Created(entity));
                }
                else
                {
                    existing.PermissionName = permissionName; // keep in sync
                    existing.CanCreate = dto.CanCreate;
                    existing.CanUpdate = dto.CanUpdate;
                    existing.CanRead = dto.CanRead;
                    existing.CanDelete = dto.CanDelete;
                    existing.IsActive = true;
                    existing.IsDeleted = false;

                    await _db.SaveChangesAsync();
                    return ApiResponse.Success(existing, "Updated");
                }
            }
            catch (DbUpdateException dbEx)
            {
                return ApiResponse.Fail<AssignPermission>(400, dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<AssignPermission>(500, ex.Message);
            }
        }

        // PUT: api/AssignPermissions/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AssignPermission>>> Update(int id, AssignPermissionUpdateDto dto)
        {
            try
            {
                var entity = await _db.AssignPermissions.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<AssignPermission>(404, "Permission assignment not found");

                entity.CanCreate = dto.CanCreate;
                entity.CanUpdate = dto.CanUpdate;
                entity.CanRead = dto.CanRead;
                entity.CanDelete = dto.CanDelete;
                entity.IsActive = dto.IsActive;

                await _db.SaveChangesAsync();
                return ApiResponse.Success(entity, "Updated");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<AssignPermission>(500, ex.Message);
            }
        }

        // DELETE (soft): api/AssignPermissions/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var entity = await _db.AssignPermissions.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<object>(404, "Permission assignment not found");

                entity.IsDeleted = true;
                entity.IsActive = false;
                await _db.SaveChangesAsync();

                return ApiResponse.Success<object>(null, "Deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<object>(500, ex.Message);
            }
        }

        // OPTIONAL: bulk upsert for a role
        // POST: api/AssignPermissions/bulk/5
        [HttpPost("bulk/{roleId:int}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<AssignPermission>>>> BulkUpsert(
            int roleId, IEnumerable<AssignPermissionCreateDto> items)
        {
            try
            {
                var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
                if (role == null) return ApiResponse.Fail<IEnumerable<AssignPermission>>(404, "Role not found");

                var results = new List<AssignPermission>();

                foreach (var dto in items.Where(i => i.RoleId == roleId))
                {
                    if (!Enum.IsDefined(typeof(PermissionModule), dto.PermissionId))
                        continue;

                    var name = ((PermissionModule)dto.PermissionId).ToString();

                    var existing = await _db.AssignPermissions
                                            .FirstOrDefaultAsync(p => p.RoleId == roleId
                                                                    && p.PermissionId == dto.PermissionId);

                    if (existing == null)
                    {
                        var entity = new AssignPermission
                        {
                            RoleId = roleId,
                            PermissionId = dto.PermissionId,
                            PermissionName = name,
                            CanCreate = dto.CanCreate,
                            CanUpdate = dto.CanUpdate,
                            CanRead = dto.CanRead,
                            CanDelete = dto.CanDelete,
                            IsActive = true
                        };
                        _db.AssignPermissions.Add(entity);
                        results.Add(entity);
                    }
                    else
                    {
                        existing.PermissionName = name;
                        existing.CanCreate = dto.CanCreate;
                        existing.CanUpdate = dto.CanUpdate;
                        existing.CanRead = dto.CanRead;
                        existing.CanDelete = dto.CanDelete;
                        existing.IsActive = true;
                        existing.IsDeleted = false;
                        results.Add(existing);
                    }
                }

                await _db.SaveChangesAsync();
                return ApiResponse.Success<IEnumerable<AssignPermission>>(results, "Bulk upsert completed");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<IEnumerable<AssignPermission>>(500, ex.Message);
            }
        }
    }
  
    public class PermissionCatalogItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class AssignPermissionCreateDto
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanRead { get; set; }
        public bool CanDelete { get; set; }
    }

    public class AssignPermissionUpdateDto
    {
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanRead { get; set; }
        public bool CanDelete { get; set; }
        public bool IsActive { get; set; }
    }

}
