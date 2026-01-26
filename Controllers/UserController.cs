using ApiCrud.Models;
using ApiCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            try
            {
                var User = await _service.GetAllUsersAsync();
                return Ok(User);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  new { message = "Erro ao buscar usuários", error = ex.Message });

            }

        }
    }
}