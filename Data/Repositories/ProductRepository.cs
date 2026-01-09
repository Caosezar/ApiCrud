using ApiCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCrud.Data.Repositories
{
    /// <summary>
    /// ProductRepository - Implementação da camada Repository
    /// 
    /// ℹ️ O QUE FAZ O REPOSITORY?
    /// - Isola a lógica de acesso aos dados
    /// - Traduz operações de negócio em queries de banco
    /// - Fica entre a Service e o DbContext
    /// - Se o banco mudar, só alteram aqui (não na Service)
    /// 
    /// FLUXO: Controller → Service → Repository → DbContext → Banco
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApiCrudContext _context;

        /// <summary>
        /// Construtor que recebe o DbContext por injeção de dependência
        /// O DbContext é usado para acessar o banco de dados
        /// </summary>
        public ProductRepository(ApiCrudContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Busca TODOS os produtos do banco de dados
        /// 
        /// EXEMPLO DE USO:
        /// var produtos = await _repository.GetAllProductsAsync();
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            // ToListAsync(): Executa a query e traz os dados do banco
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Busca UM produto pelo ID
        /// 
        /// EXEMPLO DE USO:
        /// var produto = await _repository.GetProductByIdAsync(1);
        /// </summary>
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            // FirstOrDefaultAsync(id): Retorna primeiro match ou null
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Adiciona um novo produto ao banco (CREATE)
        /// Nota: SaveChangesAsync() deve ser chamado pela Service
        /// </summary>
        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza um produto existente (UPDATE)
        /// Nota: SaveChangesAsync() deve ser chamado pela Service
        /// </summary>
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove um produto do banco (DELETE)
        /// </summary>
        public async Task DeleteProductAsync(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
