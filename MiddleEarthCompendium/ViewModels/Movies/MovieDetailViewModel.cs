using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MiddleEarthCompendium.ViewModels.Movies
{
    public partial class MovieDetailViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;
        private readonly ITmdbService _tmdbService;

        [ObservableProperty]
        private Movie? _movie;

        [ObservableProperty]
        private ObservableCollection<Quote> _quotes = [];

        [ObservableProperty]
        private string? _posterUrl;

        public MovieDetailViewModel(ILotrApiService lotrApiService, ITmdbService tmdbService)
        {
            _lotrApiService = lotrApiService;
            _tmdbService = tmdbService;
        }

        [RelayCommand]
        private async Task LoadMovieAsync(string movieId)
        {
            if (IsBusy || string.IsNullOrEmpty(movieId)) return;

            try
            {
                IsBusy = true;

                Movie = await _lotrApiService.GetMovieAsync(movieId);
                Title = Movie?.Name ?? "Movie";

                // Fetch poster
                if (Movie != null)
                {
                    PosterUrl = await _tmdbService.GetMoviePosterUrlAsync(Movie.Name, "w500");
                }

                var quotes = await _lotrApiService.GetMovieQuotesAsync(movieId);

                Quotes.Clear();
                foreach (var quote in quotes)
                {
                    Quotes.Add(quote);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
