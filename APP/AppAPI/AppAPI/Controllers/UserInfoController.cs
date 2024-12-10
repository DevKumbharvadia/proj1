using AppAPI.Data;
using AppAPI.Models.ResponseModel;
using AppAPI.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllUserInfo")]
        public async Task<ActionResult<ApiResponse<List<UserInfoView>>>> GetAllUsersInfo()
        {
            try
            {
                var users = await _context.Users
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

                if (!users.Any())
                {
                    return NotFound(new ApiResponse<List<UserInfoView>>
                    {
                        Message = "No users found.",
                        Success = false
                    });
                }

                return Ok(new ApiResponse<List<UserInfoView>>
                {
                    Message = "Users information retrieved successfully",
                    Success = true,
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<UserInfoView>>
                {
                    Message = $"An error occurred while retrieving user info: {ex.Message}",
                    Success = false
                });
            }
        }

        [HttpGet("GetUserInfoById")]
        public async Task<ActionResult<ApiResponse<UserInfoView>>> GetUserInfoById(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserId == id)
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
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new ApiResponse<UserInfoView>
                    {
                        Message = "User not found.",
                        Success = false
                    });
                }

                return Ok(new ApiResponse<UserInfoView>
                {
                    Message = "User information retrieved successfully",
                    Success = true,
                    Data = user
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserInfoView>
                {
                    Message = $"An error occurred while retrieving user info: {ex.Message}",
                    Success = false
                });
            }
        }
    }
}

