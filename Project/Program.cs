using EvolveDb;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using RestWithASPNET;
using RestWithASPNET.Business;
using RestWithASPNET.Business.Implementations;
using RestWithASPNET.Configurations;
using RestWithASPNET.Hypermedia.Enricher;
using RestWithASPNET.Hypermedia.Filters;
using RestWithASPNET.Repository;
using RestWithASPNET.Repository.Generic;
using RestWithASPNET.Repository.Implementations;
using RestWithASPNET.Services;
using RestWithASPNET.Services.Implementations;
using RestWithASPNETUdemy.Business;
using RestWithASPNETUdemy.Business.Implementations;
using RestWithASPNETUdemy.Hypermedia.Enricher;
using RestWithASPNETUdemy.Model.Context;
using RestWithASPNETUdemy.Repository;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Setar valores do appsettings.json no tokenConfigurations
var tokenConfigurations = new TokenConfiguration();
new ConfigureFromConfigurationOptions<TokenConfiguration>(
        builder.Configuration.GetSection("TokenConfigurations")
    )
    .Configure(tokenConfigurations);

builder.Services.AddSingleton(tokenConfigurations);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = tokenConfigurations.Issuer,
            ValidAudience = tokenConfigurations.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret)) //Chave Simetrica
        };
    });

//Serviço de Autorização
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build());
});





//Permitir qualquer origem, metodo ou header configuração Default para consumir API
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
    options.RespectBrowserAcceptHeader = true; //Para aceitar a propriedade setada no cabeçalho do header da request

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




//Injeção de Dependencia
builder.Services.AddScoped<IPersonBusiness, PersonBusiness>();
builder.Services.AddScoped<IBookBusiness, BookBusiness>();
builder.Services.AddScoped<ILoginBusiness, LoginBusiness>();

//Injeção de Dependencia : Repository Token/USuario
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

//Injeção de Dependencia : Repository Generica
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

//Injeção de Dependencia : Repository Files
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IFileBusiness, FileBusiness>();

// Adicionando serviços ao contêiner
builder.Services.AddControllers().AddNewtonsoftJson(); // Adiciona suporte ao Newtonsoft Json


//Adicionando Injeção do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
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




var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
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
app.MapControllerRoute("DefaultApi", "{controller=values}/v{version=apiVersion}/{id?}"); // Sugestão do Comentario


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
