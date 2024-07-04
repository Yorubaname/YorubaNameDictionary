using Microsoft.AspNetCore.Localization;
using System.Globalization;

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

            app.UseAuthorization();

            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("yo") };

            app.UseRequestLocalization();
            app.MapRazorPages();

            app.Run();
        }
    }
}
