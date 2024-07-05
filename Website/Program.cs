using Microsoft.AspNetCore.Localization;
using ProxyKit;
using System.Globalization;
using Website.Config;
using Website.Services;

namespace Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            // Add services to the container
            services.AddLocalization();
            services.AddRazorPages()
                    .AddViewLocalization();

            services.AddProxy();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("yo") };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                // Configure localization based on query string 'lang'
                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider()
                {
                    QueryStringKey = "lang"
                });
            });

            services.AddHttpClient();
            services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
            services.AddTransient<ApiService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting(); 
            
            app.MapRazorPages();

            app.Map("/api/v1", appBuilder =>
            {
                appBuilder.RunProxy(context =>
                {
                    var externalApiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

                    return context
                        .ForwardTo(externalApiBaseUrl)
                        .Send();
                });
            });

            app.UseAuthorization();

            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("yo") };

            app.UseRequestLocalization();

            app.Run();
        }
    }
}
