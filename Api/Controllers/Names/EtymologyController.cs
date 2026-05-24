using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using YorubaOrganization.Core.Repositories;

namespace Api.Controllers.Names
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EtymologyController : ControllerBase
    {
        private const string WordsDefinitionsInEnglishPath = "/api/v1/words/definitions/in-english";

        private readonly IEtymologyRepository _etymologyRepository;
        private readonly IHttpClientFactory _httpClientFactory;

        public EtymologyController(IEtymologyRepository etymologyRepository,
            IHttpClientFactory httpClientFactory)
        {
            _etymologyRepository = etymologyRepository;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("latest-meaning")]
        [Obsolete("This endpoint has been replaced with the GET meanings endpoint")]
        public async Task<IActionResult> GetLatestMeaning([FromQuery] string parts)
        {
            if (string.IsNullOrWhiteSpace(parts))
            {
                return BadRequest("Parts parameter is required.");
            }

            var partsList = parts.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim());
            var result = await _etymologyRepository.GetLatestMeaningOf(partsList);
            return Ok(result);
        }

        [HttpGet("meanings")]
        public async Task<IActionResult> GetMeanings([FromQuery] string parts)
        {
            if (string.IsNullOrWhiteSpace(parts))
            {
                return BadRequest("Parts parameter is required.");
            }

            var partsList = parts
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .ToArray();

            if (partsList.Length == 0)
            {
                return BadRequest("At least one part is required.");
            }

            var wordsParam = string.Join(',', partsList);
            var requestUrl = $"{WordsDefinitionsInEnglishPath}?words={Uri.EscapeDataString(wordsParam)}";

            var response = await SendProxyRequestAsync(HttpMethod.Get, requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch etymology meanings from Words API.");
            }

            var payload = await response.Content.ReadFromJsonAsync<IDictionary<string, string[]>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(payload ?? new Dictionary<string, string[]>());
        }

        [HttpPost("meanings")]
        [Authorize(Policy = "AdminAndLexicographers")]
        public async Task<IActionResult> AddMeanings([FromBody] IDictionary<string, string>? payload)
        {
            if (payload == null || payload.Count == 0)
            {
                return BadRequest("Payload must be a dictionary of part to English definition.");
            }

            var response = await SendProxyRequestAsync(HttpMethod.Post, WordsDefinitionsInEnglishPath, JsonContent.Create(payload));

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to submit new etymology meanings to Words API.");
            }

            var responsePayload = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return Ok(responsePayload ?? []);
        }

        private async Task<HttpResponseMessage> SendProxyRequestAsync(HttpMethod method, string requestUrl, HttpContent? content = null)
        {
            var client = _httpClientFactory.CreateClient("WordsApiClient");
            using var requestMessage = new HttpRequestMessage(method, requestUrl)
            {
                Content = content
            };

            ForwardAuthorizationHeader(requestMessage);
            return await client.SendAsync(requestMessage);
        }

        private void ForwardAuthorizationHeader(HttpRequestMessage requestMessage)
        {
            if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader)
                && !string.IsNullOrWhiteSpace(authorizationHeader))
            {
                requestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationHeader.ToString());
            }
        }
    }
}
