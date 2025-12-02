using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiddleEarthCompendium.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            Title = "Middle-earth Compendium";  
        }

        [RelayCommand]
        private async Task NavigateToCharacters()
        {
            await Shell.Current.GoToAsync("//characters");
        }

        [RelayCommand]
        private async Task NavigateToMovies()
        {
            await Shell.Current.GoToAsync("//movies");
        }

        [RelayCommand]
        private async Task NavigateToQuotes()
        {
            await Shell.Current.GoToAsync("//quotes");
        }
    }
}
