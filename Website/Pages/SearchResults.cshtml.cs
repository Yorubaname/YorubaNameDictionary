using Application.Services;
using Core.Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Website.Pages.Shared;
using Website.Resources;
using Website.Services;

namespace Website.Pages
{
    public class SearchResultsModel(
        IStringLocalizer<Messages> localizer, 
        ApiService apiService, JsonSerializerOptions jsonSerializerOptions) : BasePageModel(localizer)
    {
        private readonly ApiService _apiService = apiService;
        private readonly JsonSerializerOptions _jsonSerializerOptions = jsonSerializerOptions;

        [BindProperty(SupportsGet = true)]
        [FromQuery(Name = "q")]
        public string? Query { get; set; } = null;
        public NameEntryDto[] Names { get; set; } = [];
        public List<string> Alphabets { get; private set; } = [];

        public async Task<IActionResult> OnGet()
        {
            if (Query == null)
            {
                // TODO: Create an event to indicate that this page was accessed without a query parameter.
                return RedirectToPage("Index");
            }

            Names = await _apiService.SearchNameAsync(Query);

            if (Names.Length == 1 && IsEqualWithoutAccent(Names[0].Name, Query))
            {
                // var nameQuery = System.Net.WebUtility.UrlEncode(Query);
                TempData["Name"] = JsonSerializer.Serialize(Names[0]);
                return RedirectToPage("SingleEntry", new { nameEntry = Query });
            }

            Alphabets = YorubaAlphabetService.YorubaAlphabets;
            return Page();
        }

        private static bool IsEqualWithoutAccent(string string1, string string2)
        {
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            int result = compareInfo.Compare(string1, string2, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
            return result == 0;
        }
    }
}
