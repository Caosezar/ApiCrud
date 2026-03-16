namespace ApiCrud.Models
{
    public class User
    {
        public int Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? Username { get; set; }

        public string? PasswordHash { get; set; }

        public byte? RoleId { get; set; }

        public DateTime? LastLogin { get; set; }

        public int? FailedLogininAttempts { get; set; }

        public DateTime? LockoutEnd { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}