using Microsoft.Extensions.DependencyInjection;
using Core.Repositories;
using Infrastructure.MongoDB.Repositories;
using YorubaOrganization.Core.Repositories;
using YorubaOrganization.Infrastructure;
using Microsoft.Extensions.Configuration;
using YorubaOrganization.Infrastructure.Repositories;

namespace Infrastructure.MongoDB
{
    public static class DependencyInjection
    {
        public static void InitializeDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<MongoDBConnections>(configuration.GetSection("MongoDB"))
                .AddSingleton<IMongoDatabaseFactory, MongoDatabaseFactory>()
                .AddScoped<INameEntryRepository, NameEntryRepository>()
                .AddScoped<IGeoLocationsRepository, GeoLocationsRepository>()
                .AddScoped<IEntryFeedbackRepository, NameEntryFeedbackRepository>()
                .AddScoped<ISuggestedNameRepository, SuggestedNameRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IEtymologyRepository, NameEtymologyRepository>();
        }
    }
}
