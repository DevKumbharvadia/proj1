using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.RequestModel;
using AppAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyerInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BuyerInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetBuyerInfo")]
        public async Task<IActionResult> GetBuyerInfo()
        {
            var buyerInfo = await _context.BuyerInfos.ToListAsync();

            return Ok(new ApiResponse<List<BuyerInfo>>
            {
                Message = "Data retrieved successfully",
                Success = true,
                Data = buyerInfo,
            });
        }

        [HttpGet("BuyerInfoExist")]
        public async Task<IActionResult> BuyerInfoExist(Guid id)
        {
            // Check if the BuyerInfo exists for the given user ID
            bool exists = await _context.BuyerInfos.AnyAsync(b => b.UserId == id);

            bool data = exists;

            // Return response
            return Ok(new ApiResponse<bool>
            {
                Message = exists ? "Buyer info exists." : "Buyer info does not exist.",
                Success = true,
                Data = data
            });
        }


        [HttpPost("AddBuyerInfo")]
        public async Task<IActionResult> AddBuyerInfo([FromForm] BuyerInfoRequest buyerInfoRequest)
        {
            if (buyerInfoRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            var newBuyerInfo = new BuyerInfo
            {
                BuyerInfoId = Guid.NewGuid(),
                UserId = buyerInfoRequest.UserId,
                ContactNumber = buyerInfoRequest.ContactNumber,
                Address = buyerInfoRequest.Address
            };

            _context.BuyerInfos.Add(newBuyerInfo);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<BuyerInfo>
            {
                Message = "Buyer information added successfully.",
                Success = true,
                Data = newBuyerInfo
            });
        }

        [HttpPut("UpdateBuyerInfo")]
        public async Task<IActionResult> UpdateBuyerInfo(Guid UserId, [FromBody] BuyerInfoUpdateRequest buyerInfoRequest)
        {
            if (buyerInfoRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            var existingBuyerInfo = await _context.BuyerInfos.FirstOrDefaultAsync(b => b.UserId == UserId);

            if (existingBuyerInfo == null)
            {
                return NotFound($"BuyerInfo with ID {UserId} not found.");
            }

            existingBuyerInfo.ContactNumber = buyerInfoRequest.ContactNumber;
            existingBuyerInfo.Address = buyerInfoRequest.Address;

            _context.BuyerInfos.Update(existingBuyerInfo);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<BuyerInfo>
            {
                Message = "Buyer information updated successfully.",
                Success = true,
                Data = existingBuyerInfo
            });
        }

    }
}
