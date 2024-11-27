using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPI.Data;
using AppAPI.Models;
using AppAPI.Models.Domain;
using AppAPI.Models.DTO;
using TodoAPI.Models;

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

        [HttpGet("GetAllRoles")] //ok
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpGet("GetUserRolesByID")] //ok
        public ActionResult<ApiResponse<List<string>>> GetUserRoles(Guid userId)
        {
            try
            {
                var roles = _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.Role!.RoleName)
                    .ToList();

                if (!roles.Any())
                {
                    return NotFound(new ApiResponse<List<string>>
                    {
                        Success = false,
                        Message = "User has no roles assigned",
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
                });
            }
        }

        [HttpPost("AddRole")] //ok
        public async Task<ActionResult<ApiResponse<Role>>> AddRole(RoleDTO role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role), "The role object cannot be null.");
            }

            if (string.IsNullOrEmpty(role.RoleName))
            {
                throw new ArgumentException("Role name is required.", nameof(role.RoleName));
            }

            var roleNameLowerCase = role.RoleName.ToLower();

            if (await _context.Roles.AnyAsync(r => r.RoleName.ToLower() == roleNameLowerCase))
            {
                return BadRequest(new ApiResponse<Role>
                {
                    Message = "Role name already exists",
                    Success = false,
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
                Message = "Role added successfully",
                Success = true,
                Data = newRole
            });
        }

        [HttpPut("UpdateRole")] //ok
        public async Task<IActionResult> UpdateRole(Guid Id, RoleDTO role)
        {
            if (Id == Guid.Empty)
            {
                return BadRequest("Invalid role ID.");
            }

            if (role == null)
            {
                return BadRequest("Role data is required.");
            }

            if (string.IsNullOrEmpty(role.RoleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            var roleNameLowerCase = role.RoleName.ToLower();

            var existingRoleWithName = await _context.Roles
                .Where(r => r.RoleName.ToLower() == roleNameLowerCase && r.RoleId != Id) 
                .FirstOrDefaultAsync();

            if (existingRoleWithName != null)
            {
                return BadRequest("Role name already exists.");
            }

            var existingRole = await _context.Roles.FindAsync(Id);
            if (existingRole == null)
            {
                return NotFound("Role not found.");
            }

            existingRole.RoleName = roleNameLowerCase;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"An error occurred while updating the role: {ex.Message}");
            }

            return Ok(new { Message = "Role updated successfully.", Role = existingRole });
        }

        [HttpDelete("DeleteRole")] //ok
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound($"Role with ID {id} was not found.");
            }

            _context.Roles.Remove(role);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Failed to delete role: {ex.Message}");
            }

            return Ok($"Role with ID {id} has been deleted.");
        }

    }
}
