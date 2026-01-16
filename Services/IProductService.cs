using ApiCrud.Models;

namespace ApiCrud.Services
{
    /// <summary>
    /// IProductService - Interface da camada Service
    /// 
    /// ℹ️ POR QUE UMA INTERFACE AQUI TAMBÉM?
    /// - Mantém a separação de responsabilidades
    /// - Define o contrato que o Controller espera
    /// 
    /// IMPORTANTE: Service contém a LÓGICA DE NEGÓCIO
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Busca todos os produtos
        /// 
        /// DIFERENÇA DO REPOSITORY:
        /// - Repository: Apenas retorna do banco
        /// - Service: Pode aplicar regras, filtros, validações, transformações
        /// </summary>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>Busca um produto pelo ID</summary>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>Cria um novo produto com validações</summary>
        Task<Product> CreateProductAsync(Product product);

        /// <summary>Atualiza um produto com validações</summary>
        Task<Product> UpdateProductAsync(int id, Product product);

        /// <summary>Deleta um produto</summary>
        Task<bool> DeleteProductAsync(int id);
    }
}
