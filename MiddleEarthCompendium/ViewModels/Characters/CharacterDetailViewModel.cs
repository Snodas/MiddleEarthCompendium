using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MiddleEarthCompendium.ViewModels.Characters
{
    public partial class CharacterDetailViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;
        private readonly IWikiScraperService _wikiScraperService;

        [ObservableProperty]
        private Character? _character;

        [ObservableProperty]
        private ObservableCollection<Quote> _quotes = [];

        [ObservableProperty]
        private string? _characterImageUrl;

        [ObservableProperty]
        private string? _characterBio;

        [ObservableProperty]
        private bool _isLoadingWiki;

        public CharacterDetailViewModel(ILotrApiService lotrApiService, IWikiScraperService wikiScraperService)
        {
            _lotrApiService = lotrApiService;
            _wikiScraperService = wikiScraperService;
        }

        [RelayCommand]
        private async Task LoadCharacterAsync(string characterId)
        {
            if (IsBusy || string.IsNullOrEmpty(characterId)) return;

            try
            {
                IsBusy = true;
                IsLoadingWiki = true;
                CharacterImageUrl = null;
                CharacterBio = null;

                var characterTask = _lotrApiService.GetCharacterAsync(characterId);
                var quotesTask = _lotrApiService.GetCharacterQuotesAsync(characterId);

                Character = await characterTask;
                Title = Character?.Name ?? "Character";

                var wikiTask = !string.IsNullOrEmpty(Character?.WikiUrl)
                    ? _wikiScraperService.GetCharacterInfoAsync(Character.WikiUrl)
                    : Task.FromResult<WikiCharacterInfo?>(null);

                var quotes = await quotesTask;
                Quotes.Clear();
                foreach (var quote in quotes)
                {
                    Quotes.Add(quote);
                }

                IsBusy = false;

                var wikiInfo = await wikiTask;
                if (wikiInfo != null)
                {
                    CharacterImageUrl = wikiInfo.ImageUrl;
                    CharacterBio = wikiInfo.Bio;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                IsLoadingWiki = false;
            }
        }

        [RelayCommand]
        private async Task OpenWikiAsync()
        {
            if (!string.IsNullOrEmpty(Character?.WikiUrl))
            {
                await Browser.Default.OpenAsync(Character.WikiUrl, BrowserLaunchMode.SystemPreferred);
            }
        }
    }
}
