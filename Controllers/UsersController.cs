using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db) => _db = db;

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse<User>>> SignUp(SignUpRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return ApiResponse.Fail<User>(400, "Validation failed");

                // Check if email already exists
                if (await _db.Users.AnyAsync(u => u.Email == request.Email && u.IsActive))
                    return ApiResponse.Fail<User>(400, "Email already exists");

                // Set default role for all signups
                int defaultRoleId = 1;
                if (!await _db.Roles.AnyAsync(r => r.Id == defaultRoleId))
                    return ApiResponse.Fail<User>(400, "Default role not found");

           /*     // Hash password (simple SHA256 for now)
                string hashedPassword;
                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    var bytes = System.Text.Encoding.UTF8.GetBytes(request.Password);
                    var hash = sha256.ComputeHash(bytes);
                    hashedPassword = Convert.ToBase64String(hash);
                }*/

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,

                    Email = request.Email,
                    CNIC = request.CNIC,
                    Password = request.Password,
                    RoleId = defaultRoleId,  // <— default role assigned here
                    IsActive = true,
                    IsDeleted = false
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                var saved = await _db.Users.Include(u => u.Role).FirstAsync(u => u.Id == user.Id);
                return CreatedAtAction(nameof(GetById), new { id = saved.Id }, ApiResponse.Created(saved));
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<User>(500, ex.Message);
            }
        }



        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<User>>>> GetAll()
        {
            try
            {
                var data = await _db.Users.Include(u => u.Role).AsNoTracking().ToListAsync();
                return ApiResponse.Success<IEnumerable<User>>(data);
            }
            catch (Exception ex) { return ApiResponse.Fail<IEnumerable<User>>(500, ex.Message); }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<User>>> GetById(int id)
        {
            try
            {
                var entity = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(x => x.Id == id);
                return entity == null ? ApiResponse.Fail<User>(404, "User not found")
                                      : ApiResponse.Success(entity);
            }
            catch (Exception ex) { return ApiResponse.Fail<User>(500, ex.Message); }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<User>>> Create(User model)
        {
            try
            {
                if (!ModelState.IsValid) return ApiResponse.Fail<User>(400, "Validation failed");

                if (await _db.Users.AnyAsync(u => u.Email == model.Email && model.IsActive == true))
                    return ApiResponse.Fail<User>(400, "Email already exists");

                if (!await _db.Roles.AnyAsync(r => r.Id == model.RoleId))
                    return ApiResponse.Fail<User>(400, "Invalid RoleId");

                _db.Users.Add(model);
                await _db.SaveChangesAsync();

                var saved = await _db.Users.Include(u => u.Role).FirstAsync(u => u.Id == model.Id);
                return CreatedAtAction(nameof(GetById), new { id = saved.Id }, ApiResponse.Created(saved));
            }
            catch (Exception ex) { return ApiResponse.Fail<User>(500, ex.Message); }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<User>>> Update(int id, User model)
        {
            try
            {
                if (!ModelState.IsValid) return ApiResponse.Fail<User>(400, "Validation failed");

                var entity = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<User>(404, "User not found");

                if (entity.Email != model.Email && await _db.Users.AnyAsync(u => u.Email == model.Email))
                    return ApiResponse.Fail<User>(400, "Email already exists");

                if (!await _db.Roles.AnyAsync(r => r.Id == model.RoleId))
                    return ApiResponse.Fail<User>(400, "Invalid RoleId");

                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.CNIC = model.CNIC;
                entity.Email = model.Email;
                entity.Password = model.Password; // phase-1
                entity.RoleId = model.RoleId;
                entity.IsActive = model.IsActive;

                await _db.SaveChangesAsync();

                var saved = await _db.Users.Include(u => u.Role).FirstAsync(u => u.Id == entity.Id);
                return ApiResponse.Success(saved, "Updated");
            }
            catch (Exception ex) { return ApiResponse.Fail<User>(500, ex.Message); }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            try
            {
                var entity = await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (entity == null) return ApiResponse.Fail<object>(404, "User not found");

                entity.IsDeleted = true;
                entity.IsActive = false;
                await _db.SaveChangesAsync();
                return ApiResponse.Success<object>(null, "Deleted");

            }
            catch (Exception ex) { return ApiResponse.Fail<object>(500, ex.Message); }
        }
    }
}
