namespace Website.Services.MultiLanguage
{
    public class LanguageService : ILanguageService
    {
        private const string Yoruba = "yoruba";
        private const string Igbo = "igbo";

        private static readonly Dictionary<string, string> _hostToLanguageMap = new(){
            { "yorubaname.com", Yoruba },
            { "yorubanames.com", Yoruba },
            { "igboname.com", Igbo },
            { "igbonames.com", Igbo },
        };
        private static readonly Dictionary<string, string> _languageToWebsiteMap = new()
        {
            { Yoruba, "YorubaName.com" },
            { Igbo, "IgboName.com" },
        };
        private static readonly Dictionary<string, string> _languageToLanguageDisplayMap = new()
        {
            { Yoruba, "Yorùbá" },
            { Igbo, "Igbo" },
        };
        private static readonly Dictionary<string, string> _languageToSocialNameMap = new()
        {
            { Yoruba, "YorubaNames" },
            { Igbo, "IgboNames" },
        };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _language;

        public virtual string Website => _languageToWebsiteMap[_language];
        public virtual string SocialName => _languageToSocialNameMap[_language];
        public virtual string LanguageDisplay => _languageToLanguageDisplayMap[_language];
        public bool IsYoruba => _language == Yoruba;
        public bool IsIgbo => _language == Igbo;

        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _language = GetCurrentLanguage();
        }

        private string GetCurrentLanguage()
        {
            var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
            if (host == null)
            {
                return Yoruba;
            }

            return _hostToLanguageMap.TryGetValue(WithoutSubdomain(host), out string? value) ? value : Yoruba;
        }
        private static string WithoutSubdomain(string host)
        {
            if (string.IsNullOrEmpty(host))
                return host;

            if (System.Net.IPAddress.TryParse(host, out _))
            {
                return host; // Return the IP as-is
            }

            var parts = host.Split('.');
            return parts.Length > 2 ? $"{parts[^2]}.{parts[^1]}" : host;
        }
    }
}
