using System;
using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models.RequestModel
{
    public class UserRegisteRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
