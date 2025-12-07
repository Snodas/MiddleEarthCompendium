using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Interfaces
{
    public interface ITmdbService
    {
        Task<string?> GetMoviePosterUrlAsync(string movieName, string size = "w500");
        Task<Dictionary<string, string>> GetAllMoviePostersAsync(IEnumerable<string> movieNames, string size = "w500");
    }
}
