using Api;
using Api.Utilities;
using Application.Cache;
using Application.Domain;
using Application.Services;
using Core.Cache;
using Core.Events;
using Infrastructure.MongoDB;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
var services = builder.Services;

// Add services to the container.

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
    // Other configurations...
});
var mongoDbSettings = Configuration.GetSection("MongoDB");
services.InitializeDatabase(mongoDbSettings.GetValue<string>("ConnectionString"), mongoDbSettings.GetValue<string>("DatabaseName"));

services.AddScoped<NameEntryService>();
services.AddScoped<GeoLocationsService>();
services.AddScoped<NameEntryFeedbackService>();
services.AddScoped<IEventPubService, EventPubService>();
services.AddScoped<SearchService>();
services.AddScoped<SuggestedNameService>();

// TODO Hafiz: I foresee having problems with using scoped services in a singleton here. When I get there, I will cross the bridge.
services.AddSingleton<IRecentIndexesCache, RecentIndexesCache>();
services.AddSingleton<IRecentSearchesCache, RecentSearchesCache>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
