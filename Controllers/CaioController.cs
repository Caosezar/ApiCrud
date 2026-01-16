using ApiCrud.Models;
using ApiCrud.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;

namespace ApiCrud.Controllers
{
    [ApiController]
    [Route("caio/[controller]")]
    public class CaioController : ControllerBase
    {
        //interface do serviço
        private readonly ICaioService _service;

        public CaioController(ICaioService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Caio>>> GetAll()
        {
            try
            {
                var caio = await _service.GetAllCaiosAsync();
                return Ok(null);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                  new { message = "Erro ao buscar produtos", error = ex.Message });
            }
        }
    }
}
