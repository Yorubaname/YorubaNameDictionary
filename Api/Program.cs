using Api.ExceptionHandler;
using Application.Cache;
using Application.Domain;
using Application.Events;
using Application.Migrator;
using Application.Services;
using Application.Validation;
using Core.Cache;
using Core.Enums;
using Core.Events;
using Core.StringObjectConverters;
using FluentValidation;
using Infrastructure.Twitter;
using Infrastructure.MongoDB;
using Infrastructure.Hangfire;
using Microsoft.AspNetCore.Authentication;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Dashboard;
using Api.Utilities;

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
    options.AddPolicy("AdminAndLexicographers", policy => policy.RequireRole(Role.ADMIN.ToString(), Role.PRO_LEXICOGRAPHER.ToString(), Role.BASIC_LEXICOGRAPHER.ToString()));
    options.AddPolicy("AdminAndProLexicographers", policy => policy.RequireRole(Role.ADMIN.ToString(), Role.PRO_LEXICOGRAPHER.ToString()));
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
                    new string[] { }
                }
            });
});
var mongoDbSettings = configuration.GetRequiredSection("MongoDB");
services.InitializeDatabase(mongoDbSettings.GetValue<string>("ConnectionString"), mongoDbSettings.GetValue<string>("DatabaseName"));

builder.Services.AddTransient(x =>
  new MySqlConnection(builder.Configuration.GetSection("MySQL:ConnectionString").Value));

services.AddSingleton<NameEntryService>();
services.AddSingleton<GeoLocationsService>();
services.AddSingleton<NameEntryFeedbackService>();
services.AddSingleton<IEventPubService, EventPubService>();
services.AddSingleton<SearchService>();
services.AddSingleton<SuggestedNameService>();
services.AddSingleton<UserService>();
services.AddScoped<GeoLocationValidator>();
services.AddScoped<EmbeddedVideoValidator>();
services.AddScoped<EtymologyValidator>();
services.AddScoped<SqlToMongoMigrator>();
services.AddSingleton<IRecentIndexesCache, RecentIndexesCache>();
services.AddSingleton<IRecentSearchesCache, RecentSearchesCache>();

//Validation
services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();

services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ExactNameSearchedAdapter).Assembly));

// Twitter integration configuration
services.AddSingleton<ITwitterService, TwitterService>();
services.AddTwitterClient(configuration);

builder.Services.AddMemoryCache();
builder.Services.SetupHangfire(configuration.GetRequiredSection("MongoDB:ConnectionString").Value!);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(DevCORSAllowAll);
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/backJobMonitor", new DashboardOptions
{
    Authorization = [new HangfireAuthFilter()]
});

app.Run();
