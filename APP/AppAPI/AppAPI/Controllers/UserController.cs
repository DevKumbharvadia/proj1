using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppAPI.Data;
using AppAPI.Models;
using AppAPI.Models.Domain;
using AppAPI.Models.DTO;
using AppAPI.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using AppAPI.Models.ResponseModel;
using AppAPI.Models.RequestModel;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsUserValid(UserRegisteRequest model)
        {
            if (_context.Users.Any(u => u.Username == model.Username))
            {
                throw new ArgumentException("Username already exists");
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                throw new ArgumentException("Email already exists");
            }

            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email))
            {
                throw new ArgumentException("All fields (Username, Email, Password) are required.");
            }

            return true;
        }

        [HttpPost("AddUser")]
        public ActionResult<ApiResponse<User>> AddUser([FromBody] UserRegisteRequest model)
        {
            try
            {
                IsUserValid(model);

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = model.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Email = model.Email
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return CreatedAtAction(nameof(AddUser), new { id = user.UserId }, new ApiResponse<User>
                {
                    Message = "User registered successfully",
                    Success = true,
                    Data = user
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<User>
                {
                    Message = ex.Message,
                    Success = false,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<User>
                {
                    Message = $"Error registering user: {ex.Message}",
                    Success = false,
                });
            }
        }

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(new ApiResponse<List<User>>
            {
                Message = "Users retrieved successfully",
                Success = true,
                Data = users
            });
        }

        [HttpGet("GetUserByID")]
        public async Task<ActionResult<ApiResponse<User>>> GetUserByID(Guid Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound(new ApiResponse<User>
                {
                    Message = "User not found",
                    Success = false,
                });
            }

            return Ok(new ApiResponse<User>
            {
                Message = "User retrieved successfully",
                Success = true,
                Data = user
            });
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<ApiResponse<User>>> UpdateUser(Guid Id, [FromBody] UpdateUserDTO model)
        {
            if (model == null)
            {
                return BadRequest(new ApiResponse<User>
                {
                    Message = "The update model cannot be null.",
                    Success = false,
                });
            }

            try
            {
                var user = await _context.Users.FindAsync(Id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<User>
                    {
                        Message = "User not found.",
                        Success = false,
                    });
                }

                // Check if username and email are unique, excluding the current user
                if (_context.Users.Any(u => u.Username == model.Username && u.UserId != Id))
                {
                    return BadRequest(new ApiResponse<User>
                    {
                        Message = "Username already exists.",
                        Success = false,
                    });
                }

                if (_context.Users.Any(u => u.Email == model.Email && u.UserId != Id))
                {
                    return BadRequest(new ApiResponse<User>
                    {
                        Message = "Email already exists.",
                        Success = false,
                    });
                }

                // Update user data
                user.Username = model.Username;
                user.Email = model.Email;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<User>
                {
                    Data = user,
                    Message = "User updated successfully.",
                    Success = true,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<User>
                {
                    Message = $"An error occurred while updating the user: {ex.Message}",
                    Success = false,
                });
            }
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<ApiResponse<User>>> DeleteUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<User>
                {
                    Message = "User not found.",
                    Success = false,
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<User>
            {
                Message = "User deleted successfully",
                Success = true,
                Data = user
            });
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AppAPI.Data;
//using AppAPI.Models;
//using AppAPI.Models.Domain;
//using AppAPI.Models.DTO;
//using TodoAPI.Models;
//using AppAPI.Models.Interface;
//using Microsoft.AspNetCore.Authorization;

//namespace TodoAPI.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserController : ControllerBase //ok
//    {
//        private readonly ApplicationDbContext _context;

//        public UserController(ApplicationDbContext context)
//        {
//            _context = context;
//        }


//        [HttpPost("AddUser")] //ok
//        public ActionResult<ApiResponse<User>> AddUser([FromBody] UserRegisterModelDTO model)
//        {
//            if (_context.Users.Any(u => u.Username == model.Username))
//            {
//                return BadRequest(new ApiResponse<User>
//                {
//                    Message = "Username already exists",
//                    Success = false,
//                });
//            }

//            if (_context.Users.Any(u => u.Email == model.Email))
//            {
//                return BadRequest(new ApiResponse<User>
//                {
//                    Message = "Email already exists",
//                    Success = false,
//                });
//            }

//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model), "The registration model cannot be null.");
//            }

//            if (string.IsNullOrEmpty(model.Username))
//            {
//                throw new ArgumentException("Username is required.", nameof(model.Username));
//            }

//            if (string.IsNullOrEmpty(model.Password))
//            {
//                throw new ArgumentException("Password is required.", nameof(model.Password));
//            }

//            if (string.IsNullOrEmpty(model.Email))
//            {
//                throw new ArgumentException("Email is required.", nameof(model.Email));
//            }

//            var user = new User
//            {
//                UserId = Guid.NewGuid(),
//                Username = model.Username,
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
//                Email = model.Email
//            };

//            _context.Users.Add(user);

//            try
//            {
//                _context.SaveChanges();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<User>
//                {
//                    Message = $"Error registering user: {ex.Message}",
//                    Success = false,
//                });
//            }

//            return CreatedAtAction(nameof(AddUser), new { id = user.UserId }, new ApiResponse<User>
//            {
//                Message = "User registered successfully",
//                Success = true,
//                Data = user
//            });
//        }

//        [HttpGet("GetAllUsers")] //ok
//        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
//        {
//            var users = await _context.Users.ToListAsync();
//            return Ok(users);
//        }

//        [HttpGet("GetUserByID")] //ok
//        public async Task<ActionResult<IEnumerable<User>>> GetUserByID(Guid Id)
//        {
//            var users = await _context.Users.FindAsync(Id);
//            return Ok(users);
//        }

//        [HttpPut("UpdateUser")] //ok
//        public async Task<ActionResult<IEnumerable<User>>> UpdateUser(Guid Id, UpdateUserDTO model)
//        {
//            if (model == null)
//            {
//                throw new ArgumentNullException(nameof(model), "The update model cannot be null.");
//            }

//            if (string.IsNullOrEmpty(model.Username))
//            {
//                return BadRequest(new ApiResponse<User>
//                {
//                    Message = "Username is required.",
//                    Success = false,
//                });
//            }

//            if (string.IsNullOrEmpty(model.Password))
//            {
//                return BadRequest(new ApiResponse<User>
//                {
//                    Message = "Password is required.",
//                    Success = false,
//                });
//            }

//            if (string.IsNullOrEmpty(model.Email))
//            {
//                return BadRequest(new ApiResponse<User>
//                {
//                    Message = "Email is required.",
//                    Success = false,
//                });
//            }

//            var user = await _context.Users.FindAsync(Id);
//            if (user == null)
//            {
//                return NotFound(new ApiResponse<User>
//                {
//                    Message = "User not found.",
//                    Success = false,
//                });
//            }

//            if (_context.Users.Any(u => u.Username == model.Username && u.UserId != Id))
//            {
//                return BadRequest(new ApiResponse<User>
//                {
//                    Message = "Username already exists.",
//                    Success = false,
//                });
//            }

//            user.Username = model.Username;
//            user.Email = model.Email;
//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

//            try
//            {
//                _context.Users.Update(user);
//                await _context.SaveChangesAsync();

//                return Ok(new ApiResponse<User>
//                {
//                    Data = user,
//                    Message = "User updated successfully.",
//                    Success = true,
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new ApiResponse<User>
//                {
//                    Message = $"An error occurred while updating the user: {ex.Message}",
//                    Success = false,
//                });
//            }
//        }

//        [HttpDelete("DeleteUser")] //ok
//        public async Task<ActionResult<User>> DeleteUser(Guid id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//            {
//                return NotFound($"User with ID {id} not found.");
//            }

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();
//            return Ok(user);
//        }
//    }
//}
