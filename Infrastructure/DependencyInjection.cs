using Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tweetinvi;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTwitterClient(this IServiceCollection services, IConfiguration configuration)
        {
            var twitterConfig = configuration.GetRequiredSection("Twitter").Get<TwitterConfig>()!;

            services.AddSingleton(twitterConfig);

            services.AddSingleton<ITwitterClient>(provider =>
            {
                return new TwitterClient(
                    twitterConfig.ConsumerKey,
                    twitterConfig.ConsumerSecret,
                    twitterConfig.AccessToken,
                    twitterConfig.AccessTokenSecret
                );
            });

            return services;
        }
    }
}
