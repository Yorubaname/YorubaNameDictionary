using Application.Services.MultiLanguage;
using Core.StringObjectConverters;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using ProxyKit;
using System.Globalization;
using Words.Website.Config;
using Words.Website.Middleware;
using Words.Website.Resources;
using Words.Website.Services;

namespace Words.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container
            services.AddSingleton(JsonSerializerOptionsProvider.GetJsonSerializerOptionsWithCustomConverters());
            services.AddLocalization();

            services.AddProxy();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("yo") };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider()
                {
                    QueryStringKey = "lang"
                });

                options.RequestCultureProviders.Insert(1, new CookieRequestCultureProvider
                {
                    CookieName = "culture"
                });
            });

            services.AddRazorPages().AddViewLocalization();

            services.AddHttpClient();

            services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
            services.AddTransient<ApiService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error/500");
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapRazorPages();

            // Proxy API requests
            app.Map("/api/v1", appBuilder =>
            {
                appBuilder.RunProxy(context =>
                {
                    var externalApiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

                    return context
                        .ForwardTo(externalApiBaseUrl)
                        .AddXForwardedHeaders()
                        .Send();
                });
            });

            app.UseRouting();
            app.UseRequestLocalization();
            app.UseMiddleware<LocalizationCookieMiddleware>();

            app.UseAuthorization();

            app.Run();
        }
    }
}