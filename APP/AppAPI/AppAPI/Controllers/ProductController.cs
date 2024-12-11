using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.DTO;
using AppAPI.Models.RequestModel;
using AppAPI.Models.ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;
using TodoAPI.Models;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly SieveProcessor _sieveProcessor;

        public ProductController(ApplicationDbContext context, SieveProcessor sieveProcessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProduct()
        {
            var products = await _context.Products
                .Include(p => p.Seller)
                .ToListAsync();

            if (products == null || !products.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No products found.",
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
                Message = "Products retrieved successfully.",
                Data = result
            });
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product retrieved successfully.",
                Data = product
            });
        }

        [HttpGet("GetProductDetailsById")]
        public async Task<IActionResult> GetProductDetailsById(Guid id)
        {
            var product = await _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new
                {
                    p.ProductId,
                    p.ProductName,
                    p.Description,
                    p.ImageContent,
                    p.Price,
                    p.StockQuantity,
                    p.SellerId,
                    p.CreatedAt,
                    p.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product retrieved successfully.",
                Data = product
            });
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductUploadDTO productDto)
        {
            if (productDto.ImageFile == null || productDto.ImageFile.Length == 0)
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "No file uploaded.",
                    Data = null
                });

            byte[] fileContent;
            using (var memoryStream = new MemoryStream())
            {
                await productDto.ImageFile.CopyToAsync(memoryStream);
                fileContent = memoryStream.ToArray();
            }

            var product = new Product
            {
                ProductName = productDto.ProductName,
                Price = productDto.Price,
                StockQuantity = productDto.StockQuantity,
                Description = productDto.Description,
                ImageContent = fileContent,
                SellerId = productDto.SellerId,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product added successfully.",
                Data = new { product.ProductId, product.ProductName }
            });
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductUpdateRequst productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });
            }

            // Update fields if they are provided
            if (!string.IsNullOrEmpty(productDto.ProductName))
            {
                product.ProductName = productDto.ProductName;
            }

            if (productDto.Price != null && productDto.Price != product.Price)
            {
                product.Price = productDto.Price;
            }

            if (productDto.StockQuantity != null && productDto.StockQuantity != product.StockQuantity)
            {
                product.StockQuantity = productDto.StockQuantity;
            }

            if (!string.IsNullOrEmpty(productDto.Description))
            {
                product.Description = productDto.Description;
            }

            product.UpdatedAt = DateTime.UtcNow;

            // Handle file upload only if a new file is provided
            if (productDto.File != null && productDto.File.Length > 0)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await productDto.File.CopyToAsync(memoryStream);
                        product.ImageContent = memoryStream.ToArray(); // Replace the old image with the new one
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Error processing the image file.",
                        Data = new { error = ex.Message }
                    });
                }
            }   

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Product updated successfully.",
                Data = product
            });
        }


        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Product not found.",
                    Data = null
                });
            }

            var deletedProduct = new DeletedProduct
            {
                Id = Guid.NewGuid(),
                ProductId = id
            };

            _context.DeletedProducts.Add(deletedProduct);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Product deleted successfully.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Internal server error, unable to delete product: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}


//using AppAPI.Data;
//using AppAPI.Models.Domain;
//using AppAPI.Models.DTO;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Sieve.Models;
//using Sieve.Services;

//using TodoAPI.Models;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProductController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IWebHostEnvironment _environment;
//        private readonly SieveProcessor _sieveProcessor;

//        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
//        {
//            _context = context;
//            _environment = environment;
//            _sieveProcessor = sieveProcessor;
//        }

//        [HttpGet("GetAllProducts")] //ok
//        public async Task<IActionResult> GetAllProduct()
//        {
//            var products = await _context.Products
//                .Include(p => p.Seller)
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

//            return Ok(result);
//        }

//        [HttpGet("GetProductById")] //ok
//        public async Task<IActionResult> GetProduct(Guid id)
//        {
//            var product = await _context.Products.FindAsync(id);

//            if (product == null)
//            {
//                return NotFound(new { message = "Product not found." });
//            }

//            return Ok(product);
//        }

//        [HttpPost("AddProduct")] //ok
//        public async Task<IActionResult> AddProduct([FromForm] ProductUploadDTO productDto)
//        {
//            if (productDto.ImageFile == null || productDto.ImageFile.Length == 0)
//                return BadRequest("No file uploaded.");

//            byte[] fileContent;
//            using (var memoryStream = new MemoryStream())
//            {
//                await productDto.ImageFile.CopyToAsync(memoryStream);
//                fileContent = memoryStream.ToArray();
//            }

//            var product = new Product
//            {
//                ProductName = productDto.ProductName,
//                Price = productDto.Price,
//                StockQuantity = productDto.StockQuantity,
//                Description = productDto.Description,
//                ImageContent = fileContent,
//                SellerId = productDto.SellerId,
//                CreatedAt = DateTime.UtcNow
//            };

//            await _context.Products.AddAsync(product);
//            await _context.SaveChangesAsync();

//            return Ok(new { product.ProductId, product.ProductName });
//        }

//        [HttpPut("UpdateProduct")] //ok
//        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductUpdateDTO productDto)
//        {
//            var product = await _context.Products.FindAsync(id);
//            if (product == null)
//                return NotFound(new { message = "Product not found." });

//            if (!string.IsNullOrEmpty(productDto.ProductName))
//            {
//                product.ProductName = productDto.ProductName;
//            }

//            if (productDto.Price != null && productDto.Price != product.Price)
//            {
//                product.Price = productDto.Price;
//            }

//            if (productDto.StockQuantity != null && productDto.StockQuantity != product.StockQuantity)
//            {
//                product.StockQuantity = productDto.StockQuantity;
//            }

//            if (!string.IsNullOrEmpty(productDto.Description))
//            {
//                product.Description = productDto.Description;
//            }

//            product.UpdatedAt = DateTime.UtcNow;

//            if (productDto.File != null && productDto.File.Length > 0)
//            {
//                try
//                {
//                    using (var memoryStream = new MemoryStream())
//                    {
//                        await productDto.File.CopyToAsync(memoryStream);
//                        product.ImageContent = memoryStream.ToArray();
//                    }
//                }
//                catch (Exception ex)
//                {
//                    return BadRequest(new { message = "Error processing the image file.", error = ex.Message });
//                }
//            }

//            await _context.SaveChangesAsync();

//            return Ok(new { message = "Product updated successfully.", product = product });
//        }

//        [HttpDelete("DeleteProduct")] //ok
//        public async Task<IActionResult> DeleteProduct(Guid id)
//        {

//            var deletedProduct = _context.DeletedProducts.Add(new DeletedProduct { Id = Guid.NewGuid(), ProductId = id });
//            try
//            {
//                ;
//                await _context.SaveChangesAsync();

//                return Ok(new { message = "Product deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error occurred while deleting product: {ex.Message}");
//                return StatusCode(500, new { message = $"Internal server error, unable to delete product {ex}" });
//            }
//        }
//    }
//}
