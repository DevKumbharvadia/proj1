namespace AppAPI.Models.ViewModel
{
    public class UserInfoView
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public DateTime? LastLogoutTime { get; set; }
    }
}
