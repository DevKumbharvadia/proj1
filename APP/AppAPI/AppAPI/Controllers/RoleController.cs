using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.DTO;
using TodoAPI.Models; // Assuming ApiResponse<T> is here
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all roles
        [HttpGet("GetAllRoles")]
        public async Task<ActionResult<ApiResponse<List<Role>>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(new ApiResponse<List<Role>>
            {
                Success = true,
                Message = "Roles retrieved successfully",
                Data = roles
            });
        }

        // Get roles for a specific user by User ID
        [HttpGet("GetUserRolesByUserID")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetUserRolesByUserID(Guid userId)
        {
            try
            {
                var roles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.Role!.RoleName)
                    .ToListAsync();

                if (!roles.Any())
                {
                    return Ok(new ApiResponse<List<string>>
                    {
                        Success = false,
                        Message = "User has no roles assigned",
                        Data = roles
                    });
                }

                return Ok(new ApiResponse<List<string>>
                {
                    Success = true,
                    Message = "Roles retrieved successfully",
                    Data = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<string>>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        // Add a new role
        [HttpPost("AddRole")]
        public async Task<ActionResult<ApiResponse<Role>>> AddRole([FromBody] RoleDTO role)
        {
            if (role == null)
            {
                return BadRequest(new ApiResponse<Role>
                {
                    Success = false,
                    Message = "Role data is required.",
                    Data = null
                });
            }

            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest(new ApiResponse<Role>
                {
                    Success = false,
                    Message = "Role name is required.",
                    Data = null
                });
            }

            var roleNameLowerCase = role.RoleName.ToLower();

            if (await _context.Roles.AnyAsync(r => r.RoleName.ToLower() == roleNameLowerCase))
            {
                return BadRequest(new ApiResponse<Role>
                {
                    Success = false,
                    Message = "Role name already exists",
                    Data = null
                });
            }

            var newRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = roleNameLowerCase,
            };

            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Role>
            {
                Success = true,
                Message = "Role added successfully",
                Data = newRole
            });
        }

        // Update an existing role
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleDTO role)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid role ID",
                    Data = null
                });
            }

            if (role == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Role data is required",
                    Data = null
                });
            }

            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Role name cannot be empty",
                    Data = null
                });
            }

            var roleNameLowerCase = role.RoleName.ToLower();

            var existingRoleWithName = await _context.Roles
                .Where(r => r.RoleName.ToLower() == roleNameLowerCase && r.RoleId != id)
                .FirstOrDefaultAsync();

            if (existingRoleWithName != null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Role name already exists",
                    Data = null
                });
            }

            var existingRole = await _context.Roles.FindAsync(id);
            if (existingRole == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Role not found",
                    Data = null
                });
            }

            existingRole.RoleName = roleNameLowerCase;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"An error occurred while updating the role: {ex.Message}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Role updated successfully",
                Data = existingRole
            });
        }

        // Delete a role
        [HttpDelete("DeleteRole")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Role with ID {id} was not found.",
                    Data = null
                });
            }

            _context.Roles.Remove(role);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to delete role: {ex.Message}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Role with ID {id} has been deleted.",
                Data = null
            });
        }
    }
}



//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using AppAPI.Data;
//using AppAPI.Models;
//using AppAPI.Models.Domain;
//using AppAPI.Models.DTO;
//using TodoAPI.Models;

//namespace TodoAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RoleController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;

//        public RoleController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("GetAllRoles")] //ok
//        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
//        {
//            var roles = await _context.Roles.ToListAsync();
//            return Ok(roles);
//        }

//        [HttpGet("GetUserRolesByUserID")] //ok
//        public ActionResult<ApiResponse<List<string>>> GetUserRolesByUserID(Guid userId)
//        {
//            try
//            {
//                var roles = _context.UserRoles
//                    .Where(ur => ur.UserId == userId)
//                    .Select(ur => ur.Role!.RoleName)
//                    .ToList();

//                if (!roles.Any())
//                {
//                    return NotFound(new ApiResponse<List<string>>
//                    {
//                        Success = false,
//                        Message = "User has no roles assigned",
//                    });
//                }

//                return Ok(new ApiResponse<List<string>>
//                {
//                    Success = true,
//                    Message = "Roles retrieved successfully",
//                    Data = roles
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<List<string>>
//                {
//                    Success = false,
//                    Message = ex.Message,
//                });
//            }
//        }

//        [HttpPost("AddRole")] //ok
//        public async Task<ActionResult<ApiResponse<Role>>> AddRole(RoleDTO role)
//        {
//            if (role == null)
//            {
//                throw new ArgumentNullException(nameof(role), "The role object cannot be null.");
//            }

//            if (string.IsNullOrEmpty(role.RoleName))
//            {
//                throw new ArgumentException("Role name is required.", nameof(role.RoleName));
//            }

//            var roleNameLowerCase = role.RoleName.ToLower();

//            if (await _context.Roles.AnyAsync(r => r.RoleName.ToLower() == roleNameLowerCase))
//            {
//                return BadRequest(new ApiResponse<Role>
//                {
//                    Message = "Role name already exists",
//                    Success = false,
//                });
//            }

//            var newRole = new Role
//            {
//                RoleId = Guid.NewGuid(),
//                RoleName = roleNameLowerCase,
//            };

//            _context.Roles.Add(newRole);
//            await _context.SaveChangesAsync();

//            return Ok(new ApiResponse<Role>
//            {
//                Message = "Role added successfully",
//                Success = true,
//                Data = newRole
//            });
//        }

//        [HttpPut("UpdateRole")] //ok
//        public async Task<IActionResult> UpdateRole(Guid Id, RoleDTO role)
//        {
//            if (Id == Guid.Empty)
//            {
//                return BadRequest("Invalid role ID.");
//            }

//            if (role == null)
//            {
//                return BadRequest("Role data is required.");
//            }

//            if (string.IsNullOrEmpty(role.RoleName))
//            {
//                return BadRequest("Role name cannot be empty.");
//            }

//            var roleNameLowerCase = role.RoleName.ToLower();

//            var existingRoleWithName = await _context.Roles
//                .Where(r => r.RoleName.ToLower() == roleNameLowerCase && r.RoleId != Id) 
//                .FirstOrDefaultAsync();

//            if (existingRoleWithName != null)
//            {
//                return BadRequest("Role name already exists.");
//            }

//            var existingRole = await _context.Roles.FindAsync(Id);
//            if (existingRole == null)
//            {
//                return NotFound("Role not found.");
//            }

//            existingRole.RoleName = roleNameLowerCase;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateException ex)
//            {
//                return StatusCode(500, $"An error occurred while updating the role: {ex.Message}");
//            }

//            return Ok(new { Message = "Role updated successfully.", Role = existingRole });
//        }

//        [HttpDelete("DeleteRole")] //ok
//        public async Task<IActionResult> DeleteRole(Guid id)
//        {
//            var role = await _context.Roles.FindAsync(id);
//            if (role == null)
//            {
//                return NotFound($"Role with ID {id} was not found.");
//            }

//            _context.Roles.Remove(role);

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateException ex)
//            {
//                return BadRequest($"Failed to delete role: {ex.Message}");
//            }

//            return Ok($"Role with ID {id} has been deleted.");
//        }

//    }
//}
