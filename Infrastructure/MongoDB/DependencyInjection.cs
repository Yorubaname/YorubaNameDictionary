using Microsoft.Extensions.DependencyInjection;
using Core.Repositories;
using YorubaOrganization.Core.Repositories;
using YorubaOrganization.Infrastructure;
using Microsoft.Extensions.Configuration;
using YorubaOrganization.Infrastructure.Repositories;
using Core.Entities;
using Infrastructure.MongoDB.Repositories.Names;
using Core.Repositories.Words;
using Infrastructure.MongoDB.Repositories.Words;
using Infrastructure.MongoDB.Repositories;
using Words.Core.Entities;

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
                .AddScoped<IEntryFeedbackRepository<NameEntry>, NameEntryFeedbackRepository>()
                .AddScoped<ISuggestedNameRepository, SuggestedNameRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IEtymologyRepository, NameEtymologyRepository>();

            services
                .AddScoped<IWordEntryRepository, WordRepository>()
                .AddScoped<IEntryFeedbackRepository<WordEntry>, WordFeedbackRepository>();
        }
    }
}
