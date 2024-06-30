using EvolveDb;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using RestWithASPNET.Business;
using RestWithASPNET.Business.Implementations;
using RestWithASPNET.Hypermedia.Enricher;
using RestWithASPNET.Hypermedia.Filters;
using RestWithASPNET.Repository.Generic;
using RestWithASPNETUdemy.Business;
using RestWithASPNETUdemy.Business.Implementations;
using RestWithASPNETUdemy.Hypermedia.Enricher;
using RestWithASPNETUdemy.Model.Context;
using RestWithASPNETUdemy.Repository;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Permitir qualquer origem, metodo ou header configura��o Default para consumir API
builder.Services.AddCors(options => options.AddDefaultPolicy(builder => {
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

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



//Descomentar para habilitar saida da API em XML
builder.Services.AddMvc(options => //Content Negotiation
{
    options.RespectBrowserAcceptHeader = true; //Para aceitar a propriedade setada no cabe�alho do header da request

    options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
    options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
})
.AddXmlSerializerFormatters();



var filteroptions = new HyperMediaFilterOptions();
filteroptions.ContentResponseEnricherList.Add(new PersonEnricher());
filteroptions.ContentResponseEnricherList.Add(new BookEnricher());
builder.Services.AddSingleton(filteroptions);

//Versioning API
builder.Services.AddApiVersioning();

//Adicionando Inje��o do Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new OpenApiInfo
        {
            //Atributos
            Title = "Rest API's From 0 to Azure with ASP.NET Core and Docker",
            Version = "v1",
            Description = "API RESTful developed in course 'Rest API's From 0 to Azure with ASP.NET Core and Docker'",
            Contact = new OpenApiContact
            {
                Name = "Leonardo Silva",
                Url = new Uri("https://github.com/leosilvajr")
            }
        });
});



//Inje��o de Dependencia
builder.Services.AddScoped<IPersonBusiness, PersonBusinessImplementation>();
//builder.Services.AddScoped<IRepository, PersonRepositoryImplementation>();

builder.Services.AddScoped<IBookBusiness, BookBusinessImplementation>();
//builder.Services.AddScoped<IBookRepository, BookRepositoryImplementation>();



builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();


app.UseSwagger(); //Responsavel por gerar JSON

app.UseSwaggerUI(C =>
{
    C.SwaggerEndpoint("/swagger/v1/swagger.json", "Rest API's From 0 to Azure with ASP.NET Core and Docker");
}); //Responsavel por gerar pagina HMTL

//Swagger Page
var option = new RewriteOptions();
option.AddRedirect("^$", "swagger"); // Redirecionar para a pagina do Swagger
app.UseRewriter(option);

app.MapControllers();
//app.MapControllerRoute("DefaultApi", "{controller=values}/{id?}");
app.MapControllerRoute("DefaultApi", "{controller=values}/v{version=apiVersion}/{id?}"); // Sugest�o do Comentario


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
