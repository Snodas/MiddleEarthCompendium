using Infrastructure.Cache;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services
{
    public class WikiApiService : IWikiApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ICacheService _cache;
        private const string FandomApiBase = "https://lotr.fandom.com/api.php";

        public WikiApiService(HttpClient httpClient, ICacheService cache)
        {
            _httpClient = httpClient;
            _cache = cache;
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("MiddleEarthCompendium/1.0 (Student Project)");
        }

        public async Task<WikiCharacterInfo?> GetCharacterInfoAsync(string wikiUrl)
        {
            if (string.IsNullOrEmpty(wikiUrl))
                return null;

            return await _cache.GetOrSetAsync(
                $"wiki_{wikiUrl.GetHashCode()}",
                async () => await FetchCharacterInfoAsync(wikiUrl)
            );
        }

        private async Task<WikiCharacterInfo?> FetchCharacterInfoAsync(string wikiUrl)
        {
            try
            {
                var title = ExtractTitleFromUrl(wikiUrl);
                if (string.IsNullOrEmpty(title))
                    return null;

                var imageUrl = await FetchImageUrlAsync(title);
                var bio = await FetchBioAsync(title);

                return new WikiCharacterInfo
                {
                    ImageUrl = imageUrl,
                    Bio = bio
                };
            }
            catch
            {
                return null;
            }
        }

        private async Task<string?> FetchImageUrlAsync(string title)
        {
            var url = $"{FandomApiBase}?action=query&titles={Uri.EscapeDataString(title)}&prop=pageimages&pithumbsize=500&format=json";
            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var pages = doc.RootElement.GetProperty("query").GetProperty("pages");

            foreach (var page in pages.EnumerateObject())
            {
                if (page.Value.TryGetProperty("thumbnail", out var thumbnail))
                {
                    var imageUrl = thumbnail.GetProperty("source").GetString();

                    if (!string.IsNullOrEmpty(imageUrl) && imageUrl.Contains("/scale-to-width-down/"))
                    {
                        imageUrl = imageUrl[..imageUrl.IndexOf("/scale-to-width-down/")];
                    }

                    return imageUrl;
                }
                break;
            }

            return null;
        }

        private async Task<string?> FetchBioAsync(string title)
        {
            var url = $"{FandomApiBase}?action=parse&page={Uri.EscapeDataString(title)}&prop=text&section=0&format=json";
            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);

            if (doc.RootElement.TryGetProperty("parse", out var parse) &&
                parse.TryGetProperty("text", out var text) &&
                text.TryGetProperty("*", out var html))
            {
                return ExtractBioFromHtml(html.GetString());
            }

            return null;
        }

        private static string? ExtractBioFromHtml(string? html)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            html = Regex.Replace(html, @"<aside[^>]*>.*?</aside>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<table[^>]*infobox[^>]*>.*?</table>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(script|style|figure)[^>]*>.*?</\1>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<sup[^>]*>.*?</sup>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            var matches = Regex.Matches(html, @"<p[^>]*>(.*?)</p>", RegexOptions.Singleline);

            foreach (Match match in matches)
            {
                var text = Regex.Replace(match.Groups[1].Value, @"<[^>]+>", "");
                text = WebUtility.HtmlDecode(text).Trim();
                text = Regex.Replace(text, @"\s+", " ");

                if (text.Length < 50)
                    continue;

                if (text.Split(',').Length > 5 && text.Length < 200)
                    continue;

                if (text.StartsWith('"') || text.StartsWith('\'') || text.StartsWith('\u201C') || text.StartsWith('\u2014'))
                    continue;

                if (text.Length > 500)
                    text = text[..500].TrimEnd() + "...";

                return text;
            }

            return null;
        }

        private static string? ExtractTitleFromUrl(string wikiUrl)
        {
            var match = Regex.Match(wikiUrl, @"/wiki/(.+)$");
            return match.Success ? Uri.UnescapeDataString(match.Groups[1].Value) : null;
        }
    }
}
