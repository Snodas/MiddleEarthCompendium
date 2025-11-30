using Infrastructure.Cache;
using Infrastructure.Configuration;
using Infrastructure.Services;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<ApiOptions>(configuration.GetSection("ApiOptions"));
            services.Configure<CacheConfiguration>(configuration.GetSection("CacheConfiguration"));

            var cacheConfig = configuration.GetSection("CacheConfiguration").Get<CacheConfiguration>()
                ?? new CacheConfiguration();

            services.AddFusionCache()
                .WithDefaultEntryOptions(new FusionCacheEntryOptions
                {
                    Duration = cacheConfig.DefaultDuration,
                    IsFailSafeEnabled = cacheConfig.FailSafeEnabled,
                    FailSafeMaxDuration = cacheConfig.FailSafeMaxDuration
                })
                .WithNewtonsoftJsonSerializer();

            services.AddSingleton<ICacheService, CacheService>();

            var apiOptions = configuration.GetSection("ApiOptions").Get<ApiOptions>();

            services.AddHttpClient<ILotrApiService, LotrApiService>(client =>
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiOptions?.ApiKey);
            });

            return services;
        }
    }
}
