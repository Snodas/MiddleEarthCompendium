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
    public partial class MoviesViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;

        [ObservableProperty]
        private ObservableCollection<Movie> _movies = [];

        public MoviesViewModel(ILotrApiService lotrApiService)
        {
            _lotrApiService = lotrApiService;
            Title = "Movies";
        }

        [RelayCommand]
        private async Task LoadMoviesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var movies = await _lotrApiService.GetMoviesAsync();

                Movies.Clear();
                foreach (var movie in movies.OrderBy(m => m.Name))
                {
                    Movies.Add(movie);
                }
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

        [RelayCommand]
        private async Task GoToMovieDetailAsync(Movie movie)
        {
            if (movie == null) return;

            await Shell.Current.GoToAsync($"moviedetail?id={movie._id}");
        }
    }
}

