using AppAPI.Data;
using AppAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly SieveProcessor _sieveProcessor;

        public SellerController(ApplicationDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
        {
            _context = context;
            _environment = environment;
            _sieveProcessor = sieveProcessor;
        }

        // Get sales data for all sellers
        [HttpGet("AllSalesData")]
        public async Task<IActionResult> GetSalesForAllSellers()
        {
            var salesData = await _context.Transactions
                .Include(th => th.Product)
                .GroupBy(th => th.Product.SellerId)
                .Select(group => new SalesForAllSellersDTO
                {
                    SellerId = group.Key,
                    TotalAmountSold = group.Sum(th => th.TotalAmount),
                    TotalProductsSold = group.Sum(th => th.Quantity),
                    ItemsSold = group
                        .GroupBy(th => new { th.ProductId, th.Product.ProductName })
                        .Select(innerGroup => new ItemSoldDTO
                        {
                            ProductId = innerGroup.Key.ProductId,
                            ProductName = innerGroup.Key.ProductName,
                            TotalQuantitySold = innerGroup.Sum(th => th.Quantity),
                            TotalAmountSold = innerGroup.Sum(th => th.TotalAmount)
                        })
                        .ToList()
                })
                .ToListAsync();

            if (!salesData.Any())
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No sales data found.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Sales data retrieved successfully",
                Data = salesData
            });
        }

        // Get sales data for a specific seller by seller ID
        [HttpGet("SalesDataByID")]
        public async Task<IActionResult> GetSalesBySeller(Guid sellerId)
        {
            var salesData = await _context.Users
                .Where(u => u.UserId == sellerId)
                .Select(user => new SalesBySellerDTO
                {
                    SellerId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,

                    TotalAmountSold = user.Products
                        .SelectMany(p => p.Transactions)
                        .Sum(th => (double?)th.TotalAmount) ?? 0,

                    TotalProductsSold = user.Products
                        .SelectMany(p => p.Transactions)
                        .Sum(th => (int?)th.Quantity) ?? 0,

                    ItemsSold = _context.Transactions
                        .Where(th => th.SellerId == sellerId)
                        .GroupBy(th => new
                        {
                            th.ProductId,
                            th.Product.ProductName,
                            Price = th.TotalAmount / th.Quantity
                        })
                        .Select(group => new ItemSoldDTO
                        {
                            ProductId = group.Key.ProductId,
                            ProductName = group.Key.ProductName,
                            Price = group.Key.Price,
                            TotalQuantitySold = group.Sum(th => th.Quantity),
                            TotalAmountSold = group.Sum(th => th.TotalAmount)
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (salesData == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No sales data found for the given seller.",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Sales data retrieved successfully",
                Data = salesData
            });
        }
    }
}


//using AppAPI.Data;
//using AppAPI.Models.DTOs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Sieve.Services;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SellerController : ControllerBase // ok
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _environment;
//        private readonly SieveProcessor _sieveProcessor;

//        public SellerController(ApplicationDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
//        {
//            _context = context;
//            _environment = environment;
//            _sieveProcessor = sieveProcessor;
//        }

//        [HttpGet("AllSalesData")] //ok
//        public async Task<IActionResult> GetSalesForAllSellers()
//        {
//            var salesData = await _context.Transactions
//                .Include(th => th.Product)
//                .GroupBy(th => th.Product.SellerId)
//                .Select(group => new SalesForAllSellersDTO
//                {
//                    SellerId = group.Key,
//                    TotalAmountSold = group.Sum(th => th.TotalAmount),
//                    TotalProductsSold = group.Sum(th => th.Quantity),
//                    ItemsSold = group
//                        .GroupBy(th => new { th.ProductId, th.Product.ProductName })
//                        .Select(innerGroup => new ItemSoldDTO
//                        {
//                            ProductId = innerGroup.Key.ProductId,
//                            ProductName = innerGroup.Key.ProductName,
//                            TotalQuantitySold = innerGroup.Sum(th => th.Quantity),
//                            TotalAmountSold = innerGroup.Sum(th => th.TotalAmount)
//                        })
//                        .ToList()
//                })
//                .ToListAsync();

//            if (!salesData.Any())
//                return NotFound(new { message = "No sales data found." });

//            return Ok(new
//            {
//                message = "Sales data retrieved successfully",
//                data = salesData
//            });
//        }

//        [HttpGet("SalesDataByID")] //ok
//        public async Task<IActionResult> GetSalesBySeller(Guid sellerId)
//        {
//            var salesData = await _context.Users
//                .Where(u => u.UserId == sellerId)
//                .Select(user => new SalesBySellerDTO
//                {
//                    SellerId = user.UserId,
//                    Username = user.Username,
//                    Email = user.Email,

//                    TotalAmountSold = user.Products
//                        .SelectMany(p => p.Transactions)
//                        .Sum(th => (double?)th.TotalAmount) ?? 0,

//                    TotalProductsSold = user.Products
//                        .SelectMany(p => p.Transactions)
//                        .Sum(th => (int?)th.Quantity) ?? 0,

//                    ItemsSold = _context.Transactions
//                        .Where(th => th.SellerId == sellerId)
//                        .GroupBy(th => new
//                        {
//                            th.ProductId,
//                            th.Product.ProductName,
//                            Price = th.TotalAmount / th.Quantity
//                        })
//                        .Select(group => new ItemSoldDTO
//                        {
//                            ProductId = group.Key.ProductId,
//                            ProductName = group.Key.ProductName,
//                            Price = group.Key.Price,
//                            TotalQuantitySold = group.Sum(th => th.Quantity),
//                            TotalAmountSold = group.Sum(th => th.TotalAmount)
//                        })
//                        .ToList()
//                })
//                .FirstOrDefaultAsync();

//            if (salesData == null)
//                return NotFound(new { message = "No sales data found for the given seller." });

//            return Ok(new
//            {
//                message = "Sales data retrieved successfully",
//                data = salesData
//            });
//        }


//    }
//}
