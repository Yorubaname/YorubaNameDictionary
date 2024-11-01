using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Core.Repositories;
using Infrastructure.MongoDB.Repositories;
using YorubaOrganization.Core.Repositories;

namespace Infrastructure.Database
{
    public static class DependencyInjection
    {
        public static void InitializeDatabase(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddSingleton<IMongoClient, MongoClient>(s => new MongoClient(connectionString));
            services.AddSingleton(s => s.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

            services.AddSingleton<INameEntryRepository, NameEntryRepository>();
            services.AddSingleton<IGeoLocationsRepository, GeoLocationsRepository>();
            services.AddSingleton<INameEntryFeedbackRepository, NameEntryFeedbackRepository>();
            services.AddSingleton<ISuggestedNameRepository, SuggestedNameRepository>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IEtymologyRepository, EtymologyRepository>();
        }
    }
}
