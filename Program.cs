using Microsoft.EntityFrameworkCore;
using BackEndAPI.Data;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

string? connectionString;

if (builder.Environment.IsDevelopment())
{
    Console.WriteLine("Development environment");

    connectionString = builder.Configuration.GetConnectionString("BackEndAPIContext");

    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Connection string not found in appsettings.json");

    Console.WriteLine($"Connection string = {connectionString}");
}
else
{
    Console.WriteLine("Production environment");

    connectionString = builder.Configuration["ConnectionStrings__DefaultConnection"];

    if (string.IsNullOrEmpty(connectionString))
        throw new InvalidOperationException("Production connection string not found");

    Console.WriteLine($"Connection string received (URI): {connectionString}");

    if (connectionString.StartsWith("postgres://"))
    {
        connectionString = ConvertPostgresUriToConnectionString(connectionString);
        Console.WriteLine($"Converted to Npgsql format: {connectionString}");
    }
}

builder.Services.AddDbContext<BackEndAPIContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();

static string ConvertPostgresUriToConnectionString(string connectionString)
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');

    var npgBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Username = userInfo.Length > 0 ? userInfo[0] : "",
        Password = userInfo.Length > 1 ? userInfo[1] : "",
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require
    };

    return npgBuilder.ConnectionString;
}

