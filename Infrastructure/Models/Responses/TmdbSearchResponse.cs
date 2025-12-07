using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Models.Responses
{
    public class TmdbSearchResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbMovie>? Results { get; set; }
    }
}
