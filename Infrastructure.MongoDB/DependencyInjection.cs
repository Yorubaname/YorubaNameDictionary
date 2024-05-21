using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Core.Repositories;
using Infrastructure.MongoDB.Repositories;

namespace Infrastructure.MongoDB
{
    public static class DependencyInjection
    {
        public static void InitializeDatabase(this IServiceCollection services, string connectionString, string databaseName)
        {
            services.AddSingleton<IMongoClient, MongoClient>(s => new MongoClient(connectionString));
            services.AddScoped(s => s.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

            services.AddScoped<INameEntryRepository, NameEntryRepository>();
            services.AddScoped<IGeoLocationsRepository, GeoLocationsRepository>();
            services.AddScoped<INameEntryFeedbackRepository, NameEntryFeedbackRepository>();
            services.AddScoped<ISuggestedNameRepository, SuggestedNameRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
