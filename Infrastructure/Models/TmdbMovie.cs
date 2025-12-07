using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Models
{
    public class TmdbMovie
    {
        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }
        
        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
