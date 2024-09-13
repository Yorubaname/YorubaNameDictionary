using Hangfire;
using Hangfire.Mongo;
using MongoDB.Driver;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Api.Utilities;

namespace Infrastructure.Hangfire
{
    public static class DependencyInjection
    {
        public static IServiceCollection SetupHangfire(this IServiceCollection services, string mongoConnectionString)
        {
            var mongoUrlBuilder = new MongoUrlBuilder(mongoConnectionString);
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "hangfire.mongo",
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection
                })
            );

            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.ServerName = "Hangfire.Mongo server 1";
            });
            return services;
        }
        public static void UseHangfireDashboard(this IApplicationBuilder app, string dashboardPath)
        {
            app.UseHangfireDashboard(dashboardPath, new DashboardOptions
            {
                Authorization = [new HangfireAuthFilter()]
            });
        }
    }
}
