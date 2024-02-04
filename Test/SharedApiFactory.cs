using System.Collections.Generic;
using System.Threading.Tasks;
using Api;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Xunit;

namespace Test;

public class SharedApiFactory : WebApplicationFactory <IApiMarker>, IAsyncLifetime
{
    private const int Port = 27018;
    private readonly IContainer _testDbContainer =
        new ContainerBuilder()
            .WithImage("mongo:latest")
            .WithEnvironment( new Dictionary<string, string>
            {
                {"MONGO_INITDB_ROOT_USERNAME", "admin"},
                {"MONGO_INITDB_ROOT_PASSWORD", "password"},
                {"MONGO_INITDB_DATABASE", "yoruba_names_dictionary_test_DB"}
            })
            .WithPortBinding(27018, 27017)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(Port))
            .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(x =>
        {
            x.RemoveAll(typeof(IMongoClient));
            x.RemoveAll(typeof(IMongoDatabase));
            x.AddSingleton<IMongoClient, MongoClient>(s => new MongoClient("mongodb://localhost:27018"));
            x.AddScoped(s => s.GetRequiredService<IMongoClient>().GetDatabase("yoruba_names_dictionary_test_DB"));
        });
    }

    public async Task InitializeAsync()
    {
        await _testDbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _testDbContainer.StopAsync();
    }
}