using Infrastructure.Cache;
using Infrastructure.Configuration;
using Infrastructure.Models.Responses;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;

namespace Infrastructure.Services
{
    public class TmdbService : ITmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;
        private readonly TmdbOptions _options;
        private readonly JsonSerializerOptions _jsonOptions;

        private static readonly Dictionary<string, string> MovieNameMappings = new()
        {
            { "The Fellowship of the Ring", "The Lord of the Rings: The Fellowship of the Ring" },
            { "The Two Towers", "The Lord of the Rings: The Two Towers" },
            { "The Return of the King", "The Lord of the Rings: The Return of the King" },
            { "The Hobbit Series", "The Hobbit: An Unexpected Journey" },
            { "The Unexpected Journey", "The Hobbit: An Unexpected Journey" },
            { "The Desolation of Smaug", "The Hobbit: The Desolation of Smaug" },
            { "The Battle of the Five Armies", "The Hobbit: The Battle of the Five Armies" }
        };

        public TmdbService(
            HttpClient httpClient,
            ICacheService cache,
            IOptions<TmdbOptions> options)
        {
            _httpClient = httpClient;
            _cache = cache;
            _options = options.Value;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<string?> GetMoviePosterUrlAsync(string movieName, string size = "w500")
        {
            var cacheKey = $"tmdb_poster_{movieName.Replace(" ", "_")}_{size}";

            return await _cache.GetOrSetAsync(cacheKey, async () =>
            {
                var searchTerm = MovieNameMappings.TryGetValue(movieName, out var mapped)
                    ? mapped
                    : movieName;

                var encodedSearch = Uri.EscapeDataString(searchTerm);
                var url = $"{_options.BaseUrl}/search/movie?api_key={_options.ApiKey}&query={encodedSearch}";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json, _jsonOptions);

                var posterPath = result?.Results?.FirstOrDefault()?.PosterPath;
                if (string.IsNullOrEmpty(posterPath))
                    return null;

                return $"{_options.ImageBaseUrl}{size}{posterPath}";
            });
        }

        public async Task<Dictionary<string, string>> GetAllMoviePostersAsync(IEnumerable<string> movieNames, string size = "w500")
        {
            var results = new Dictionary<string, string>();
            var tasks = movieNames.Select(async name =>
            {
                var posterUrl = await GetMoviePosterUrlAsync(name, size);
                return (name, posterUrl);
            });

            var completed = await Task.WhenAll(tasks);

            foreach (var (name, posterUrl) in completed)
            {
                if (!string.IsNullOrEmpty(posterUrl))
                    results[name] = posterUrl;
            }

            return results;
        }
    }
}
