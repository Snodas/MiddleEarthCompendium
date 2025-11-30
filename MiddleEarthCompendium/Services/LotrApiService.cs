using MiddleEarthCompendium.Models;
using MiddleEarthCompendium.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;

namespace MiddleEarthCompendium.Services
{
    public class LotrApiService : ILotrApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IFusionCache _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public Task<Character?> GetCharacterAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Character>> GetCharactersAsync(int limit = 100)
        {
            throw new NotImplementedException();
        }

        public Task<List<Character>> GetCharactersByRaceAsync(string race)
        {
            throw new NotImplementedException();
        }

        public Task<List<Quote>> GetCharactersQuotesAsync(string characterId)
        {
            throw new NotImplementedException();
        }

        public Task<Movie?> GetMovieAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Quote>> GetMovieQuotesAsync(string movieId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Movie>> GetMoviesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Quote>> GetQuotesAsync(int limit = 100)
        {
            throw new NotImplementedException();
        }
    }
}
