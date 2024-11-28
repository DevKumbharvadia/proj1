namespace AppAPI.Models.ResponseModel
{
    public class RefreshTokenResponse
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }

}
