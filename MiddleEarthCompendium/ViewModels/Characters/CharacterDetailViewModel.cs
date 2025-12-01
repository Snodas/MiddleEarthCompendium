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

        [ObservableProperty]
        private Character? _character;

        [ObservableProperty]
        private ObservableCollection<Quote> _quotes = [];

        public CharacterDetailViewModel(ILotrApiService lotrApiService)
        {
            _lotrApiService = lotrApiService;
        }

        [RelayCommand]
        private async Task LoadCharacterAsync(string characterId)
        {
            if (IsBusy || string.IsNullOrEmpty(characterId)) return;

            try
            {
                IsBusy = true;

                Character = await _lotrApiService.GetCharacterAsync(characterId);
                Title = Character?.Name ?? "Character";

                var quotes = await _lotrApiService.GetCharacterQuotesAsync(characterId);

                Quotes.Clear();
                foreach (var quote in quotes)
                {
                    Quotes.Add(quote);
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
    }
}
