using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IFusionCache _cache;
        private readonly CacheConfiguration _config;

        public CacheService(
            IFusionCache cache,
            IOptions<CacheConfiguration> config)
        {
            _cache = cache;
            _config = config.Value;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var result = await _cache.TryGetAsync<T>(key, token: cancellationToken);
            return result.HasValue ? result.Value : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            await _cache.SetAsync<T>(
                key,
                value,
                options => options.SetDuration(duration ?? _config.DefaultDuration),
                cancellationToken
            );
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? duration = null, CancellationToken cancellationToken = default)
        {
            return await _cache.GetOrSetAsync<T>(
                key,
                async ct => await factory(),
                options => options
                    .SetDuration(duration ?? _config.DefaultDuration)
                    .SetFailSafe(_config.FailSafeEnabled)
                    .SetFactoryTimeouts(_config.FactoryTimeout),
                cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key,token : cancellationToken);
        }
    }
}
