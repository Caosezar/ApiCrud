using ApiCrud.DTOs;
using Microsoft.AspNetCore.Identity.Data;

namespace ApiCrud.Services
{
    public interface IAuthService
    {
        Task<object?> RegisterAsync(RegisterRequestDto dto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);  
    }
}
