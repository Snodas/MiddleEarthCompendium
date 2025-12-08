using MiddleEarthCompendium.ViewModels.Quotes;

namespace MiddleEarthCompendium.Views.Quotes;

public partial class QuotesPage : ContentPage
{
    private readonly QuotesViewModel _viewModel;

    public QuotesPage(QuotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Quotes.Count == 0)
        {
            await _viewModel.LoadQuotesCommand.ExecuteAsync(null);
        }
    }
}