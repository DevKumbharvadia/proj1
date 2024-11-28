using AppAPI.Data;
using AppAPI.Models.DTO;
using AppAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoAPI.Models;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserAuditController> _logger;

        public UserAuditController(ApplicationDbContext context, ILogger<UserAuditController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetAllAudits")]
        public IActionResult GetUserAudits()
        {
            try
            {
                var audits = _context.UserAudits
                    .Include(a => a.User)
                    .Select(a => new AuditWithUserDTO
                    {
                        UserAuditId = a.UserAuditId,
                        UserId = a.UserId,
                        Username = a.User!.Username, // Ensure User is not null
                        LoginTime = a.LoginTime,
                        LogoutTime = a.LogoutTime
                    })
                    .ToList();

                if (!audits.Any())
                {
                    return NotFound(new ApiResponse<List<AuditWithUserDTO>>
                    {
                        Message = "No audits found.",
                        Success = true,
                        Data = new List<AuditWithUserDTO>()
                    });
                }

                return Ok(new ApiResponse<List<AuditWithUserDTO>>
                {
                    Message = "User audits retrieved successfully.",
                    Success = true,
                    Data = audits
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching audits: {ex.Message}", ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("GetAuditsByUserID")]
        public async Task<IActionResult> GetUserAuditsByUserId(Guid userId)
        {
            try
            {
                var audits = await _context.UserAudits
                    .Where(a => a.UserId == userId)
                    .Include(a => a.User)  // Ensure User data is included
                    .Select(a => new AuditWithUserDTO
                    {
                        UserAuditId = a.UserAuditId,
                        UserId = a.UserId,
                        Username = a.User!.Username, // Ensure User is not null
                        LoginTime = a.LoginTime,
                        LogoutTime = a.LogoutTime
                    })
                    .ToListAsync();

                if (!audits.Any())
                {
                    return NotFound(new ApiResponse<List<AuditWithUserDTO>>
                    {
                        Message = $"No audits found for user with ID: {userId}",
                        Success = false,
                        Data = new List<AuditWithUserDTO>()
                    });
                }

                return Ok(new ApiResponse<List<AuditWithUserDTO>>
                {
                    Message = "User audits retrieved successfully.",
                    Success = true,
                    Data = audits
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while fetching audits for user {userId}: {ex.Message}", ex);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}


//using AppAPI.Data;
//using AppAPI.Models.DTO;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserAuditController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;

//        public UserAuditController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("GetAllAudits")] //ok
//        public IActionResult GetUserAudits()
//        {
//            try
//            {
//                var audits = _context.UserAudits
//                    .Include(a => a.User)
//                    .Select(a => new AuditWithUserDTO
//                    {
//                        UserAuditId = a.UserAuditId,
//                        UserId = a.UserId,
//                        Username = a.User!.Username,
//                        LoginTime = a.LoginTime,
//                        LogoutTime = a.LogoutTime
//                    })
//                    .ToList();

//                if (!audits.Any())
//                {
//                    return NotFound("No audits found.");
//                }

//                return Ok(audits);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred: {ex.Message}");
//                return StatusCode(500, "Internal server error.");
//            }
//        }

//        [HttpGet("GetAuditsByUserID")] //ok
//        public async Task<IActionResult> GetUserAuditsByUserId(Guid userId)
//        {
//            try
//            {
//                var audits = await _context.UserAudits
//                    .Where(a => a.UserId == userId)
//                    .ToListAsync();

//                if (!audits.Any())
//                {
//                    return NotFound($"No audits found for user with ID: {userId}");
//                }

//                return Ok(audits);
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred: {ex.Message}");
//                return StatusCode(500, "Internal server error.");
//            }
//        }

//    }
//}
