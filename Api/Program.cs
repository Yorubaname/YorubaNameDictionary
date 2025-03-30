using Application.Validation;
using Core.StringObjectConverters;
using FluentValidation;
using Infrastructure.Twitter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text.Json.Serialization;
using Hangfire;
using Infrastructure.Hangfire;
using Infrastructure.Redis;
using Ardalis.GuardClauses;
using YorubaOrganization.Core.Enums;
using YorubaOrganization.Core.Events;
using YorubaOrganization.Core.Cache;
using Application.EventHandlers;
using Infrastructure.MongoDB;
using Api.Middleware;
using YorubaOrganization.Core.Tenants;
using Api.Tenants;
using Microsoft.AspNetCore.HttpOverrides;
using Application.Services.MultiLanguage;
using YorubaOrganization.Application.Services;
using Api.Utilities;
using Application.Services.Names;
using Application.Services.Words;
using Core.Entities;
using Words.Core.Entities;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
configuration.AddEnvironmentVariables("YND_");

string DevCORSAllowAll = "AllowAllForDev";
var services = builder.Services;

// Add services to the container.

services.AddCors(options =>
{
    options.AddPolicy(name: DevCORSAllowAll,
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });
});

services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Configure policies
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Role.ADMIN.ToString()));
    options.AddPolicy("AdminAndLexicographers", policy => policy.RequireRole(
        Role.ADMIN.ToString(),
        Role.PRO_LEXICOGRAPHER.ToString(),
        Role.BASIC_LEXICOGRAPHER.ToString()
        ));
    options.AddPolicy("AdminAndProLexicographers", policy => policy.RequireRole(
        Role.ADMIN.ToString(),
        Role.PRO_LEXICOGRAPHER.ToString()
        ));
});

services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new CommaSeparatedStringConverter());
    options.JsonSerializerOptions.Converters.Add(new HyphenSeparatedStringConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SchemaFilter<CustomSchemaFilter>();

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Yoruba Names Dictionary API", Version = "v1" });

    // TODO Hafiz: Implement the class required for the code below.
    //c.DocumentFilter<HostBasedTitleDocumentFilter>();

    // Define the Basic Authentication scheme
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Description = "Basic Authentication"
    });

    // Apply the Basic Authentication scheme globally
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    Array.Empty<string>()
                }
            });
});

services.AddTransient<ILanguageService, LanguageService>();
services.AddScoped<ITenantProvider, HttpTenantProvider>();
services.AddHttpContextAccessor();

services.InitializeDatabase(configuration);

builder.Services.AddTransient(x =>
  new MySqlConnection(Guard.Against.NullOrEmpty(configuration.GetSection("MySQL:ConnectionString").Value)));

services.AddScoped<GeoLocationsService>();
services.AddScoped<IEventPubService, EventPubService>();
services.AddScoped<UserService>();

services
    .AddScoped<EntryFeedbackService<NameEntry>>()
    .AddScoped<NameEntryFeedbackService>()
    .AddScoped<NameEntryService>()
    .AddScoped<NameSearchService>()
    .AddScoped<SuggestedNameService>();

// Words
services
    .AddScoped<EntryFeedbackService<WordEntry>>()
    .AddScoped<WordFeedbackService>()
    .AddScoped<WordEntryService>()
    .AddScoped<WordSearchService>();

services
    .AddScoped<IRecentIndexesCache, RedisRecentIndexesCache>()
    .AddScoped<IRecentSearchesCache, RedisRecentSearchesCache>();

//Validation
// TODO YDict: Check that the removed validator injections do not cause negative impact.
services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PostPublishedNameCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ExactEntrySearchedEventHandler).Assembly);
});

// Twitter integration configuration
services.AddSingleton<ITwitterService, TwitterService>();
services.AddTwitterClient(configuration);

services.SetupHangfire(Guard.Against.NullOrEmpty(configuration.GetRequiredSection("MongoDB:Default").Value));
services.SetupRedis(configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(DevCORSAllowAll);
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
});
app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<TenantIdentificationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/backJobMonitor");
}

app.Run();
