using ApiCrud.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            try
            {
                return Ok(null);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  new { message = "Erro ao buscar usuários", error = ex.Message });

            }

        }
    }
}