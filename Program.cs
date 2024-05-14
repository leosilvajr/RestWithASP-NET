using Microsoft.EntityFrameworkCore;
using RestWithASPNET.Model.Context;
using RestWithASPNET.Services;
using RestWithASPNET.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Propriedades de Banco de Dados MySQL
var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySQLContext>(op => op.UseMySql(
    connection,
    new MySqlServerVersion(new Version(8, 0, 29))));

//Injeção de Dependencia
builder.Services.AddScoped<IPersonService, PersonServiceImplementation>(); 

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
