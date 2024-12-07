﻿using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using TodoAPI.Models; // Assuming ApiResponse<T> is here

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public ProductStockController(ApplicationDbContext context, SieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        [HttpGet("GetAllStockLogs")]
        public async Task<IActionResult> GetAllStockUpdates()
        {

            var stockLogs = await _context.ProductStockLogs
                .Include(sl => sl.Product) // Assuming StockLogs has a reference to Product
                .ToListAsync();

            if (stockLogs == null || !stockLogs.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No stock logs found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock logs retrieved successfully.",
                Data = stockLogs
            });
        }

        [HttpGet("GetStockLogByProductId")]
        public async Task<IActionResult> GetStockUpdateById(Guid id)
        {
            var stockLog = await _context.ProductStockLogs
                .Include(sl => sl.Product) // Assuming StockLogs has a reference to Product
                .FirstOrDefaultAsync(sl => sl.ProductId == id);

            if (stockLog == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Stock log not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock log retrieved successfully.",
                Data = stockLog
            });
        }

        [HttpPost("AddProductStockLog")]
        public async Task<IActionResult> AddProductStockUpdate([FromBody] ProductStockAddRequest stockAddRequest)
        {
            if (stockAddRequest == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid stock update data.",
                    Data = null
                });
            }

            var product = await _context.Products.FindAsync(stockAddRequest.ProductId);
            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });
            }

            var stockLog = new ProductStockLog
            {
                ProductId = stockAddRequest.ProductId,
                QuantityChanged = stockAddRequest.QuantityChanged,
                NewStockLevel = product.StockQuantity + stockAddRequest.QuantityChanged, // Assuming you update the stock quantity based on the log
                Timestamp = DateTime.UtcNow
            };

            product.StockQuantity = stockLog.NewStockLevel;

            _context.ProductStockLogs.Add(stockLog);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock log added successfully.",
                Data = stockLog
            });
        }

        [HttpPut("UpdateProductStockLog")]
        public async Task<IActionResult> UpdateProductStockUpdate(Guid id, [FromBody] ProductStockAddRequest stockUpdateDto)
        {
            var stockLog = await _context.ProductStockLogs.FindAsync(id);
            if (stockLog == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Stock log not found.",
                    Data = null
                });
            }

            var product = await _context.Products.FindAsync(stockLog.ProductId);
            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });
            }

            // Updating the stock log and product stock quantity
            stockLog.QuantityChanged = stockUpdateDto.QuantityChanged;
            stockLog.NewStockLevel = product.StockQuantity + stockUpdateDto.QuantityChanged;
            stockLog.Timestamp = DateTime.UtcNow;

            product.StockQuantity = stockLog.NewStockLevel;

            _context.ProductStockLogs.Update(stockLog);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock log updated successfully.",
                Data = stockLog
            });
        }

        [HttpDelete("DeleteProductStockLog")]
        public async Task<IActionResult> DeleteProductStockUpdate(Guid id)
        {
            var stockLog = await _context.ProductStockLogs.FindAsync(id);
            if (stockLog == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Stock log not found.",
                    Data = null
                });
            }

            _context.ProductStockLogs.Remove(stockLog);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Stock log deleted successfully.",
                Data = null
            });
        }
    }
}


//using AppAPI.Data;
//using AppAPI.Models.Domain;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Sieve.Services;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductStockController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _environment;
//        private readonly SieveProcessor _sieveProcessor;

//        public ProductStockController(ApplicationDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
//        {
//            _context = context;
//            _environment = environment;
//            _sieveProcessor = sieveProcessor;
//        }

//        [HttpGet("GetAllStockLogs")]
//        public async Task<ActionResult<IEnumerable<Product>>> GetAllStockUpdates()
//        {
//            return Ok();
//        }

//        [HttpGet("GetStockLogById")]
//        public async Task<ActionResult<Product>> GetStockUpdateById(Guid id)
//        {
//            return Ok();
//        }

//        [HttpPost("AddProductStockLog")]
//        public async Task<ActionResult<Product>> AddProductStockUpdate(Product product)
//        {
//            return Ok();
//        }

//        //[HttpPut("UpdateProductStockLog")]
//        //public async Task<IActionResult> UpdateProductStockUpdate(Guid id, Product product)
//        //{
//        //    return Ok();
//        //}

//        //[HttpDelete("DeleteProductStockLog")]
//        //public async Task<IActionResult> DeleteProductStockUpdate(Guid id)
//        //{
//        //    return Ok();
//        //}
//    }
//}
