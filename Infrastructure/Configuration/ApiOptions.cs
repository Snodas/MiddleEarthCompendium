using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Configuration
{
    public class ApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }
}
