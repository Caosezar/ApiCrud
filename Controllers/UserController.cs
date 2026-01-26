using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    using ApiCrud.Models;
    using ApiCrud.Services;
    using Microsoft.AspNetCore.Mvc;
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = $"Usuário com Id {id} não encontardo" });
                }
                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error ao buscar usuário", error = ex.Message });
            }
        }
    }
}