using Microsoft.AspNetCore.Http;
using YorubaOrganization.Core.Tenants;

namespace Application.Services.MultiLanguage
{
    public class LanguageService : ILanguageService
    {
        private static readonly Dictionary<string, string> _hostToTenantMap = new(){
            { "yorubaword.com", Tenants.YorubaWord },
            { "yorubanames.com", Tenants.YorubaNames },
            { "yorubaname.com", Tenants.YorubaNames },
            { "igboname.com", Tenants.IgboNames },
            { "igbonames.com", Tenants.IgboNames },
        };
        private static readonly Dictionary<string, string> _tenantToWebsiteMap = new()
        {
            { Tenants.YorubaNames, "YorubaName.com" },
            { Tenants.YorubaWord, "YorubaWord.com" },
            { Tenants.IgboNames, "IgboName.com" },
        };
        private static readonly Dictionary<string, string> _tenantToLanguageDisplayMap = new()
        {
            { Tenants.YorubaNames, "Yorùbá" },
            { Tenants.YorubaWord, "Yorùbá" },
            { Tenants.IgboNames, "Igbo" },
        };
        private static readonly Dictionary<string, string> _tenantToSocialNameMap = new()
        {
            { Tenants.YorubaNames, "YorubaNames" },
            { Tenants.YorubaWord, "YorubaWords" },
            { Tenants.IgboNames, "IgboNames" }
        };

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _tenant;

        public virtual string Website => _tenantToWebsiteMap[_tenant];
        public virtual string SocialName => _tenantToSocialNameMap[_tenant];
        public virtual string LanguageDisplay => _tenantToLanguageDisplayMap[_tenant];
        public bool IsYoruba => _tenant == Tenants.YorubaNames;
        public bool IsIgbo => _tenant == Tenants.IgboNames;

        public string CurrentTenant => _tenant;

        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _tenant = GetCurrentTenant();
        }

        private string GetCurrentTenant()
        {
            var host = _httpContextAccessor.HttpContext?.Request.Host.Host;
            if (host == null)
            {
                return Tenants.YorubaNames;
            }

            return _hostToTenantMap.TryGetValue(WithoutSubdomain(host), out string? value) ? value : GetTenantFromHeader();
        }

        private string GetTenantFromHeader()
        {
            var currentTenant = _httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();

            return currentTenant switch
            {
                Tenants.IgboNames => Tenants.IgboNames,
                Tenants.YorubaNames => Tenants.YorubaNames,
                Tenants.YorubaWord => Tenants.YorubaWord,
                _ => Tenants.YorubaNames
            };
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
