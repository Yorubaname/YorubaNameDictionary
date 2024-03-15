using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Enums;
using Core.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Test.Integration.NameController.Data;
using Xunit;

namespace Test.Integration.NameController;

[Collection("Shared_Test_Collection")]
public class NameControllerTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly BootStrappedApiFactory _bootStrappedApiFactory;
    
    public NameControllerTest(BootStrappedApiFactory bootStrappedApiFactory)
    {
        _bootStrappedApiFactory = bootStrappedApiFactory;
        _client = _bootStrappedApiFactory.HttpClient;
    }

    [Theory]
    [ClassData(typeof(NamesAllTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyAllParameter(List<NameEntry> seed)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        await nameEntryRepository.Create(seed);
        var url = "/api/v1/Names?all=true";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(seed.Count(), nameEntryDtos!.Length);
        for (int i = 0; i < seed.Count; i++)
        {
            Assert.Equal(seed[i].Name, nameEntryDtos[i].Name);
        }
    }

    [Theory]
    [ClassData(typeof(NamesStateTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyState(List<NameEntry> seed, State state)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        var filteredSeed = seed.Where(x => x.State == state).ToList();
        await nameEntryRepository.Create(seed);
        var url = $"/api/v1/Names?state={state}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(filteredSeed.Count, nameEntryDtos!.Length);
        for (int i = 0; i < filteredSeed.Count; i++)
        {
            Assert.Equal(filteredSeed[i].Name, nameEntryDtos[i].Name);
        }
    }
    
    [Theory]
    [ClassData(typeof(NamesCountTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyCount(List<NameEntry> seed, int count)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        await nameEntryRepository.Create(seed);
        var expectedCount = seed.Count > count ? count : seed.Count;
        var url = $"/api/v1/Names?count={count}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expectedCount, nameEntryDtos!.Length);
        for (int i = 0; i < expectedCount; i++)
        {
            Assert.Equal(seed[i].Name, nameEntryDtos[i].Name);
        }
    }

    [Theory]
    [ClassData(typeof(NamesSubmittedByTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlySubmittedBy(List<NameEntry> seed, string creator)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        await nameEntryRepository.Create(seed);
        var expectedData = seed.Where(x => x.CreatedBy == creator).ToList();
        var url = $"/api/v1/Names?submittedBy={creator}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expectedData.Count, nameEntryDtos!.Length);
        for (int i = 0; i < expectedData.Count(); i++)
        {
            Assert.Equal(expectedData[i].Name, nameEntryDtos[i].Name);
        }
    }
    
    [Theory]
    [ClassData(typeof(NamesStateAndCountTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyStateAndCount(List<NameEntry> seed, State state, int count)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        var filteredSeed = seed.Where(x => x.State == state).ToList();
        var expectedCount = filteredSeed.Count > count ? count : filteredSeed.Count;
        await nameEntryRepository.Create(seed);
        var url = $"/api/v1/Names?state={state}&count={count}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expectedCount, nameEntryDtos!.Length);
        for (int i = 0; i < expectedCount; i++)
        {
            Assert.Equal(filteredSeed[i].Name, nameEntryDtos[i].Name);
        }
    }
    
    [Theory]
    [ClassData(typeof(NamesStateAndSubmittedByTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyStateAndSubmittedBy(List<NameEntry> seed, State state, string submittedBy)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        var filteredSeed = seed.Where(x => x.State == state && x.CreatedBy == submittedBy).ToList();
        await nameEntryRepository.Create(seed);
        var url = $"/api/v1/Names?state={state}&submittedBy={submittedBy}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(filteredSeed.Count, nameEntryDtos!.Length);
        for (int i = 0; i < filteredSeed.Count; i++)
        {
            Assert.Equal(filteredSeed[i].Name, nameEntryDtos[i].Name);
        }
    }
    
    [Theory]
    [ClassData(typeof(NamesCountAndSubmittedByTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyCountAndSubmittedBy(List<NameEntry> seed, int count, string submittedBy)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        var filteredSeed = seed.Where(x => x.CreatedBy == submittedBy).Take(count).ToList();
        var expectedCount = filteredSeed.Count > count ? count : filteredSeed.Count;
        await nameEntryRepository.Create(seed);
        var url = $"/api/v1/Names?count={count}&submittedBy={submittedBy}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expectedCount, nameEntryDtos!.Length);
        for (int i = 0; i < expectedCount; i++)
        {
            Assert.Equal(filteredSeed[i].Name, nameEntryDtos[i].Name);
        }
    }
    
    [Theory]
    [ClassData(typeof(NamesStateCountAndSubmittedByTestData))]
    public async Task TestGetAllNamesEndpointProvidingOnlyState_CountAndSubmittedBy(List<NameEntry> seed, State state, int count, string submittedBy)
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        // Arrange
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        var filteredSeed = seed.Where(x => x.State == state && x.CreatedBy == submittedBy).Take(count).ToList();
        var expectedCount = filteredSeed.Count > count ? count : filteredSeed.Count;
        await nameEntryRepository.Create(seed);
        var url = $"/api/v1/Names?state={state}&count={count}&submittedBy={submittedBy}";
        // Act
        var result = await _client.GetAsync(url);
        var responseContent = await result.Content.ReadAsStringAsync();
        var nameEntryDtos = JsonConvert.DeserializeObject<NameEntryDto[]>(responseContent);
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(expectedCount, nameEntryDtos!.Length);
        for (int i = 0; i < expectedCount; i++)
        {
            Assert.Equal(filteredSeed[i].Name, nameEntryDtos[i].Name);
        }
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
        var nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        await nameEntryRepository.DeleteAll();
    }
} 

