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

        /// <summary>Tabela de Produtos</summary>
        public DbSet<Product> Products { get; set; }

        // /// <summary>Tabela de Usuários</summary>
        // public DbSet<User> Users { get; set; }

        // /// <summary>Tabela de Categorias</summary>
        // public DbSet<Category> Categories { get; set; }

        // /// <summary>Tabela de Pedidos</summary>
        // public DbSet<Order> Orders { get; set; }

        // /// <summary>Tabela de Itens de Pedidos</summary>
        // public DbSet<OrderItem> OrderItems { get; set; }

        // /// <summary>Tabela de Logs de Auditoria</summary>
        // public DbSet<AuditLog> AuditLogs { get; set; }

        /// <summary>
        /// Configurações adicionais do modelo (opcional, usa data annotations por padrão)
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // // Configuração da tabela Users
            // modelBuilder.Entity<User>(entity =>
            // {
            //     entity.HasKey(e => e.Id);

            //     entity.Property(e => e.FirstName)
            //         .IsRequired()
            //         .HasMaxLength(100);

            //     entity.Property(e => e.LastName)
            //         .IsRequired()
            //         .HasMaxLength(100);

            //     entity.Property(e => e.Email)
            //         .IsRequired()
            //         .HasMaxLength(255);

            //     entity.Property(e => e.Phone)
            //         .HasMaxLength(20);

            //     entity.Property(e => e.IsActive)
            //         .HasDefaultValue(true);

            //     entity.Property(e => e.CreatedAt)
            //         .HasDefaultValueSql("GETDATE()");

            //     entity.Property(e => e.UpdatedAt)
            //         .HasDefaultValueSql("GETDATE()");

            //     // Índice único para email
            //     entity.HasIndex(e => e.Email)
            //         .IsUnique();
            // });

            // // Configuração da tabela Categories
            // modelBuilder.Entity<Category>(entity =>
            // {
            //     entity.HasKey(e => e.Id);

            //     entity.Property(e => e.Name)
            //         .IsRequired()
            //         .HasMaxLength(100);

            //     entity.Property(e => e.Description)
            //         .HasMaxLength(500);

            //     entity.Property(e => e.CreatedAt)
            //         .HasDefaultValueSql("GETDATE()");

            //     entity.HasIndex(e => e.Name);
            // });

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

                // Relacionamento com Category (N Products -> 1 Category)
                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Name);
            });

            // // Configuração da tabela Orders
            // modelBuilder.Entity<Order>(entity =>
            // {
            //     entity.HasKey(e => e.Id);

            //     entity.Property(e => e.OrderDate)
            //         .HasDefaultValueSql("GETDATE()");

            //     entity.Property(e => e.Status)
            //         .HasMaxLength(50)
            //         .HasDefaultValue("Pending");

            //     entity.Property(e => e.TotalAmount)
            //         .HasColumnType("decimal(10, 2)")
            //         .HasDefaultValue(0);

            //     entity.Property(e => e.ShippingAddress)
            //         .HasMaxLength(500);

            //     entity.Property(e => e.Notes)
            //         .HasMaxLength(1000);

            //     // Relacionamento com User (N Orders -> 1 User)
            //     entity.HasOne(e => e.User)
            //         .WithMany(u => u.Orders)
            //         .HasForeignKey(e => e.UserId)
            //         .OnDelete(DeleteBehavior.Restrict);

            //     entity.HasIndex(e => e.UserId);
            // });

            // // Configuração da tabela OrderItems
            // modelBuilder.Entity<OrderItem>(entity =>
            // {
            //     entity.HasKey(e => e.Id);

            //     entity.Property(e => e.Quantity)
            //         .IsRequired();

            //     entity.Property(e => e.UnitPrice)
            //         .HasColumnType("decimal(10, 2)")
            //         .IsRequired();

            //     entity.Property(e => e.Subtotal)
            //         .HasColumnType("decimal(10, 2)");

            //     // Relacionamento com Order (N OrderItems -> 1 Order)
            //     entity.HasOne(e => e.Order)
            //         .WithMany(o => o.OrderItems)
            //         .HasForeignKey(e => e.OrderId)
            //         .OnDelete(DeleteBehavior.Cascade);

            //     // Relacionamento com Product (N OrderItems -> 1 Product)
            //     entity.HasOne(e => e.Product)
            //         .WithMany(p => p.OrderItems)
            //         .HasForeignKey(e => e.ProductId)
            //         .OnDelete(DeleteBehavior.Restrict);

            //     entity.HasIndex(e => e.OrderId);
            //     entity.HasIndex(e => e.ProductId);
            // });

            // // Configuração da tabela AuditLogs
            // modelBuilder.Entity<AuditLog>(entity =>
            // {
            //     entity.HasKey(e => e.Id);

            //     entity.Property(e => e.TableName)
            //         .IsRequired()
            //         .HasMaxLength(100);

            //     entity.Property(e => e.RecordId)
            //         .IsRequired();

            //     entity.Property(e => e.Action)
            //         .IsRequired()
            //         .HasMaxLength(20);

            //     entity.Property(e => e.OldData)
            //         .HasColumnType("nvarchar(max)");

            //     entity.Property(e => e.NewData)
            //         .HasColumnType("nvarchar(max)");

            //     entity.Property(e => e.ChangedBy)
            //         .HasMaxLength(100);

            //     entity.Property(e => e.ChangedAt)
            //         .HasDefaultValueSql("GETDATE()");

            //     entity.HasIndex(e => e.TableName);
            //     entity.HasIndex(e => e.ChangedAt);
            // });
        }
    }
}
