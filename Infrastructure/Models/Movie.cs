using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Models
{
    public class Movie
    {
        public string _id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int RuntimeInMinutes { get; set; }
        public double BudgetInMillions { get; set; }
        public double BoxOfficeRevenueInMillions { get; set; }
        public int AcademyAwardNominations { get; set; }
        public int AcademyAwardWins { get; set; }
        public double RottenTomatoesScore { get; set; }

        [JsonIgnore]
        public string? PosterUrl { get; set; }
    }
}
