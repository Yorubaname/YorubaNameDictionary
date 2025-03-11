using Application.Services.MultiLanguage;
using YorubaOrganization.Core.Tenants;

namespace Api.Tenants
{
    public class HttpTenantProvider(ILanguageService languageService) : ITenantProvider
    {
        public string GetCurrentTenant() => languageService.CurrentTenant;
    }
}
