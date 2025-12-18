using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MiddleEarthCompendium.ViewModels.Quotes
{
    public partial class QuotesViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;

        private List<QuoteDisplayModel> _allQuotes = [];
        private Dictionary<string, string> _characterNames = [];
        private Dictionary<string, string> _movieNames = [];

        [ObservableProperty]
        private ObservableCollection<QuoteDisplayModel> _quotes = [];

        [ObservableProperty]
        private ObservableCollection<string> _movieOptions = [];

        [ObservableProperty]
        private string _selectedMovie = "All Movies";

        [ObservableProperty]
        private string _searchText = string.Empty;

        public QuotesViewModel(ILotrApiService lotrApiService)
        {
            _lotrApiService = lotrApiService;
            Title = "Quotes";
        }

        partial void OnSelectedMovieChanged(string value) => ApplyFilters();
        partial void OnSearchTextChanged(string value) => ApplyFilters();

        [RelayCommand]
        private async Task LoadQuotesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var characters = await _lotrApiService.GetCharactersAsync(1000);
                var movies = await _lotrApiService.GetMoviesAsync();

                _characterNames = characters.ToDictionary(c => c._id, c => c.Name);
                _movieNames = movies.ToDictionary(m => m._id, m => m.Name);

                MovieOptions.Clear();
                MovieOptions.Add("All Movies");
                foreach (var movie in movies.OrderBy(m => m.Name))
                {
                    MovieOptions.Add(movie.Name);
                }

                var quotes = await _lotrApiService.GetQuotesAsync(2000);

                _allQuotes = quotes.Select(q => new QuoteDisplayModel
                {
                    Id = q._id,
                    Dialog = q.Dialog,
                    CharacterId = q.Character,
                    CharacterName = _characterNames.GetValueOrDefault(q.Character, "Unknown"),
                    MovieId = q.Movie,
                    MovieName = _movieNames.GetValueOrDefault(q.Movie, "Unknown")
                }).ToList();

                ApplyFilters();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ApplyFilters()
        {
            var filtered = _allQuotes.AsEnumerable();

            if (!string.IsNullOrEmpty(SelectedMovie) && SelectedMovie != "All Movies")
            {
                filtered = filtered.Where(q => q.MovieName == SelectedMovie);
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(q =>
                    q.Dialog.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    q.CharacterName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            Quotes.Clear();
            foreach (var quote in filtered.Take(200))
            {
                Quotes.Add(quote);
            }
        }

        [RelayCommand]
        private async Task GoToCharacterAsync(QuoteDisplayModel quote)
        {
            if (quote == null || string.IsNullOrEmpty(quote.CharacterId)) return;

            await Shell.Current.GoToAsync($"characterdetail?id={quote.CharacterId}");
        }
    }
}
