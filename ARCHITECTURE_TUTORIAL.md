# 📚 Clean Architecture - Tutorial Prático para Iniciantes

## 🎯 O que é Clean Architecture?

Clean Architecture é um padrão de organização de código que **separa a aplicação em camadas**, cada uma com uma responsabilidade específica. O objetivo é criar um código **testável, mantível e escalável**.

### ❌ Sem Clean Architecture (Problemas)
```
Controller (tudo junto)
  └─ Acesso ao banco
  └─ Validações
  └─ Lógica de negócio
  └─ Formatação de resposta
```
❌ Difícil testar  
❌ Difícil manter  
❌ Código repetido  
❌ Mudanças quebram tudo  

### ✅ Com Clean Architecture (Solução)
```
Controller (recebe HTTP)
  └─ Service (lógica)
    └─ Repository (dados)
      └─ DbContext (banco)
```
✅ Testável  
✅ Fácil manter  
✅ Código reutilizável  
✅ Mudanças isoladas  

---

## 🏗️ As 4 Camadas de Clean Architecture

### 1️⃣ **Models** (Entidades de Dados)

**O que é?**  
Classe que representa uma tabela no banco de dados. Contém apenas as propriedades (sem lógica).

**Localização:** `Models/Product.cs`

**Exemplo:**
```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsAvailable { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
```

**Para que serve?**
- Define a estrutura dos dados
- Comunica com o banco de dados
- É compartilhada por todas as camadas

---

### 2️⃣ **Repository** (Acesso aos Dados)

**O que é?**  
Camada que isola a lógica de **acesso ao banco de dados**. Apenas lê e salva dados, sem validações.

**Localização:** `Data/Repositories/`

#### Interface (Contrato)
```csharp
public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(int id);
}
```

#### Implementação (Código Real)
```csharp
public class ProductRepository : IProductRepository
{
    private readonly ApiCrudContext _context;

    public ProductRepository(ApiCrudContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }
}
```

**Para que serve?**
- ✅ Acessa o banco de dados
- ✅ Executa queries
- ❌ NÃO valida dados
- ❌ NÃO contém lógica de negócio
- ❌ NÃO formata respostas

**Exemplo do fluxo:**
```
Repository.GetAllProductsAsync()
  ↓
context.Products.ToListAsync()
  ↓
SELECT * FROM Products
  ↓
Retorna lista do banco
```

---

### 3️⃣ **Service** (Lógica de Negócio)

**O que é?**  
Camada que contém as **regras de negócio**. Valida dados, aplica regras e coordena a execução.

**Localização:** `Services/`

#### Interface (Contrato)
```csharp
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(int id, Product product);
    Task<bool> DeleteProductAsync(int id);
}
```

#### Implementação (Código Real)
```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        // Aqui poderíamos filtrar, ordenar, paginar, etc
        return await _repository.GetAllProductsAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        // ✅ VALIDAÇÕES (regras de negócio)
        if (string.IsNullOrWhiteSpace(product.Name))
            throw new ArgumentException("Nome é obrigatório");

        if (product.Price < 0)
            throw new ArgumentException("Preço não pode ser negativo");

        // Define valores padrão
        product.CreatedAt = DateTime.UtcNow;
        product.IsAvailable ??= true;

        // Persiste no banco via Repository
        await _repository.AddProductAsync(product);
        return product;
    }
}
```

**Para que serve?**
- ✅ Contém regras de negócio
- ✅ Valida dados
- ✅ Coordena operations
- ❌ NÃO acessa banco diretamente
- ❌ NÃO formata respostas HTTP

**Exemplos de regras de negócio:**
```
"Produto com preço negativo não pode ser criado"
"Não vender produtos fora de estoque"
"Atualizar timestamp 'UpdatedAt' ao modificar"
"Ativar produto apenas se tiver estoque"
```

**Exemplo do fluxo:**
```
Service.CreateProductAsync(produto)
  ↓
1. Valida se nome é válido
2. Valida se preço >= 0
3. Define CreatedAt = agora
4. Chama repository.AddProductAsync()
  ↓
5. Repository salva no banco
  ↓
6. Service retorna produto criado
```

---

### 4️⃣ **Controller** (API / Requisições HTTP)

**O que é?**  
Camada que **recebe requisições HTTP** dos clientes e **retorna respostas**.

**Localização:** `Controllers/ProductsController.cs`

**Exemplo:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    // GET /api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _service.GetAllProductsAsync();
        return Ok(products); // Retorna 200 OK
    }

    // POST /api/products
    [HttpPost]
    public async Task<ActionResult<Product>> Create([FromBody] Product product)
    {
        try
        {
            var created = await _service.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // Retorna 400 Bad Request
        }
    }

    // DELETE /api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteProductAsync(id);
        if (!deleted)
            return NotFound(); // Retorna 404 Not Found

        return NoContent(); // Retorna 204 No Content
    }
}
```

**Para que serve?**
- ✅ Recebe requisições HTTP
- ✅ Converte dados HTTP em objetos .NET
- ✅ Chama a Service
- ✅ Retorna respostas HTTP
- ❌ NÃO contém regras de negócio

**Códigos HTTP comuns:**
```
200 OK           → Sucesso com dados
201 Created      → Recurso criado
204 No Content   → Sucesso sem dados
400 Bad Request  → Dados inválidos
404 Not Found    → Recurso não existe
500 Error        → Erro no servidor
```

---

## 🔄 Fluxo Completo: Uma Requisição de Criar Produto

```
1. CLIENT (navegador/app)
   │
   └─→ POST /api/products
       { "name": "Notebook", "price": 3000 }
       │
       ├─ HTTP Request
       │
       └─→ 2. CONTROLLER (ProductsController)
           └─ Recebe a requisição
           └─ Extrai os dados do JSON
           └─ Chama: _service.CreateProductAsync(product)
               │
               └─→ 3. SERVICE (ProductService)
                   └─ Valida se nome é válido
                   └─ Valida se preço >= 0
                   └─ Define CreatedAt = agora
                   └─ Chama: _repository.AddProductAsync(product)
                       │
                       └─→ 4. REPOSITORY (ProductRepository)
                           └─ Chama: _context.Products.Add(product)
                           └─ Chama: _context.SaveChangesAsync()
                               │
                               └─→ 5. DBCONTEXT (ApiCrudContext)
                                   └─ Traduz para SQL
                                   └─ Executa: INSERT INTO Products ...
                                       │
                                       └─→ 6. SQL SERVER (Banco de Dados)
                                           └─ Insere o produto na tabela
                                           └─ Retorna: Product (com ID gerado)
                                   │
                                   └─ Retorna: Product
                           │
                           └─ Retorna: Product
                   │
                   └─ Retorna: Product criado
               │
               └─ Verifica se funcionou
               └─ Retorna: HTTP 201 Created
                   { "id": 1, "name": "Notebook", "price": 3000 }
               │
               ├─ HTTP Response
               │
               └─→ 7. CLIENT (Recebe resposta)
                   └─ Produto criado com sucesso!
```

---

## 💉 Injeção de Dependência (DI)

**O que é?**  
O framework cria automaticamente os objetos e **"injeta"** nas classes que precisam.

**Exemplo SEM DI (manual):**
```csharp
// ❌ Ruim - manual, difícil de testar
var context = new ApiCrudContext();
var repository = new ProductRepository(context);
var service = new ProductService(repository);
var controller = new ProductsController(service);
```

**Exemplo COM DI (automático):**
```csharp
// ✅ Bom - framework cria automaticamente
// Program.cs
builder.Services.AddDbContext<ApiCrudContext>(...);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// No Controller
public ProductsController(IProductService service)
{
    _service = service; // Framework injeta automaticamente!
}
```

**Ciclos de Vida:**
```
Transient  → Nova instância a cada uso (raro)
Scoped     → Nova instância por requisição HTTP (mais comum)
Singleton  → Uma instância para toda a aplicação (cache)
```

---

## 🧪 Testando Cada Camada

### Teste de Controller (Integração)
```csharp
[Test]
public async Task GetAll_ReturnsOkWithProducts()
{
    // Arrange
    var mockService = new Mock<IProductService>();
    mockService.Setup(s => s.GetAllProductsAsync())
        .ReturnsAsync(new[] { new Product { Id = 1, Name = "Test" } });
    
    var controller = new ProductsController(mockService.Object);
    
    // Act
    var result = await controller.GetAll();
    
    // Assert
    Assert.AreEqual(200, ((OkObjectResult)result.Result).StatusCode);
}
```

### Teste de Service (Unitário)
```csharp
[Test]
public async Task CreateProduct_ThrowsException_WhenPriceIsNegative()
{
    // Arrange
    var mockRepository = new Mock<IProductRepository>();
    var service = new ProductService(mockRepository.Object);
    
    var product = new Product { Name = "Test", Price = -10 };
    
    // Act & Assert
    Assert.ThrowsAsync<ArgumentException>(
        async () => await service.CreateProductAsync(product)
    );
}
```

### Teste de Repository (Mock)
```csharp
[Test]
public async Task GetAllProducts_CallsDbContext()
{
    // Arrange
    var mockContext = new Mock<ApiCrudContext>();
    var repository = new ProductRepository(mockContext.Object);
    
    // Act
    await repository.GetAllProductsAsync();
    
    // Assert
    mockContext.Verify(c => c.Products.ToListAsync(), Times.Once);
}
```

---

## 📋 Resumo das Responsabilidades

| Camada | Responsabilidade | Exemplo |
|--------|-----------------|---------|
| **Model** | Estrutura de dados | `public string Name { get; set; }` |
| **Repository** | Acesso ao banco | `_context.Products.Add(product)` |
| **Service** | Lógica de negócio | `if (price < 0) throw new Exception()` |
| **Controller** | API HTTP | `[HttpGet] public async Task<IActionResult> GetAll()` |

---

## 🎓 Vantagens da Clean Architecture

| Vantagem | Explicação |
|----------|-----------|
| **Testabilidade** | Cada camada pode ser testada isoladamente |
| **Manutenção** | Mudanças isoladas não quebram tudo |
| **Reutilização** | Mesma Service em múltiplos Controllers |
| **Escalabilidade** | Fácil adicionar novas features |
| **Compreensão** | Código claro, cada coisa em seu lugar |
| **Independência** | Trocar SQL Server por Oracle sem quebrar código |

---

## 📝 Próximas Passos

1. **Criar Database:**
   ```sql
   CREATE DATABASE ApiCrudDB;
   
   USE [ApiCrudDB];
   CREATE TABLE Products (
       Id INT PRIMARY KEY IDENTITY(1,1),
       Name NVARCHAR(200) NOT NULL,
       Price DECIMAL(10,2) NOT NULL
   );
   ```

2. **Executar Migrations (se usar EF Core Migrations):**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Testar Endpoints:**
   ```bash
   # Listar todos
   GET http://localhost:7208/api/products
   
   # Criar novo
   POST http://localhost:7208/api/products
   Body: { "name": "Mouse", "price": 50 }
   
   # Deletar
   DELETE http://localhost:7208/api/products/1
   ```

---

## ✅ Checklist de Implementação

- ✅ Models criados
- ✅ DbContext configurado
- ✅ Repository interface criada
- ✅ Repository implementado
- ✅ Service interface criada
- ✅ Service implementada
- ✅ Controller criado
- ✅ DI registrado em Program.cs
- ✅ Connection String em appsettings.json
- ⏳ Banco de dados criado (manual)

---

## 🚀 Você está pronto!

Agora você entende Clean Architecture! A estrutura está pronta para:
- Criar novos endpoints
- Adicionar validações
- Trocar implementações
- Escrever testes
- Escalar a aplicação

**Parabéns! 🎉**
