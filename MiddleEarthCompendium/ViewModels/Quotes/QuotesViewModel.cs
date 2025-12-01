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

        [ObservableProperty]
        private ObservableCollection<Quote> _quotes = [];

        [ObservableProperty]
        private Quote? _randomQuote;

        public QuotesViewModel(ILotrApiService lotrApiService)
        {
            _lotrApiService = lotrApiService;
            Title = "Quotes";
        }

        [RelayCommand]
        private async Task LoadQuotesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var quotes = await _lotrApiService.GetQuotesAsync(200);

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

        [RelayCommand]
        private async Task GetRandomQuoteAsync()
        {
            try
            {
                if (Quotes.Count == 0)
                {
                    await LoadQuotesAsync();
                }

                if (Quotes.Count > 0)
                {
                    var random = new Random();
                    RandomQuote = Quotes[random.Next(Quotes.Count)];
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", ex.Message, "OK");
            }
        }

    }
}
