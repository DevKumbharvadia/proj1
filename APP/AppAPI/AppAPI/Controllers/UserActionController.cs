using AppAPI.Models.Domain;
using AppAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpGet("GetUserActionsById")]
        public async Task<IActionResult> GetUserActionsById(Guid Id)
        {
            var userAction = await _context.UserActions
                                            .Where(t => t.UserActionId == Id)
                                            .FirstOrDefaultAsync();

            if (userAction == null)
            {
                return NotFound(new ApiResponse<UserAction>
                {
                    Message = "User action not found.",
                    Success = false,
                    Data = null
                });
            }

            return Ok(new ApiResponse<UserAction>
            {
                Message = "User action retrieved successfully",
                Success = true,
                Data = userAction
            });
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
