using ApiCrud.DTOs;
using ApiCrud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService service)
        {
            _authService = service;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO dto)
        {
            if (!ModelState.IsValid)

                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);
            if (result == null)

                return BadRequest("Username já existe.");
            return StatusCode(201, result);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized("Credenciais inválidas.");
            return Ok(result);
        }
    }
}
