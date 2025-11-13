using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BackEndAPI.Data;

var builder = WebApplication.CreateBuilder(args);

const string ProductionConnectionStringKey = "ConnectionStrings__DefaultConnection";

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<BackEndAPIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BackEndAPIContext") ?? throw new InvalidOperationException("Connection string 'BackEndAPIContext' not found.")));
} 
else
{
    var connectionString = builder.Configuration.GetConnectionString(ProductionConnectionStringKey);

    // Se a string ainda não for encontrada, tenta buscar DIRETAMENTE como uma variável de ambiente
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration[ProductionConnectionStringKey];
    }
    
    // Finaliza com erro se a string de conexão real não foi encontrada
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException($"Connection string '{ProductionConnectionStringKey}' not found. Please check Render environment variables.");
    }

    // Configura o DbContext com a string de conexão encontrada
    builder.Services.AddDbContext<BackEndAPIContext>(options =>
        options.UseNpgsql(connectionString));
}

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.UseSwagger();
app.UseSwaggerUI();    

app.Run();
