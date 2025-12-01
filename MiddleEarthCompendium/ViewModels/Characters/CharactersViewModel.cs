using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MiddleEarthCompendium.ViewModels
{
    public partial class CharactersViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;

        [ObservableProperty]
        private ObservableCollection<Character> _characters = [];

        [ObservableProperty]
        private string _selectedRace = "All";

        [ObservableProperty]
        private Character? _selectedCharacter;

        public List<string> Races { get; } =
        [
            "All",
            "Human",
            "Elf",
            "Dwarf",
            "Hobbit",
            "Maiar",
            "Orc",
            "Dragon"
        ];

        public CharactersViewModel(ILotrApiService lotrApiService)
        {
            _lotrApiService = lotrApiService;
            Title = "Characters";
        }

        [RelayCommand]
        private async Task LoadCharactersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                List<Character> characters;

                if (SelectedRace == "All")
                {
                    characters = await _lotrApiService.GetCharactersAsync(500);
                }
                else
                {
                    characters = await _lotrApiService.GetCharactersByRaceAsync(SelectedRace);
                }

                Characters.Clear();
                foreach (var character in characters)
                {
                    Characters.Add(character);
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

        partial void OnSelectedRaceChanged(string value)
        {
            LoadCharactersCommand.Execute(null);
        }

    }
}
