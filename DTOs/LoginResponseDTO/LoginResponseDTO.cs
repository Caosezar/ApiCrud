namespace ApiCrud.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}
