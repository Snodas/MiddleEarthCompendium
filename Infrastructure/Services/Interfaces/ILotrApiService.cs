using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Interfaces
{
    public interface ILotrApiService
    {
        Task<List<Movie>> GetMoviesAsync();
        Task<Movie?> GetMovieAsync(string id);
        Task<List<Quote>> GetMovieQuotesAsync(string movieId);

        Task<List<Character>> GetCharactersAsync(int limit = 100);
        Task<List<Character>> GetCharactersByRaceAsync(string race);
        Task<Character?> GetCharacterAsync(string id);
        Task<List<Quote>> GetCharacterQuotesAsync(string characterId);

        Task<List<Quote>> GetQuotesAsync(int limit = 100);
    }
}
