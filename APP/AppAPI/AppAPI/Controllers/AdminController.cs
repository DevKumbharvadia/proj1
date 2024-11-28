using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.ResponseModel;
using AppAPI.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("RewriteRoles")]
        public async Task<ActionResult<ApiResponse<UserAction>>> RewriteRoles(Guid userId, List<Guid> roleIds)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<UserAction>
                {
                    Message = $"User with ID {userId} does not exist.",
                    Success = false,
                    Data = null
                });
            }

            var existingUserRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();

            foreach (var userRole in existingUserRoles)
            {
                if (!roleIds.Contains(userRole.RoleId))
                {
                    _context.UserRoles.Remove(userRole);
                }
            }

            foreach (var roleId in roleIds)
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role == null)
                {
                    return BadRequest(new ApiResponse<UserAction>
                    {
                        Message = $"Role with ID {roleId} does not exist.",
                        Success = false,
                        Data = null
                    });
                }

                var userRoleExist = existingUserRoles.SingleOrDefault(ur => ur.RoleId == roleId);
                if (userRoleExist == null)
                {
                    _context.UserRoles.Add(new UserRole { RoleId = roleId, UserId = userId });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserAction>
            {
                Message = "User roles updated successfully.",
                Success = true,
                Data = null  // No specific data to return
            });
        }

        [HttpPost("BlacklistUser")]
        public async Task<ActionResult<ApiResponse<UserAction>>> BlacklistUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<UserAction>
                {
                    Message = $"User with ID {id} not found.",
                    Success = false,
                    Data = null
                });
            }

            _context.BlacklistedUsers.Add(new BlacklistedUser { Id = Guid.NewGuid(), UserId = id });

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<UserAction>
            {
                Message = "User blacklisted successfully.",
                Success = true,
                Data = null  // No specific data to return
            });
        }

        [HttpGet("GetAllWhiteListedUserInfo")]
        public async Task<ActionResult<ApiResponse<List<UserInfoView>>>> GetAllUsersInfo()
        {
            var users = await _context.Users
                .Where(user => !_context.BlacklistedUsers.Any(blacklisted => blacklisted.UserId == user.UserId))
                .Select(user => new UserInfoView
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    LastLoginTime = user.UserAudits
                        .OrderByDescending(audit => audit.LoginTime)
                        .Select(audit => audit.LoginTime)
                        .FirstOrDefault(),
                    LastLogoutTime = user.UserAudits
                        .OrderByDescending(audit => audit.LoginTime)
                        .Select(audit => audit.LogoutTime)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<UserInfoView>>
            {
                Message = "White-listed user info retrieved successfully.",
                Success = true,
                Data = users
            });
        }
    }
}
