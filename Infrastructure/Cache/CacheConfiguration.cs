using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Cache
{
    public class CacheConfiguration
    {
        public int DefaultDurationMinutes { get; set; } = 60;
        public bool FailSafeEnabled { get; set; } = true;  
        public int FailSafeMaxDurationMinutes { get; set; } = 120;
        public int FactoryTimeoutSeconds { get; set; } = 30;

        public TimeSpan DefaultDuration => TimeSpan.FromMinutes(DefaultDurationMinutes);
        public TimeSpan FailSafeMaxDuration => TimeSpan.FromMinutes(FailSafeMaxDurationMinutes);
        public TimeSpan FactoryTimeout => TimeSpan.FromSeconds(FactoryTimeoutSeconds);
    }
}
