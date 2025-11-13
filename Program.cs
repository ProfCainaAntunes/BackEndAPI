using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BackEndAPI.Data;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BackEndAPIContext");

if (connectionString != null && connectionString.StartsWith("postgres://"))
{
    // Converte postgres://user:pass@host:port/db -> Host=...;Username=...;Password=...;Database=...;SSL Mode=Require;Trust Server Certificate=true
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<BackEndAPIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BackEndAPIContext") ?? throw new InvalidOperationException("Connection string 'BackEndAPIContext' not found.")));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
