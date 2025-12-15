using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.Models;
using Infrastructure.Services.Interfaces;
using System.Collections.ObjectModel;

namespace MiddleEarthCompendium.ViewModels
{
    public partial class CharactersViewModel : BaseViewModel
    {
        private readonly ILotrApiService _lotrApiService;
        private List<Character> _currentCharacters = [];

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
            await FetchCharactersAsync();
        }

        private async Task FetchCharactersAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (SelectedRace == "All")
                {
                    _currentCharacters = await _lotrApiService.GetCharactersAsync(1000);
                }
                else
                {
                    _currentCharacters = await _lotrApiService.GetCharactersByRaceAsync(SelectedRace);
                }

                ApplyLocalFilters();
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

        private void ApplyLocalFilters()
        {
            var filtered = _currentCharacters.AsEnumerable();

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
            Task.Run(async () => await FetchCharactersAsync());
        }

        partial void OnSelectedSortChanged(string value)
        {
            if (_currentCharacters.Count > 0)
                ApplyLocalFilters();
        }

        partial void OnSearchTextChanged(string value)
        {
            if (_currentCharacters.Count > 0)
                ApplyLocalFilters();
        }
    }
}