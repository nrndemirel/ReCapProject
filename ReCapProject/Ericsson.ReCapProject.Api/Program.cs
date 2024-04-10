using Ericsson.ReCapProject.Api.Authorization;
using Ericsson.ReCapProject.Core.Injectors;
using Ericsson.ReCapProject.Persistence;
using Ericsson.ReCapProject.Persistence.DbContexts;
using Ericsson.ReCapProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Ericsson.ReCapProject.Api.Handlers;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

//TODO Policies will be updated according to the need.
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AzureReCapProjectPolicy", policy => policy.Requirements.Add(new GroupAuthorizationRequirement(builder.Configuration["AzureReCapProjectGroup:GroupId"])));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Azure Demo", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "OAuth2.0 which uses AuthorizationCode flow",
            Name = "oauth2.0",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(builder.Configuration["SwaggerAzureAd:AuthorizationURL"]),
                    TokenUrl = new Uri(builder.Configuration["SwaggerAzureAd:TokenURL"]),
                    Scopes = new Dictionary<string, string>
                    {
                        { builder.Configuration["SwaggerAzureAD:Scope"], "Access API as User" }
                    }
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                }
                ,                new[] { builder.Configuration["SwaggerAzureAd:Scope"] }
            }
        });
    });

builder.Services.AddDbContext<ReCapProjectDbContext>(configuration =>
{
    configuration.UseSqlServer(builder.Configuration["ReCapProjectDbConnectionString"]);
});
builder.Services.AddSingleton<ServiceAssemblyLoader>();
builder.Services.AddSingleton<RepositoryAssemblyLoader>();
builder.Services.AddInjectionByAttribute();
builder.Services.AddSingleton<IAuthorizationHandler, GroupAuthorizationHandler>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId(builder.Configuration["ReCapProjectSwaggerClientId"]);
        c.OAuthUsePkce();
        //c.OAuthScopeSeparator(" "); If multiples scopes
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseExceptionHandler();

app.MapControllers();

app.Run();
