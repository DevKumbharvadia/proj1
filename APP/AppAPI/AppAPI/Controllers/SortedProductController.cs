using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortedProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public SortedProductController(ApplicationDbContext context, SieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        [HttpGet("GetSortedProduct")]
        public async Task<IActionResult> GetSortedProduct([FromQuery] SieveModel model)
        {
            IQueryable<Product> productQuery;

            // Default to no filters if none are provided
            if (model.Filters == null)
            {
                productQuery = _context.Products
                    .Include(p => p.Seller)
                    .Where(p => !_context.DeletedProducts.Any(dp => dp.ProductId == p.ProductId));
            }
            else
            {
                productQuery = _context.Products
                    .Include(p => p.Seller)
                    .Where(p => p.ProductName.Contains(model.Filters))
                    .Where(p => !_context.DeletedProducts.Any(dp => dp.ProductId == p.ProductId));
            }

            productQuery = _sieveProcessor.Apply(model, productQuery);

            // Set default page and pageSize if not provided
            model.Page ??= 1;
            model.PageSize ??= 5;

            int totalCount = model.Sorts == null && model.Filters == null
                ? await _context.Products.CountAsync()
                : await productQuery.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / model.PageSize.Value);
            int page = model.Page.Value;
            int pageSize = model.PageSize.Value;

            var products = await productQuery
                .OrderBy(p => p.ProductId)
                .Skip((page - 1) * pageSize) // Correct skip calculation
                .Take(pageSize)
                .ToListAsync();

            if (!products.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Message = "No products found",
                    Success = true,
                    Data = null
                });
            }

            var result = products.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.Description,
                p.ImageContent,
                p.Price,
                p.StockQuantity,
                p.CreatedAt,
                p.UpdatedAt,
                Seller = new
                {
                    p.Seller.UserId,
                    p.Seller.Username,
                    p.Seller.Email
                }
            });

            return Ok(new ApiResponse<object>
            {
                Message = "Products retrieved successfully",
                Success = true,
                Data = new
                {
                    result,
                    totalPages,
                    currentPage = page
                }
            });
        }

        [HttpGet("getSortedProductsBySellerId")]
        public async Task<IActionResult> GetSortedProductsBySellerId([FromQuery] SieveModel model, Guid sellerId)
        {
            var productQuery = _context.Products
                .Include(p => p.Seller)
                .Where(p => p.SellerId == sellerId)
                .Where(p => !_context.DeletedProducts.Any(dp => dp.ProductId == p.ProductId))
                .AsQueryable();

            productQuery = _sieveProcessor.Apply(model, productQuery);

            // Set default page and pageSize if not provided
            model.Page ??= 1;
            model.PageSize ??= 5;

            int totalCount = model.Sorts == null && model.Filters == null
                ? await _context.Products.CountAsync()
                : await productQuery.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / model.PageSize.Value);
            int page = model.Page.Value;
            int pageSize = model.PageSize.Value;

            var products = await productQuery
                .OrderBy(p => p.ProductId)
                .Skip((page - 1) * pageSize) // Correct skip calculation
                .Take(pageSize)
                .ToListAsync();

            if (!products.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No products found for this seller",
                    Data = null
                });
            }

            var result = products.Select(p => new
            {
                p.ProductId,
                p.ProductName,
                p.Description,
                p.ImageContent,
                p.Price,
                p.StockQuantity,
                p.CreatedAt,
                p.UpdatedAt,
                Seller = new
                {
                    p.Seller.UserId,
                    p.Seller.Username,
                    p.Seller.Email
                }
            });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = new
                {
                    result,
                    totalPages,
                    currentPage = page
                }
            });
        }
    }
}


//using AppAPI.Data;
//using AppAPI.Models.Domain;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Sieve.Models;
//using Sieve.Services;
//using System;
//using TodoAPI.Models;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SortedProductController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _environment;
//        private readonly SieveProcessor _sieveProcessor;

//        public SortedProductController(ApplicationDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
//        {
//            _context = context;
//            _environment = environment;
//            _sieveProcessor = sieveProcessor;
//        }

//        [HttpGet("GetSortedProduct")] //ok
//        public async Task<IActionResult> GetSortedProduct([FromQuery] SieveModel model)
//        {
//            IQueryable<Product> productQuery;

//            if (model.Filters == null)
//            {
//                productQuery = _context.Products
//                    .Include(p => p.Seller)
//                    .AsQueryable()
//                    .Where(p => !_context.DeletedProducts
//                        .Any(dp => dp.ProductId == p.ProductId));
//            }
//            else
//            {
//                productQuery = _context.Products
//                    .Include(p => p.Seller)
//                    .Where(p => p.ProductName.Contains(model.Filters))
//                    .Where(p => !_context.DeletedProducts
//                        .Any(dp => dp.ProductId == p.ProductId))
//                    .AsQueryable();
//            }

//            productQuery = _sieveProcessor.Apply(model, productQuery);

//            if (model.Page == null || model.PageSize == null)
//            {
//                model.Page = 1;
//                model.PageSize = 5;
//            }

//            int totalCount = model.Sorts == null && model.Filters == null
//                ? await _context.Products.CountAsync()
//                : await productQuery.CountAsync();

//            var totalPages = (int)Math.Ceiling((double)totalCount / model.PageSize.Value);

//            int page = model.Page.HasValue ? model.Page.Value : 1;
//            int pageSize = model.PageSize.HasValue ? model.PageSize.Value : 10;

//            var products = await productQuery
//                .OrderBy(p => p.ProductId)
//                .Skip(0)
//                .Take(pageSize)
//                .ToListAsync();


//            if (products == null || !products.Any())
//            {
//                return Ok(new ApiResponse<object>
//                {
//                    Message = "no product found",
//                    Success = true,
//                });
//            }

//            var result = products.Select(p => new
//            {
//                p.ProductId,
//                p.ProductName,
//                p.Description,
//                p.ImageContent,
//                p.Price,
//                p.StockQuantity,
//                p.CreatedAt,
//                p.UpdatedAt,
//                Seller = new
//                {
//                    p.Seller.UserId,
//                    p.Seller.Username,
//                    p.Seller.Email
//                }
//            });

//            return Ok(new ApiResponse<object>
//            {
//                Message = "Products retrieved successfully",
//                Success = true,
//                Data = new
//                {
//                    result,
//                    totalPages = totalPages,
//                    currentPage = page
//                }
//            });
//        }

//        [HttpGet("getSortedProductsBySellerId")] //ok
//        public async Task<IActionResult> GetSortedProductsBySellerId([FromQuery] SieveModel model, Guid SellerId)
//        {
//            var productQuery = _context.Products
//                .Include(p => p.Seller)
//                .Where(p => p.SellerId == SellerId)
//                .Where(p => !_context.DeletedProducts
//                        .Any(dp => dp.ProductId == p.ProductId))
//                .AsQueryable();

//            productQuery = _sieveProcessor.Apply(model, productQuery);

//            if (model.Page == null || model.PageSize == null)
//            {
//                model.Page = 1;
//                model.PageSize = 5;
//            }

//            int totalCount = model.Sorts == null && model.Filters == null
//                ? await _context.Products.CountAsync()
//                : await productQuery.CountAsync();

//            var totalPages = (int)Math.Ceiling((double)totalCount / model.PageSize.Value);

//            int page = model.Page.HasValue ? model.Page.Value : 1;
//            int pageSize = model.PageSize.HasValue ? model.PageSize.Value : 10;

//            var products = await productQuery
//                .OrderBy(p => p.ProductId)
//                .Skip(0)
//                .Take(pageSize)
//                .ToListAsync();


//            if (products == null || !products.Any())
//            {
//                return NotFound(new { message = "No Products found." });
//            }

//            var result = products.Select(p => new
//            {
//                p.ProductId,
//                p.ProductName,
//                p.Description,
//                p.ImageContent,
//                p.Price,
//                p.StockQuantity,
//                p.CreatedAt,
//                p.UpdatedAt,
//                Seller = new
//                {
//                    p.Seller.UserId,
//                    p.Seller.Username,
//                    p.Seller.Email
//                }
//            });

//            return Ok(new ApiResponse<object>
//            {
//                Message = "Products retrieved successfully",
//                Success = true,
//                Data = new
//                {
//                    result,
//                    totalPages = totalPages,
//                    currentPage = page
//                }
//            });
//        }

//    }
//}
