using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
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
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Setar valores do appsettings.json no tokenConfigurations
var tokenConfigurations = new TokenConfiguration();
new ConfigureFromConfigurationOptions<TokenConfiguration>(
    builder.Configuration.GetSection("TokenConfigurations"))
    .Configure(tokenConfigurations);

builder.Services.AddSingleton(tokenConfigurations);

// Configura��o da autentica��o com JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigurations.Secret))
    };
});

// Servi�o de Autoriza��o
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build());
});

// Configura��o do CORS permitindo qualquer origem, m�todo ou cabe�alho
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Adicionando suporte a controladores e JSON
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Adiciona suporte ao Newtonsoft Json

// Configura��o do MySQL
var connection = builder.Configuration["MySQLConnection:MySQLConnectionString"];
builder.Services.AddDbContext<MySQLContext>(op => op.UseMySql(
    connection,
    new MySqlServerVersion(new Version(8, 0, 29))));

// Content Negotiation para JSON e XML
builder.Services.AddMvc(options =>
{
    options.RespectBrowserAcceptHeader = true;
    options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
    // options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml")); // Descomente se quiser XML
})
.AddXmlSerializerFormatters();

// Configura��o de Hypermedia (HATEOAS)
var filterOptions = new HyperMediaFilterOptions();
filterOptions.ContentResponseEnricherList.Add(new PersonEnricher());
filterOptions.ContentResponseEnricherList.Add(new BookEnricher());
builder.Services.AddSingleton(filterOptions);

// Versionamento de API
builder.Services.AddApiVersioning();

// Inje��o de depend�ncia - Servi�os e Reposit�rios
builder.Services.AddScoped<IPersonBusiness, PersonBusiness>();
builder.Services.AddScoped<IBookBusiness, BookBusiness>();
builder.Services.AddScoped<ILoginBusiness, LoginBusiness>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IFileBusiness, FileBusiness>();

// Adicionando suporte ao Swagger
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

// Configura��o do pipeline HTTP
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Swagger Middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rest API's From 0 to Azure with ASP.NET Core and Docker");
});

// Redirecionamento para a p�gina do Swagger
var option = new RewriteOptions();
option.AddRedirect("^$", "swagger");
app.UseRewriter(option);

app.MapControllers();
app.MapControllerRoute("DefaultApi", "{controller=values}/v{version=apiVersion}/{id?}");

app.Run();

// M�todo para migra��o do banco de dados usando Evolve
//void MigrateDatabase(string connection)
//{
//    try
//    {
//        var evolveConnection = new MySqlConnection(connection);
//        var evolve = new Evolve(evolveConnection, Log.Information)
//        {
//            Locations = new List<string> { "db/migrations", "db/dataset" },
//            IsEraseDisabled = true,
//        };
//        evolve.Migrate();
//    }
//    catch (Exception ex)
//    {
//        Log.Error("Database migration failed", ex);
//        throw;
//    }
//}
