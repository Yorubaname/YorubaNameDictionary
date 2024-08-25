using Core.Dto.Response;
using Core.Entities.NameEntry;
using Core.Repositories;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Test.Integration.NameController.Data;
using Xunit;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Core.Enums;
using FluentAssertions;

namespace Test.Integration.NameController
{
    [Collection("Shared_Test_Collection")]
    public class NameControllerTest : IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly BootStrappedApiFactory _bootStrappedApiFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly INameEntryRepository _nameEntryRepository;

        public NameControllerTest(BootStrappedApiFactory bootStrappedApiFactory)
        {
            _bootStrappedApiFactory = bootStrappedApiFactory;
            _client = _bootStrappedApiFactory.HttpClient;
            _jsonSerializerOptions = _bootStrappedApiFactory.JsonSerializerOptions;

            using var scope = _bootStrappedApiFactory.Server.Services.CreateScope();
            _nameEntryRepository = scope.ServiceProvider.GetRequiredService<INameEntryRepository>();
        }

        [Theory]
        [ClassData(typeof(NamesAllTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyAllParameter(List<NameEntry> seed)
        {
            // Arrange
            await _nameEntryRepository.Create(seed);

            var url = "/api/v1/Names?all=true";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var nameEntryDtos = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(seed.Count, nameEntryDtos!.Length);
            nameEntryDtos
                .Select(dto => dto.Name)
                .Should()
                .BeEquivalentTo(seed.Select(s => s.Name));
        }

        [Theory]
        [ClassData(typeof(NamesStateTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyState(List<NameEntry> seed, State state)
        {
            // Arrange
            var filteredSeed = seed.Where(x => x.State == state).ToList();
            await _nameEntryRepository.Create(seed.ToList());
            var url = $"/api/v1/Names?state={state}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var nameEntryDtos = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(filteredSeed.Count, nameEntryDtos!.Length);
            nameEntryDtos
                .Select(dto => dto.Name)
                .Should()
                .BeEquivalentTo(filteredSeed.Select(s => s.Name));
        }

        [Theory]
        [ClassData(typeof(NamesCountTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyCount(List<NameEntry> seed, int count)
        {
            // Arrange
            await _nameEntryRepository.Create(seed);
            var url = $"/api/v1/Names?count={count}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var namesResponse = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(count, namesResponse!.Length);
            namesResponse
                .Select(dto => dto.Name)
                .Should()
                .BeSubsetOf(seed.Select(s => s.Name));
        }

        [Theory]
        [ClassData(typeof(NamesSubmittedByTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlySubmittedBy(List<NameEntry> seed, string creator)
        {
            // Arrange
            await _nameEntryRepository.Create(seed);
            var expectedData = seed.Where(x => x.CreatedBy == creator).ToList();
            var url = $"/api/v1/Names?submittedBy={creator}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var nameEntryDtos = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedData.Count, nameEntryDtos!.Length);
            nameEntryDtos
                .Select(dto => dto.Name)
                .Should()
                .Equal(expectedData.Select(ed => ed.Name));
        }

        [Theory]
        [ClassData(typeof(NamesStateAndCountTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyStateAndCount(List<NameEntry> seed, State state, int count)
        {
            // Arrange
            var filteredSeed = seed.Where(x => x.State == state).ToList();
            var expectedCount = filteredSeed.Count > count ? count : filteredSeed.Count;
            await _nameEntryRepository.Create(seed);
            var url = $"/api/v1/Names?state={state}&count={count}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var namesResponse = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedCount, namesResponse!.Length);
            namesResponse
                .Select(dto => dto.Name)
                .Should()
                .BeEquivalentTo(filteredSeed.Select(s => s.Name));
        }

        [Theory]
        [ClassData(typeof(NamesStateAndSubmittedByTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyStateAndSubmittedBy(List<NameEntry> seed, State state, string submittedBy)
        {
            // Arrange
            var filteredSeed = seed.Where(x => x.State == state && x.CreatedBy == submittedBy).ToList();
            await _nameEntryRepository.Create(seed);
            var url = $"/api/v1/Names?state={state}&submittedBy={submittedBy}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var namesResponse = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(filteredSeed.Count, namesResponse!.Length);
            namesResponse
                .Select(dto => dto.Name)
                .Should()
                .BeEquivalentTo(filteredSeed.Select(s => s.Name));
        }

        [Theory]
        [ClassData(typeof(NamesCountAndSubmittedByTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyCountAndSubmittedBy(List<NameEntry> seed, int count, string submittedBy)
        {
            // Arrange
            var filteredSeed = seed.Where(x => x.CreatedBy == submittedBy).Take(count).ToList();
            var expectedCount = filteredSeed.Count > count ? count : filteredSeed.Count;
            await _nameEntryRepository.Create(seed);
            var url = $"/api/v1/Names?count={count}&submittedBy={submittedBy}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var namesResponse = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedCount, namesResponse!.Length);
            namesResponse
                .Select(dto => dto.Name)
                .Should()
                .BeEquivalentTo(filteredSeed.Select(s => s.Name));
        }

        [Theory]
        [ClassData(typeof(NamesStateCountAndSubmittedByTestData))]
        public async Task TestGetAllNamesEndpointProvidingOnlyState_CountAndSubmittedBy(List<NameEntry> seed, State state, int count, string submittedBy)
        {
            // Arrange
            var filteredSeed = seed.Where(x => x.State == state && x.CreatedBy == submittedBy).Take(count).ToList();
            var expectedCount = filteredSeed.Count > count ? count : filteredSeed.Count;
            await _nameEntryRepository.Create(seed);
            var url = $"/api/v1/Names?state={state}&count={count}&submittedBy={submittedBy}";

            // Act
            var result = await _client.GetAsync(url);
            var responseContent = await result.Content.ReadAsStringAsync();
            var namesResponse = JsonSerializer.Deserialize<NameEntryDto[]>(responseContent, _jsonSerializerOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(expectedCount, namesResponse!.Length);
            namesResponse
                .Select(dto => dto.Name)
                .Should()
                .BeEquivalentTo(filteredSeed.Select(s => s.Name));
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _nameEntryRepository.DeleteAll();
        }
    }
}