using AppAPI.Models.Domain;
using AppAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppAPI.Models.ResponseModel;
using AppAPI.Models.RequestModel;
using AppAPI.Models.DTO;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserActionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserActionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetUserActions")]
        public async Task<IActionResult> GetUserActions()
        {
            var userActionList = await _context.UserActions.ToListAsync();

            if (userActionList == null || !userActionList.Any())
            {
                return Ok(new ApiResponse<List<UserAction>>
                {
                    Message = "No user actions found.",
                    Success = true,
                    Data = new List<UserAction>()
                });
            }

            return Ok(new ApiResponse<List<UserAction>>
            {
                Message = "User actions retrieved successfully",
                Success = true,
                Data = userActionList
            });
        }

        [HttpGet("GetUserActionsByAuditId")]
        public async Task<IActionResult> GetUserActionsById(Guid Id)
        {
            var userActions = await _context.UserActions
                                             .Where(t => t.UserAuditId == Id)
                                             .Select(t => new UserActionDTO
                                             {
                                                 UserActionId = t.UserActionId,
                                                 Action = t.Action,
                                                 TimeOfAction = t.TimeOfAction
                                             })
                                             .ToListAsync();

            if (!userActions.Any())
            {
                return NotFound(new ApiResponse<List<UserActionDTO>>
                {
                    Message = "No user actions found.",
                    Success = false,
                    Data = null
                });
            }

            return Ok(new ApiResponse<List<UserActionDTO>>
            {
                Message = "User actions retrieved successfully",
                Success = true,
                Data = userActions
            });
        }


        [HttpPost]
        public async Task<IActionResult> CreateUserAction([FromBody] UserActionRequest userAction)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid input data", Success = false });

            try
            {
                // Check if the referenced UserAudit exists
                var userAuditExists = await _context.UserAudits.AnyAsync(ua => ua.UserAuditId == userAction.UserAuditId);
                if (!userAuditExists)
                    return NotFound(new { Message = "Referenced UserAudit not found", Success = false });

                _context.UserActions.Add(new UserAction
                {
                    UserActionId = Guid.NewGuid(),
                    UserAuditId = userAction.UserAuditId,
                    TimeOfAction = DateTime.UtcNow,
                    Action = userAction.Action
                });
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<UserAction>
                {
                    Message = "User action created successfully",
                    Success = true,
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected exceptions
                return StatusCode(500, new { Message = $"Internal server error: {ex.Message}", Success = false });
            }
        }
    }
}


//using AppAPI.Models.Domain;
//using AppAPI.Data;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TodoAPI.Models;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserActionController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;

//        public UserActionController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("GetUserActions")]
//        public async Task<IActionResult> GetUserActions()
//        {

//            var userActionList = await _context.UserActions.ToListAsync();

//            return Ok(new ApiResponse<List<UserAction>>
//            {
//                Message = "User actions retrieved successfully",
//                Success = true,
//                Data = userActionList
//            });
//        }

//        [HttpGet("GetUserActionsById")]
//        public async Task<IActionResult> GetUserActionsById(Guid Id)
//        {

//            var userAction = await _context.UserActions
//                                            .Where(t => t.UserActionId == Id)
//                                            .FirstOrDefaultAsync();

//            return Ok(new ApiResponse<UserAction>
//            {
//                Message = "User action retrieved successfully",
//                Success = true,
//                Data = userAction
//            });

//        }

//    }
//}
