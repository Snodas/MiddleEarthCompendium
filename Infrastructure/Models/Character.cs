using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models
{
    public class Character
    {
        public string _id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Race { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Birth { get; set; } = string.Empty;
        public string Death { get; set; } = string.Empty;
        public string Realm { get; set; } = string.Empty;
        public string Spouse { get; set; } = string.Empty;
        public string WikiUrl { get; set; } = string.Empty;
    }
}
