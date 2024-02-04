using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Api;
using Core.Repositories;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.MongoDB.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Xunit;
namespace Test;

public abstract class BootStrappedApiFactory : WebApplicationFactory <IApiMarker>, IAsyncLifetime
{
    private const int HostPort = 27018;
    private const int ContainerPort = 27017;
    private const string MongoDbDatabaseName = "yoruba_names_dictionary_test_DB";
    private const string MongoDbPassword = "password";
    private const string MongoDbUsername = "admin";
    public HttpClient HttpClient { get; private set; } = default!;
    
    private readonly IContainer _testDbContainer =
        new ContainerBuilder()
            .WithImage("mongo:latest")
            .WithEnvironment( new Dictionary<string, string>
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
        builder.ConfigureTestServices(x =>
        {
            x.AddSingleton<IMongoClient, MongoClient>(s => new MongoClient( $"mongodb://{MongoDbUsername}:{MongoDbPassword}@localhost:{HostPort}"));
            x.AddScoped(s => s.GetRequiredService<IMongoClient>().GetDatabase(MongoDbDatabaseName));
            x.AddScoped<INameEntryRepository, NameEntryRepository>();
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