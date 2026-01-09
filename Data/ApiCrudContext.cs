using Microsoft.EntityFrameworkCore;
using ApiCrud.Models;

namespace ApiCrud.Data
{
    /// <summary>
    /// DbContext é a ponte entre a aplicação e o banco de dados.
    /// 
    /// ℹ️ O QUE É UM DBCONTEXT?
    /// - Representa uma sessão com o banco de dados
    /// - Gerencia as entidades e suas mudanças
    /// - Traduz operações .NET em comandos SQL
    /// - Cada DbSet<T> representa uma tabela no banco
    /// 
    /// EXEMPLO: context.Products representa a tabela Products no banco
    /// </summary>
    public class ApiCrudContext : DbContext
    {
        /// <summary>
        /// Construtor que recebe as opções de configuração do DbContext
        /// (injetadas pelo container de dependência do ASP.NET Core)
        /// </summary>
        public ApiCrudContext(DbContextOptions<ApiCrudContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet que representa a tabela Products no banco de dados.
        /// Permite fazer operações LINQ como context.Products.ToList()
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Configurações adicionais do modelo (opcional, usa data annotations por padrão)
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da tabela Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasMaxLength(1000);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)");

                entity.Property(e => e.StockQuantity)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsAvailable)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                // Índice para melhor performance
                entity.HasIndex(e => e.Name);
            });
        }
    }
}
