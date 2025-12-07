using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configuration
{
    public class TmdbOptions
    {
        public string BaseUrl { get; set; } = "https://api.themoviedb.org/3";
        public string ImageBaseUrl { get; set; } = "https://image.tmdb.org/t/p/";
        public string ApiKey { get; set; } = string.Empty;
    }
}
