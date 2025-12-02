using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services.Interfaces
{
    public interface IWikiScraperService
    {
        Task<WikiCharacterInfo?> GetCharacterInfoAsync(string wikiUrl);
    }
}
