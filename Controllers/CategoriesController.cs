using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Category>>>> GetAll()
        {
            try
            {
                var data = await _db.Categories.AsNoTracking().ToListAsync();
                return ApiResponse.Success<IEnumerable<Category>>(data);
            }
            catch (Exception ex) { return ApiResponse.Fail<IEnumerable<Category>>(500, ex.Message); }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<Category>>> GetById(int id)
        {
            try
            {
                var entity = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return entity == null ? ApiResponse.Fail<Category>(404, "Category not found")
                                      : ApiResponse.Success(entity);
            }
            catch (Exception ex) { return ApiResponse.Fail<Category>(500, ex.Message); }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Category>>> Create(Category model)
        {
            try
            {
                if (!ModelState.IsValid) return ApiResponse.Fail<Category>(400, "Validation failed");

                _db.Categories.Add(model);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = model.Id }, ApiResponse.Created(model));
            }
            catch (Exception ex) { return ApiResponse.Fail<Category>(500, ex.Message); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<Category>>> Update(int id, Category model)
        {
            try
            {
                if (!ModelState.IsValid) return ApiResponse.Fail<Category>(400, "Validation failed");

                var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<Category>(404, "Category not found");

                entity.Name = model.Name;
                entity.IsActive = model.IsActive;
                await _db.SaveChangesAsync();

                return ApiResponse.Success(entity, "Updated");
            }
            catch (Exception ex) { return ApiResponse.Fail<Category>(500, ex.Message); }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<object>(404, "Category not found");

                entity.IsDeleted = true;
                await _db.SaveChangesAsync();
                return ApiResponse.Success<object>(null, "Deleted");
            }
            catch (Exception ex) { return ApiResponse.Fail<object>(500, ex.Message); }
        }
    }
}
