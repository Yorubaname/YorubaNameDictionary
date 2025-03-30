using Microsoft.Extensions.DependencyInjection;
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
using Core.Repositories.Names;

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
                .AddScoped<IEtymologyRepository, NameEtymologyRepository>(); // TODO YDict: Figure out how this would work for the dictionary.

            services
                .AddScoped<IWordEntryRepository, WordRepository>()
                .AddScoped<IEntryFeedbackRepository<WordEntry>, WordFeedbackRepository>();
            // I decided to use the same Users table for the Names repository and the Words repository.
            // I also decided to use the same GeoLocationsRepository
        }
    }
}
