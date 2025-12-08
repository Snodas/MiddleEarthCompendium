using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Models
{
    public class QuoteDisplayModel
    {
        public string Id { get; set; } = string.Empty;
        public string Dialog { get; set; } = string.Empty;
        public string CharacterId { get; set; } = string.Empty;
        public string CharacterName { get; set; } = string.Empty;
        public string MovieId { get; set; } = string.Empty;
        public string MovieName { get; set; } = string.Empty;
    }
}
