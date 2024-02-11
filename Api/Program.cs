using Api;
using Api.Utilities;
using Application.Domain;
using Application.Services;
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
services.AddScoped<IEventPubService, EventPubService>();


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
