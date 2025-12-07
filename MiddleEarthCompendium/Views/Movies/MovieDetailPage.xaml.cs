using MiddleEarthCompendium.ViewModels.Movies;

namespace MiddleEarthCompendium.Views.Movies;

[QueryProperty(nameof(MovieId), "id")]
public partial class MovieDetailPage : ContentPage
{
    private readonly MovieDetailViewModel _viewModel;

    public string MovieId
    {
        set => _viewModel.LoadMovieCommand.Execute(value);
    }

    public MovieDetailPage(MovieDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
}