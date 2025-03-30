using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Api;
using Core.Repositories.Names;
using Core.StringObjectConverters;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.MongoDB.Repositories.Names;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit;
namespace Test;

public class BootStrappedApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private const int HostPort = 27018;
    private const int ContainerPort = 27017;
    private const string MongoDbDatabaseName = "yoruba_names_dictionary_test_DB";
    private const string MongoDbPassword = "password";
    private const string MongoDbUsername = "admin";

    public HttpClient HttpClient { get; private set; } = default!;

    public JsonSerializerOptions JsonSerializerOptions { get; init; }

    public BootStrappedApiFactory()
    {
        JsonSerializerOptions = JsonSerializerOptionsProvider.GetJsonSerializerOptionsWithCustomConverters();
    }

    private readonly IContainer _testDbContainer =
        new ContainerBuilder()
            .WithImage("mongo:latest")
            .WithEnvironment(new Dictionary<string, string>
            {
                {"MONGO_INITDB_ROOT_USERNAME", MongoDbUsername},
                {"MONGO_INITDB_ROOT_PASSWORD", MongoDbPassword},
                {"MONGO_INITDB_DATABASE", MongoDbDatabaseName}
            })
            .WithPortBinding(HostPort, ContainerPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(ContainerPort))
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddJsonFile("appSettings.Test.json", optional: false, reloadOnChange: true);
        });

        builder.UseEnvironment("Test");

        builder.ConfigureTestServices(x =>
        {
            x.AddSingleton<IMongoClient, MongoClient>(s => new MongoClient($"mongodb://{MongoDbUsername}:{MongoDbPassword}@localhost:{HostPort}"));
            x.AddSingleton(s => s.GetRequiredService<IMongoClient>().GetDatabase(MongoDbDatabaseName));
            x.AddSingleton<INameEntryRepository, NameEntryRepository>();
        });
    }

    public async Task InitializeAsync()
    {
        await _testDbContainer.StartAsync();
        HttpClient = CreateClient();
    }

    public new async Task DisposeAsync()
    {
        await _testDbContainer.StopAsync();
    }
}