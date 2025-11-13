using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BackEndAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<BackEndAPIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BackEndAPIContext") ?? throw new InvalidOperationException("Connection string 'BackEndAPIContext' not found.")));
} 
else
{
    builder.Services.AddDbContext<BackEndAPIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionStrings__DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ConnectionStrings__DefaultConnection' not found.")));
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
