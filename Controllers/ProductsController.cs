using ApiCrud.Models;
using ApiCrud.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiCrud.Controllers
{
    /// <summary>
    /// ProductsController - Camada de API (Controllers)
    /// 
    /// ℹ️ O QUE FAZ UM CONTROLLER?
    /// - Recebe requisições HTTP dos clientes
    /// - Converte dados HTTP em objetos .NET
    /// - Chama a Service com os dados
    /// - Converte resposta da Service em HTTP
    /// - Define os endpoints da API
    /// 
    /// ENDPOINTS DESTE CONTROLLER:
    /// - GET    /api/products        → lista todos
    /// - GET    /api/products/{id}   → lista um
    /// - POST   /api/products        → cria novo
    /// - PUT    /api/products/{id}   → atualiza
    /// - DELETE /api/products/{id}   → deleta
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        /// <summary>
        /// Construtor que recebe a Service por injeção de dependência
        /// A Service é responsável pela lógica de negócio
        /// </summary>
        public ProductsController(IProductService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET /api/products
        /// 
        /// Retorna a lista de TODOS os produtos do banco de dados
        /// 
        /// EXEMPLO DE RESPOSTA:
        /// [
        ///   { "id": 1, "name": "Notebook", "price": 3000.00, "stockQuantity": 5, "isAvailable": true },
        ///   { "id": 2, "name": "Mouse", "price": 50.00, "stockQuantity": 100, "isAvailable": true }
        /// ]
        /// 
        /// CÓDIGOS HTTP ESPERADOS:
        /// - 200 OK: Sucesso na listagem
        /// - 500 Internal Server Error: Erro no servidor
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAll()
        {
            try
            {
                var products = await _service.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao buscar produtos", error = ex.Message });
            }
        }

        /// <summary>
        /// GET /api/products/{id}
        /// 
        /// Retorna UM produto específico pelo ID
        /// 
        /// EXEMPLO:
        /// GET /api/products/1 → retorna o produto com ID 1
        /// 
        /// CÓDIGOS HTTP ESPERADOS:
        /// - 200 OK: Produto encontrado
        /// - 404 Not Found: Produto não existe
        /// - 400 Bad Request: ID inválido
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            try
            {
                var product = await _service.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound(new { message = $"Produto com ID {id} não encontrado" });
                }

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao buscar produto", error = ex.Message });
            }
        }

        /// <summary>
        /// POST /api/products
        /// 
        /// Cria um NOVO produto
        /// 
        /// EXEMPLO DE REQUISIÇÃO:
        /// {
        ///   "name": "Teclado Mecânico",
        ///   "description": "Teclado RGB 104 teclas",
        ///   "price": 450.00,
        ///   "stockQuantity": 20,
        ///   "isAvailable": true
        /// }
        /// 
        /// CÓDIGOS HTTP ESPERADOS:
        /// - 201 Created: Produto criado com sucesso
        /// - 400 Bad Request: Dados inválidos
        /// - 500 Internal Server Error: Erro no servidor
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdProduct = await _service.CreateProductAsync(product);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao criar produto", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT /api/products/{id}
        /// 
        /// Atualiza um produto existente
        /// 
        /// EXEMPLO DE REQUISIÇÃO:
        /// PUT /api/products/1
        /// {
        ///   "name": "Teclado Mecânico V2",
        ///   "price": 500.00
        /// }
        /// 
        /// CÓDIGOS HTTP ESPERADOS:
        /// - 200 OK: Produto atualizado
        /// - 404 Not Found: Produto não existe
        /// - 400 Bad Request: Dados inválidos
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> Update(int id, [FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedProduct = await _service.UpdateProductAsync(id, product);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao atualizar produto", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE /api/products/{id}
        /// 
        /// Remove um produto do banco de dados
        /// 
        /// EXEMPLO:
        /// DELETE /api/products/1 → deleta o produto com ID 1
        /// 
        /// CÓDIGOS HTTP ESPERADOS:
        /// - 204 No Content: Deletado com sucesso
        /// - 404 Not Found: Produto não existe
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteProductAsync(id);
                if (!deleted)
                {
                    return NotFound(new { message = $"Produto com ID {id} não encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { message = "Erro ao deletar produto", error = ex.Message });
            }
        }
    }
}
