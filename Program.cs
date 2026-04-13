using ApiCrud.Data;
using ApiCrud.Data.Repositories;
using ApiCrud.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ====================================
// 🏗️  INJEÇÃO DE DEPENDÊNCIAS (DI)
// ====================================
// 
// ℹ️  O QUE É INJEÇÃO DE DEPENDÊNCIA?
// É o padrão que injeta automaticamente as dependências de um serviço.
// Em vez de criar objetos manualmente, o framework cria para nós.
//
// EXEMPLO:
// Sem DI: var service = new ProductService(new ProductRepository(context));
// Com DI: [Dependency] IProductService _service; // Framework cria automaticamente!
//
// VANTAGENS:
// - Código mais testável (podemos injetar mocks)
// - Menos acoplamento
// - Código mais limpo
// - Gerenciamento automático de ciclo de vida
//

// 1️⃣  REGISTRAR DBCONTEXT (Banco de Dados)
// Scoped = cria nova instância por requisição HTTP
// Isso garante que cada requisição tenha sua própria conexão com o banco
builder.Services.AddDbContext<ApiCrudContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣  REGISTRAR REPOSITORY (Acesso aos Dados)
// Scoped = cria nova instância por requisição
// O Repository recebe o DbContext automaticamente
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICaioRepository, CaioRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();  


// 3️⃣  REGISTRAR SERVICE (Lógica de Negócio)
// Scoped = cria nova instância por requisição
// A Service recebe o Repository automaticamente
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICaioService, CaioService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//REGISTRAR JWT AUTHENTICATION
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });


// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();