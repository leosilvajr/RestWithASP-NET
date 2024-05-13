using RestWithASPNET.Services;
using RestWithASPNET.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Injeção de Dependencia
builder.Services.AddScoped<IPersonService, PersonServiceImplementation>(); 

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
