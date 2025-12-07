using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MiddleEarthCompendium.ViewModels.Characters
{
    public partial class CharacterDetailViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;
        private readonly IWikiApiService _wikiApiService;
        private readonly Random _random = new();
        private List<Quote> _allQuotes = [];

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

        [ObservableProperty]
        private bool _hasNoImage;

        [ObservableProperty]
        private bool _hasNoBio;

        [ObservableProperty]
        private string _quotesHeader = "Quotes";

        [ObservableProperty]
        private bool _hasMoreQuotes;

        public CharacterDetailViewModel(ILotrApiService lotrApiService, IWikiApiService wikiApiService)
        {
            _lotrApiService = lotrApiService;
            _wikiApiService = wikiApiService;
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
                HasNoImage = false;
                HasNoBio = false;

                var characterTask = _lotrApiService.GetCharacterAsync(characterId);
                var quotesTask = _lotrApiService.GetCharacterQuotesAsync(characterId);

                Character = await characterTask;
                Title = Character?.Name ?? "Character";

                var wikiTask = !string.IsNullOrEmpty(Character?.WikiUrl)
                    ? _wikiApiService.GetCharacterInfoAsync(Character.WikiUrl)
                    : Task.FromResult<WikiCharacterInfo?>(null);

                _allQuotes = (await quotesTask).ToList();
                LoadRandomQuotes();

                IsBusy = false;

                var wikiInfo = await wikiTask;
                if (wikiInfo != null)
                {
                    CharacterImageUrl = wikiInfo.ImageUrl;
                    CharacterBio = wikiInfo.Bio;
                }

                HasNoImage = string.IsNullOrEmpty(CharacterImageUrl);
                HasNoBio = string.IsNullOrEmpty(CharacterBio);
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

        private void LoadRandomQuotes()
        {
            Quotes.Clear();

            if (_allQuotes.Count <= 10)
            {
                foreach (var quote in _allQuotes)
                {
                    Quotes.Add(quote);
                }
                QuotesHeader = "Quotes";
                HasMoreQuotes = false;
            }
            else
            {
                var randomQuotes = _allQuotes.OrderBy(_ => _random.Next()).Take(10);
                foreach (var quote in randomQuotes)
                {
                    Quotes.Add(quote);
                }
                QuotesHeader = "Quotes";
                HasMoreQuotes = true;
            }
        }

        [RelayCommand]
        private void ShuffleQuotes()
        {
            if (_allQuotes.Count > 10)
            {
                LoadRandomQuotes();
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
