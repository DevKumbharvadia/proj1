﻿using AppAPI.Data;
using AppAPI.Models.Domain;
using AppAPI.Models.RequestModel;
using AppAPI.Models.ResponseModel;
using AppAPI.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
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
    public class TransactionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("getAllTransaction")]
        public async Task<IActionResult> GetAllTransaction()
        {
            var transactions = await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .ToListAsync();

            if (!transactions.Any())
            {
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "No transactions found.",
                    Data = null
                });
            }

            var transactionData = transactions.Select(t => new
            {
                t.TransactionId,
                t.ProductId,
                t.Quantity,
                t.TransactionDate,
                t.TotalAmount,
                t.ShipingStatus,
                ProductName = t.Product.ProductName,
                Buyer = new { t.Buyer.UserId, t.Buyer.Username },
                Seller = new { t.Seller.UserId, t.Seller.Username }
            });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Transactions retrieved successfully.",
                Data = transactionData
            });
        }

        [HttpGet("getTransactionById")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Product)
                .Include(t => t.Buyer)
                .Include(t => t.Seller)
                .FirstOrDefaultAsync(t => t.TransactionId == id);

            if (transaction == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Transaction not found.",
                    Data = null
                });
            }

            var transactionData = new
            {
                transaction.TransactionId,
                transaction.ProductId,
                transaction.Quantity,
                transaction.TransactionDate,
                transaction.TotalAmount,
                ProductName = transaction.Product.ProductName,
                Buyer = new { transaction.Buyer.UserId, transaction.Buyer.Username },
                Seller = new { transaction.Seller.UserId, transaction.Seller.Username }
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Transaction retrieved successfully.",
                Data = transactionData
            });
        }

        [HttpPost("MakePurchase")]
        public async Task<IActionResult> MakePurchase(PurchaseRequest purchase)
        {
            if (purchase == null)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Purchase details are required." });

            if (purchase.Quantity <= 0)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Quantity must be greater than zero." });

            if (purchase.ProductId == Guid.Empty || purchase.BuyerId == Guid.Empty)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid Product or Buyer ID." });

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == purchase.ProductId);

            if (product == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Product not found." });

            if (product.StockQuantity < purchase.Quantity)
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Not enough stock available." });

            double totalAmount = product.Price * purchase.Quantity;

            var buyer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == purchase.BuyerId);
            if (buyer == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Buyer not found." });

            var seller = await _context.Users.FirstOrDefaultAsync(u => u.UserId == product.SellerId);
            if (seller == null)
                return NotFound(new ApiResponse<object> { Success = false, Message = "Seller not found." });

            var transactionHistory = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                ProductId = purchase.ProductId,
                BuyerId = purchase.BuyerId,
                SellerId = product.SellerId,
                Quantity = purchase.Quantity,
                TotalAmount = totalAmount,
                TransactionDate = DateTime.UtcNow,
                ShipingStatus = false,
                Product = product,
                Buyer = buyer,
                Seller = seller
            };

            product.StockQuantity -= purchase.Quantity;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Transactions.Add(transactionHistory);
                _context.Products.Update(product);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = $"An error occurred while processing the purchase: {ex.Message}",
                    Data = null
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Purchase successful.",
                Data = new
                {
                    transactionHistory.TransactionId,
                    transactionHistory.TransactionDate,
                    Product = new { product.ProductId, product.ProductName, product.Price },
                    Buyer = new { buyer.UserId, buyer.Username },
                    Seller = new { seller.UserId, seller.Username },
                    transactionHistory.Quantity,
                    transactionHistory.TotalAmount
                }
            });
        }

        [HttpGet("getTransactionHistory")]
        public async Task<IActionResult> GetTransactionHistory()
        {
            var transactionHistory = await _context.Transactions
                .Select(t => new
                {
                    t.TransactionId,
                    t.ProductId,
                    t.Quantity,
                    t.TransactionDate,
                    t.TotalAmount,
                    t.ShipingStatus
                })
                .ToListAsync();

            var transactionHistoryWithProductDetails = new List<TransactionHistoryDTO>();

            foreach (var transaction in transactionHistory)
            {
                var product = await _context.Products
                    .Where(p => p.ProductId == transaction.ProductId)
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    transactionHistoryWithProductDetails.Add(new TransactionHistoryDTO
                    {
                        ProductName = product.ProductName,
                        Quantity = transaction.Quantity,
                        TransactionDate = transaction.TransactionDate,
                        TotalAmount = transaction.TotalAmount,
                        ShipingStatus = transaction.ShipingStatus
                    });
                }
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Transaction history retrieved successfully.",
                Data = transactionHistoryWithProductDetails
            });
        }

        [HttpGet("getTransactionHistoryByUserId")]
        public async Task<IActionResult> GetTransactionHistoryByUserId(Guid id)
        {
            var transactionHistory = await _context.Transactions
                .Where(t => t.BuyerId == id)
                .Select(t => new
                {
                    t.TransactionId,
                    t.ProductId,
                    t.Quantity,
                    t.TransactionDate,
                    t.TotalAmount
                })
                .ToListAsync();

            var transactionHistoryWithProductDetails = new List<TransactionHistoryDTO>();

            foreach (var transaction in transactionHistory)
            {
                var product = await _context.Products
                    .Where(p => p.ProductId == transaction.ProductId)
                    .FirstOrDefaultAsync();

                if (product != null)
                {
                    transactionHistoryWithProductDetails.Add(new TransactionHistoryDTO
                    {
                        ProductName = product.ProductName,
                        Quantity = transaction.Quantity,
                        TransactionDate = transaction.TransactionDate,
                        TotalAmount = transaction.TotalAmount
                    });
                }
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Transaction history for user retrieved successfully.",
                Data = transactionHistoryWithProductDetails
            });
        }
    }
}



//using AppAPI.Data;
//using AppAPI.Models.Domain;
//using AppAPI.Models.DTO;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Sieve.Services;

//namespace AppAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TransactionController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;

//        public TransactionController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet("getAllTransaction")] //ok
//        public async Task<IActionResult> getAllTransaction()
//        {
//            return Ok();
//        }

//        [HttpGet("getAllTransactionById")] //ok
//        public async Task<IActionResult> getTransactionById(Guid Id)
//        {
//            return Ok();
//        }

//        [HttpPost("MakePurchase")] //ok
//        public async Task<IActionResult> MakePurchase(PurchaseDTO purchase)
//        {
//            if (purchase == null)
//                return BadRequest("Purchase details are required.");

//            if (purchase.Quantity <= 0)
//                return BadRequest("Quantity must be greater than zero.");

//            if (purchase.ProductId == Guid.Empty || purchase.BuyerId == Guid.Empty)
//                return BadRequest("Invalid Product or Buyer ID.");

//            var product = await _context.Products
//                .FirstOrDefaultAsync(p => p.ProductId == purchase.ProductId);

//            if (product == null)
//                return NotFound("Product not found.");

//            if (product.StockQuantity < purchase.Quantity)
//                return BadRequest("Not enough stock available.");

//            double totalAmount = product.Price * purchase.Quantity;

//            var buyer = await _context.Users.FirstOrDefaultAsync(u => u.UserId == purchase.BuyerId);
//            if (buyer == null)
//                return NotFound("Buyer not found.");

//            var transactionHistory = new Transaction
//            {
//                TransactionId = Guid.NewGuid(),
//                ProductId = purchase.ProductId,
//                BuyerId = purchase.BuyerId,
//                SellerId = product.SellerId,
//                Quantity = purchase.Quantity,
//                TotalAmount = totalAmount,
//                TransactionDate = DateTime.UtcNow,
//                Product = product,
//                Buyer = buyer,  
//                Seller = await _context.Users.FirstOrDefaultAsync(u => u.UserId == product.SellerId) ?? throw new InvalidOperationException("Seller not found.")
//            };

//            product.StockQuantity -= purchase.Quantity;

//            using var transaction = await _context.Database.BeginTransactionAsync();
//            try
//            {
//                _context.Transactions.Add(transactionHistory);
//                _context.Products.Update(product);

//                await _context.SaveChangesAsync();
//                await transaction.CommitAsync();
//            }
//            catch (Exception ex)
//            {
//                await transaction.RollbackAsync();
//                return StatusCode(500, $"An error occurred while processing the purchase: {ex.Message}");
//            }

//            return Ok(new
//            {
//                Message = "Purchase successful.",
//                Transaction = new
//                {
//                    transactionHistory.TransactionId,
//                    transactionHistory.TransactionDate,
//                    Product = new { product.ProductId, product.ProductName, product.Price },
//                    Buyer = new { buyer.UserId, buyer.Username },
//                    Seller = new { transactionHistory.Seller.UserId, transactionHistory.Seller.Username },
//                    transactionHistory.Quantity,
//                    transactionHistory.TotalAmount
//                }
//            });
//        }

//        [HttpGet("getTransactionHistory")] //ok
//        public async Task<IActionResult> GetTransactionHistory()
//        {
//            var transactionHistory = await _context.Transactions
//                .Select(t => new
//                {
//                    t.TransactionId,
//                    t.ProductId,
//                    t.Quantity,
//                    t.TransactionDate,
//                    t.TotalAmount
//                })
//                .ToListAsync();

//            var transactionHistoryWithProductDetails = new List<TransactionHistoryDTO>();

//            foreach (var transaction in transactionHistory)
//            {
//                var product = await _context.Products
//                    .Where(p => p.ProductId == transaction.ProductId)
//                    .FirstOrDefaultAsync();

//                if (product != null)
//                {
//                    transactionHistoryWithProductDetails.Add(new TransactionHistoryDTO
//                    {
//                        ProductName = product.ProductName,
//                        Quantity = transaction.Quantity,
//                        TransactionDate = transaction.TransactionDate,
//                        TotalAmount = transaction.TotalAmount
//                    });
//                }
//            }

//            return Ok(transactionHistoryWithProductDetails);
//        }

//        [HttpGet("getTransactionHistoryByUserId")] //ok
//        public async Task<IActionResult> GetTransactionHistoryByUserId(Guid id)
//        {
//            var transactionHistory = await _context.Transactions
//                .Where(t => t.BuyerId == id)
//                .Select(t => new
//                {
//                    t.TransactionId,
//                    t.ProductId,
//                    t.Quantity,
//                    t.TransactionDate,
//                    t.TotalAmount
//                })
//                .ToListAsync();

//            var transactionHistoryWithProductDetails = new List<TransactionHistoryDTO>();

//            foreach (var transaction in transactionHistory)
//            {
//                var product = await _context.Products
//                    .Where(p => p.ProductId == transaction.ProductId)
//                    .FirstOrDefaultAsync();

//                if (product != null)
//                {
//                    transactionHistoryWithProductDetails.Add(new TransactionHistoryDTO
//                    {
//                        ProductName = product.ProductName,
//                        Quantity = transaction.Quantity,
//                        TransactionDate = transaction.TransactionDate,
//                        TotalAmount = transaction.TotalAmount
//                    });
//                }
//            }

//            return Ok(transactionHistoryWithProductDetails);
//        }


//    }
//}
