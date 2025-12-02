using HtmlAgilityPack;
using Infrastructure.Cache;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class WikiScraperService : IWikiScraperService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;

        public WikiScraperService(HttpClient httpClient, ICacheService cache)
        {
            _httpClient = httpClient;
            _cache = cache;
        }

        public async Task<WikiCharacterInfo?> GetCharacterInfoAsync(string wikiUrl)
        {
            if (string.IsNullOrEmpty(wikiUrl))
                return null;

            return await _cache.GetOrSetAsync(
                $"wiki_{wikiUrl.GetHashCode()}",
                async () => await ScrapeWikiPageAsync(wikiUrl)
            );
        }

        private async Task<WikiCharacterInfo?> ScrapeWikiPageAsync(string wikiUrl)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                
                var html = await _httpClient.GetStringAsync(wikiUrl);
                var doc = new HtmlDocument();

                doc.LoadHtml(html);

                var info = new WikiCharacterInfo
                {
                    ImageUrl = ExtractImageUrl(doc),
                    Bio = ExtractBio(doc)
                };

                return info;
            }
            catch
            {
                return null;
            }
        }

        private string? ExtractImageUrl(HtmlDocument doc)
        {
            var imageNode = doc.DocumentNode.SelectSingleNode("//aside[contains(@class, 'portable-infobox')]//img[@src]")
                            ?? doc.DocumentNode.SelectSingleNode("//figure[contains(@class, 'pi-image')]//img[@src]")
                            ?? doc.DocumentNode.SelectSingleNode("//table[contains(@class, 'infobox')]//img[@src]")
                            ?? doc.DocumentNode.SelectSingleNode("//div[@class='mw-parser-output']//img[@src]");

            if (imageNode == null)
                return null;

            var src = imageNode.GetAttributeValue("src", null);

            if (string.IsNullOrEmpty(src))
                return null;

            if (src.Contains("/scale-to-width-down/"))
            {
                var index = src.IndexOf("/scale-to-width-down/");
                src = src.Substring(0, index);
            }

            if (src.Contains("/revision/latest/"))
            {
                var index = src.IndexOf("/revision/latest/");
                src = src.Substring(0, index);
            }

            if (src.StartsWith("//"))
                src = "https:" + src;

            if (src.Contains("data:image") || src.Contains("placeholder") || src.Contains("icon"))
                return null;

            return src;
        }

        private string? ExtractBio(HtmlDocument doc)
        {
            var paragraphs = doc.DocumentNode.SelectNodes("//div[@class='mw-parser-output']/p")
                            ?? doc.DocumentNode.SelectNodes("//div[contains(@class, 'mw-parser-output')]//p")
                            ?? doc.DocumentNode.SelectNodes("//article//p")
                            ?? doc.DocumentNode.SelectNodes("//div[@id='content']//p");

            if (paragraphs == null)
                return null;

            foreach (var p in paragraphs)
            {
                if (p.Ancestors().Any(a => a.Name == "aside" || a.Name == "table" ||
                    (a.Attributes["class"]?.Value?.Contains("infobox") ?? false)))
                    continue;

                var text = HtmlEntity.DeEntitize(p.InnerText).Trim();

                if (string.IsNullOrWhiteSpace(text) || text.Length < 50)
                    continue;

                if (text.StartsWith("\"") || text.StartsWith("—") || text.StartsWith("-"))
                    continue;

                if (text.Length > 500)
                    text = text.Substring(0, 500).TrimEnd() + "...";

                return text;
            }

            return null;
        }
    }
}
