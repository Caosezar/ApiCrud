using ApiCrud.Models;

namespace ApiCrud.Data.Repositories
{
    /// <summary>
    /// IProductRepository - Interface da camada Repository
    /// 
    /// ℹ️ POR QUE USAR UMA INTERFACE?
    /// - Define um contrato: qualquer implementação DEVE ter esses métodos
    /// - Facilita testes (podemos criar implementações fake)
    /// - Desacopla a aplicação da tecnologia específica (EF Core, Dapper, etc)
    /// - Possibilita múltiplas implementações
    /// 
    /// EXEMPLO: Podemos trocar SQL Server por Oracle sem quebrar a aplicação
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>Retorna todos os produtos do banco de dados</summary>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>Retorna um produto específico pelo ID</summary>
        Task<Product?> GetProductByIdAsync(int id);

        /// <summary>Insere um novo produto no banco</summary>
        Task AddProductAsync(Product product);

        /// <summary>Atualiza um produto existente</summary>
        Task UpdateProductAsync(Product product);

        /// <summary>Remove um produto pelo ID</summary>
        Task DeleteProductAsync(int id);
    }
}
