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
        private List<Character> _allCharacters = [];

        [ObservableProperty]
        private ObservableCollection<Character> _characters = [];

        [ObservableProperty]
        private string _selectedRace = "All";

        [ObservableProperty]
        private string _selectedSort = "A-Z";

        [ObservableProperty]
        private string _searchText = string.Empty;

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

        public List<string> SortOptions { get; } =
        [
            "A-Z",
            "Z-A"
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

                if (_allCharacters.Count == 0)
                {
                    _allCharacters = await _lotrApiService.GetCharactersAsync(1000);
                }

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
            var filtered = _allCharacters.AsEnumerable();

            if (SelectedRace != "All")
            {
                filtered = filtered.Where(c => !string.IsNullOrEmpty(c.Race) &&
                                               c.Race.Equals(SelectedRace, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(c => !string.IsNullOrEmpty(c.Name) &&
                                               c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            filtered = SelectedSort == "A-Z"
                ? filtered.OrderBy(c => c.Name ?? string.Empty)
                : filtered.OrderByDescending(c => c.Name ?? string.Empty);

            Characters.Clear();
            foreach (var character in filtered)
            {
                Characters.Add(character);
            }
        }

        [RelayCommand]
        private async Task GoToDetailAsync(Character character)
        {
            if (character == null) return;

            await Shell.Current.GoToAsync($"characterdetail?id={character._id}");
        }

        partial void OnSelectedRaceChanged(string value)
        {
            if (_allCharacters.Count > 0)
                ApplyFilters();
        }

        partial void OnSelectedSortChanged(string value)
        {
            if (_allCharacters.Count > 0)
                ApplyFilters();
        }

        partial void OnSearchTextChanged(string value)
        {
            if (_allCharacters.Count > 0)
                ApplyFilters();
        }
    }
}
