using Infrastructure.Cache;
using Infrastructure.Configuration;
using Infrastructure.Models;
using Infrastructure.Models.Responses;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class LotrApiService : ILotrApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;
        private readonly ApiOptions _apiOptions;
        private readonly JsonSerializerOptions _jsonOptions;

        public LotrApiService(
            HttpClient httpClient,
            ICacheService cache,
            IOptions<ApiOptions> apiOptions)
        {
            _httpClient = httpClient;
            _cache = cache;
            _apiOptions = apiOptions.Value;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            return await _cache.GetOrSetAsync(
                "movies_all",
                async () => await FetchAsync<Movie>("movie")
            ) ?? [];
        }

        public async Task<Movie?> GetMovieAsync(string id)
        {
            return await _cache.GetOrSetAsync(
                $"movie_{id}",
                async () =>
                {
                    var movies = await FetchAsync<Movie>($"movie/{id}");
                    return movies.FirstOrDefault();
                }
            );
        }

        public async Task<List<Quote>> GetMovieQuotesAsync(string movieId)
        {
            return await _cache.GetOrSetAsync(
                $"movie_{movieId}_quotes",
                async () => await FetchAsync<Quote>($"movie/{movieId}/quote")
            ) ?? [];
        }

        public async Task<List<Character>> GetCharactersAsync(int limit = 100)
        {
            return await _cache.GetOrSetAsync(
                $"characters_{limit}",
                async () => await FetchAsync<Character>($"character?limit={limit}")
            ) ?? [];
        }

        public async Task<List<Character>> GetCharactersByRaceAsync(string race)
        {
            return await _cache.GetOrSetAsync(
                $"characters_race_{race}",
                async () => await FetchAsync<Character>($"character?race={Uri.EscapeDataString(race)}")
            ) ?? [];
        }

        public async Task<Character?> GetCharacterAsync(string id)
        {
            return await _cache.GetOrSetAsync(
                $"character_{id}",
                async () =>
                {
                    var characters = await FetchAsync<Character>($"character/{id}");
                    return characters.FirstOrDefault();
                }
            );
        }

        public async Task<List<Quote>> GetCharacterQuotesAsync(string characterId)
        {
            return await _cache.GetOrSetAsync(
                $"character_{characterId}_quotes",
                async () => await FetchAsync<Quote>($"character/{characterId}/quote")
            ) ?? [];
        }

        public async Task<List<Quote>> GetQuotesAsync(int limit = 100)
        {
            return await _cache.GetOrSetAsync(
                $"quotes_{limit}",
                async () => await FetchAsync<Quote>($"quote?limit={limit}")
            ) ?? [];
        }

        private async Task<List<T>> FetchAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_apiOptions.BaseUrl}/{endpoint}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);

            return result?.Docs ?? [];
        }
    }
}
