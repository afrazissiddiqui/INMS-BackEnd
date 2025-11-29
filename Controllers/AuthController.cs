using Inventory.Api.Common;
using Inventory.Api.Data;
using Inventory.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return ApiResponse.Fail<LoginResponse>(400, "Email and Password are required");

                var user = await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

                if (user == null)
                {
                    return ApiResponse.Fail<LoginResponse>(401, "Invalid email or password");
                }
                else
                {

                    var response = new LoginResponse
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = $"{user.FirstName} {user.LastName}",
                        Role = user.Role?.Name ?? "",

                    };
                    response.Permissions = _db.AssignPermissions.Where(X => X.RoleId == user.RoleId).ToList();

                    return ApiResponse.Success(response, "Login successful");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail<LoginResponse>(500, ex.Message);
            }
        }
    }
}