namespace FriterieShop.API.Controllers
{
    using FriterieShop.Domain.Contracts.Authentication;
    using FriterieShop.Domain.Entities.Identity;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IAppUserManager _userManager;
        private readonly IAppRoleManager _roleManager;
        private readonly UserManager<AppUser> _identityUserManager;

        public UserManagementController(
            IAppUserManager userManager,
            IAppRoleManager roleManager,
            UserManager<AppUser> identityUserManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _identityUserManager = identityUserManager;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.GetAllUsersAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                if (user is null) continue;

                var role = await _roleManager.GetUserRoleAsync(user.Email!) ?? string.Empty;
                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    Email = user.Email ?? string.Empty,
                    user.PhoneNumber,
                    user.EmailConfirmed,
                    Role = role
                });
            }

            return this.Ok(result);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserRequest model)
        {
            var updated = await _userManager.UpdateUserAsync(id, model.FullName, model.Email, model.PhoneNumber);
            return updated
                ? this.Ok(new { Success = true, Message = "User updated successfully." })
                : this.BadRequest(new { Success = false, Message = "Failed to update user." });
        }

        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(string id, [FromBody] UpdateRoleRequest model)
        {
            var user = await _identityUserManager.FindByIdAsync(id);
            if (user is null)
                return this.NotFound(new { Success = false, Message = "User not found." });

            var currentRole = await _roleManager.GetUserRoleAsync(user.Email!);
            if (!string.IsNullOrEmpty(currentRole))
            {
                await _identityUserManager.RemoveFromRoleAsync(user, currentRole);
            }

            var addResult = await _identityUserManager.AddToRoleAsync(user, model.Role);
            return addResult.Succeeded
                ? this.Ok(new { Success = true, Message = "Role updated successfully." })
                : this.BadRequest(new { Success = false, Message = "Failed to update role." });
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _identityUserManager.FindByIdAsync(id);
            if (user is null)
                return this.NotFound(new { Success = false, Message = "User not found." });

            var result = await _identityUserManager.DeleteAsync(user);
            return result.Succeeded
                ? this.Ok(new { Success = true, Message = "User deleted successfully." })
                : this.BadRequest(new { Success = false, Message = "Failed to delete user." });
        }
    }

    public class UpdateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }
}
