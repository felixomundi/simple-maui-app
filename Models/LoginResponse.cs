namespace users.Models
{
    public class LoginResponse
    {
        public string? Name { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
        public string? Phone { get; set; }
    }

}