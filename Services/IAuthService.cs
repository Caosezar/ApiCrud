using ApiCrud.DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace ApiCrud.Services
{
    public interface IAuthService
    {
        Task<object?> RegisterAsync(RegisterRequestDTO dto);
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO dto);  
    }
}
