using AppAPI.Data;
using AppAPI.Models.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductStockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly SieveProcessor _sieveProcessor;

        public ProductStockController(ApplicationDbContext context, IWebHostEnvironment environment, SieveProcessor sieveProcessor)
        {
            _context = context;
            _environment = environment;
            _sieveProcessor = sieveProcessor;
        }

        // CRUD for Product

        // GET: api/Products
        [HttpGet("Get")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Seller)
                .Include(p => p.Transactions)
                .ToListAsync();
        }

        // GET: api/Products/{id}
        [HttpGet("GetById")]
        public async Task<ActionResult<Product>> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Seller)
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // POST: api/Products
        [HttpPost("Add")]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        // PUT: api/Products/{id}
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProduct(Guid id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            product.UpdatedAt = DateTime.UtcNow;
            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Products/{id}
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // CRUD for ProductStockLog

        // GET: api/Products/StockLogs
        [HttpGet("StockLogs")]
        public async Task<ActionResult<IEnumerable<ProductStockLog>>> GetProductStockLogs()
        {
            return await _context.ProductStockLogs
                .Include(sl => sl.Product)
                .ToListAsync();
        }

        // GET: api/Products/StockLogs/{id}
        [HttpGet("StockLogById")]
        public async Task<ActionResult<ProductStockLog>> GetProductStockLog(Guid id)
        {
            var stockLog = await _context.ProductStockLogs
                .Include(sl => sl.Product)
                .FirstOrDefaultAsync(sl => sl.StockLogId == id);

            if (stockLog == null)
            {
                return NotFound();
            }

            return stockLog;
        }

        // POST: api/Products/StockLogs
        [HttpPost("AddStockLogs")]
        public async Task<ActionResult<ProductStockLog>> CreateProductStockLog(ProductStockLog stockLog)
        {
            // Validate product exists
            var product = await _context.Products.FindAsync(stockLog.ProductId);
            if (product == null)
            {
                return NotFound(new { Message = "Product not found" });
            }

            // Update stock level and add stock log
            product.StockQuantity += stockLog.QuantityChanged;
            stockLog.NewStockLevel = product.StockQuantity;
            _context.ProductStockLogs.Add(stockLog);
            _context.Entry(product).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductStockLog), new { id = stockLog.StockLogId }, stockLog);
        }

        // Helper method to check if a product exists
        private bool ProductExists(Guid id)
        {
            return _context.Products.Any(p => p.ProductId == id);
        }
    }
}
