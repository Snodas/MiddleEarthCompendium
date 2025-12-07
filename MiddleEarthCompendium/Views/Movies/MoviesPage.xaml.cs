using MiddleEarthCompendium.ViewModels.Movies;

namespace MiddleEarthCompendium.Views.Movies;

public partial class MoviesPage : ContentPage
{
    private readonly MoviesViewModel _viewModel;

    public MoviesPage(MoviesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Movies.Count == 0)
        {
            await _viewModel.LoadMoviesCommand.ExecuteAsync(null);
        }
    }
}