using System;
using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.DTO
{
    public class UserRegisterModelDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
