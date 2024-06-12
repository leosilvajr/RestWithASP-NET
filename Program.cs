using EvolveDb;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using RestWithASPNET.Business;
using RestWithASPNET.Business.Implementations;
using RestWithASPNET.Repository;
using RestWithASPNET.Repository.Implementations;
using RestWithASPNETUdemy.Business;
using RestWithASPNETUdemy.Business.Implementations;
using RestWithASPNETUdemy.Model.Context;
using RestWithASPNETUdemy.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Propriedades de Banco de Dados MySQL
var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySQLContext>(op => op.UseMySql(
    connection,
    new MySqlServerVersion(new Version(8, 0, 29))));

if (builder.Environment.IsDevelopment())
{
    MigrateDatabase(connection);
}
//Versioning API
builder.Services.AddApiVersioning();

//Injeção de Dependencia
builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
builder.Services.AddScoped<IPersonRepository, PersonRepositoryImplementation>();

builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();
builder.Services.AddScoped<IBookRepository, BookRepositoryImplementation>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void MigrateDatabase(string connection)
{
    try
    {
        var evolveConnection = new MySqlConnection(connection);
        var evolve = new Evolve(evolveConnection, Log.Information)
        {
            Locations = new List<string> { "db/migrations", "db/dataset" },
            IsEraseDisabled = true,
        };
        evolve.Migrate();
    }
    catch (Exception ex)
    {
        Log.Error("Database migration failed", ex);
        throw;
    }
}