using System;
using System.Collections.Generic;
using System.Text;

namespace MiddleEarthCompendium.Models.Responses
{
    public class ApiResponse<T>
    {
        public List<T> Docs { get; set; } = [];
        public int Total { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Page { get; set; }
        public int Pages { get; set; }
    }
}
