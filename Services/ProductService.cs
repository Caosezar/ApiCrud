using ApiCrud.Data.Repositories;
using ApiCrud.Models;

namespace ApiCrud.Services
{
    /// <summary>
    /// ProductService - Implementação da camada Service
    /// 
    /// ℹ️ O QUE FAZ A SERVICE?
    /// - Contém a LÓGICA DE NEGÓCIO da aplicação
    /// - Valida os dados antes de persistir
    /// - Aplica regras de negócio (ex: não vender produtos fora de estoque)
    /// - Coordena múltiplos repositories se necessário
    /// - NÃO sabe detalhes de como os dados são armazenados
    /// 
    /// EXEMPLO: "Um produto só pode ser criado se tiver preço > 0"
    /// Esta regra fica na Service, não no Repository
    /// 
    /// FLUXO COMPLETO:
    /// 1. Controller recebe requisição HTTP
    /// 2. Controller chama a Service
    /// 3. Service valida e aplica regras
    /// 4. Service chama o Repository
    /// 5. Repository acessa o banco via DbContext
    /// 6. Service retorna resultado ao Controller
    /// 7. Controller retorna resposta HTTP
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        /// <summary>
        /// Construtor que recebe o Repository por injeção de dependência
        /// Isso permite trocar a implementação do Repository sem alterar a Service
        /// </summary>
        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Busca todos os produtos
        /// 
        /// EXEMPLO DE USO:
        /// var produtos = await _service.GetAllProductsAsync();
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // Aqui poderíamos aplicar filtros, ordenação, paginação, etc
            var products = await _repository.GetAllProductsAsync();
            return products;
        }

        /// <summary>
        /// Busca um produto pelo ID
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("ID deve ser maior que 0");
            }

            return await _repository.GetProductByIdAsync(id);
        }

        /// <summary>
        /// Cria um novo produto com validações de negócio
        /// 
        /// VALIDAÇÕES:
        /// - Nome é obrigatório
        /// - Preço deve ser >= 0
        /// - StockQuantity deve ser >= 0
        /// </summary>
        public async Task<Product> CreateProductAsync(Product product)
        {
            // VALIDAÇÕES DE NEGÓCIO
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Nome do produto é obrigatório");
            }

            if (product.Name.Length > 200)
            {
                throw new ArgumentException("Nome não pode ter mais de 200 caracteres");
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("Preço não pode ser negativo");
            }

            if (product.StockQuantity.HasValue && product.StockQuantity < 0)
            {
                throw new ArgumentException("Quantidade em estoque não pode ser negativa");
            }

            // Define valores padrão
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            product.IsAvailable ??= true;
            product.StockQuantity ??= 0;

            // Persiste no banco via Repository
            await _repository.AddProductAsync(product);
            return product;
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            // Verifica se o produto existe
            var existingProduct = await _repository.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Produto com ID {id} não encontrado");
            }

            // VALIDAÇÕES
            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("Nome do produto é obrigatório");
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("Preço não pode ser negativo");
            }

            // Atualiza os campos
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity ?? 0;
            existingProduct.IsAvailable = product.IsAvailable ?? true;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            // Persiste as alterações
            await _repository.UpdateProductAsync(existingProduct);
            return existingProduct;
        }

        /// <summary>
        /// Deleta um produto
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            await _repository.DeleteProductAsync(id);
            return true;
        }
    }
}
