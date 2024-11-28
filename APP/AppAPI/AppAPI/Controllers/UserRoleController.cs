using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserRoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("AssignRoles")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUserRoles(Guid userId, List<Guid> roleIds)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = $"User with ID {userId} does not exist.",
                    Success = false
                });
            }

            var existingUserRoles = await _context.UserRoles.Where(ur => ur.UserId == userId).ToListAsync();

            foreach (var roleId in roleIds)
            {
                var role = await _context.Roles.FindAsync(roleId);
                if (role == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Message = $"Role with ID {roleId} does not exist.",
                        Success = false
                    });
                }

                if (!existingUserRoles.Any(ur => ur.RoleId == roleId))
                {
                    _context.UserRoles.Add(new UserRole { RoleId = roleId, UserId = userId });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Message = "User roles updated successfully.",
                Success = true
            });
        }

        [HttpPost("AssignRole")]
        public async Task<ActionResult<ApiResponse<object>>> AddUserRole(Guid userId, Guid roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = $"User with ID {userId} does not exist.",
                    Success = false
                });
            }

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = $"Role with ID {roleId} does not exist.",
                    Success = false
                });
            }

            var userRoleExist = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.RoleId == roleId)
                .FirstOrDefaultAsync();

            if (userRoleExist == null)
            {
                _context.UserRoles.Add(new UserRole { RoleId = roleId, UserId = userId });
                await _context.SaveChangesAsync();
            }

            return Ok(new ApiResponse<object>
            {
                Message = "User role assigned successfully.",
                Success = true
            });
        }
    }
}


//using AppAPI.Data;
//using AppAPI.Models.Domain;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserRoleController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;

//        public UserRoleController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("AssignRoles")] //ok
//        public async Task<ActionResult> UpdateUserRoles(Guid userId, List<Guid> roleIds)
//        {
//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//            {
//                return NotFound($"User with ID {userId} does not exist.");
//            }

//            var existingUserRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();

//            foreach (var roleId in roleIds)
//            {
//                var role = await _context.Roles.FindAsync(roleId);
//                if (role == null)
//                {
//                    return BadRequest($"Role with ID {roleId} does not exist.");
//                }

//                var userRoleExist = existingUserRoles.SingleOrDefault(ur => ur.RoleId == roleId);
//                if (userRoleExist == null)
//                {
//                    _context.UserRoles.Add(new UserRole { RoleId = roleId, UserId = userId });
//                }
//            }

//            await _context.SaveChangesAsync();

//            return Ok(new { Message = "User roles updated successfully." });
//        }

//        [HttpPost("AssignRole")] //ok
//        public async Task<ActionResult> AddUserRole(Guid userId, Guid roleId)
//        {
//            var user = await _context.Users.FindAsync(userId);
//            if (user == null)
//            {
//                return NotFound($"User with ID {userId} does not exist.");
//            }

//            var existingUserRoles = _context.UserRoles.Where(ur => ur.UserId == userId).ToList();

//            var role = await _context.Roles.FindAsync(roleId);
//            if (role == null)
//            {
//                return BadRequest($"Role with ID {roleId} does not exist.");
//            }

//            var userRoleExist = existingUserRoles.SingleOrDefault(ur => ur.RoleId == roleId);
//            if (userRoleExist == null)
//            {
//                _context.UserRoles.Add(new UserRole { RoleId = roleId, UserId = userId });
//            }


//            await _context.SaveChangesAsync();

//            return Ok(new { Message = "User roles updated successfully." });
//        }

//    }


//}
