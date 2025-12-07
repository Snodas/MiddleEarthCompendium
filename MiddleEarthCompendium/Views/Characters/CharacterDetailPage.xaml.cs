using MiddleEarthCompendium.ViewModels.Characters;

namespace MiddleEarthCompendium.Views.Characters;

[QueryProperty(nameof(CharacterId), "id")]
public partial class CharacterDetailPage : ContentPage
{
    private readonly CharacterDetailViewModel _viewModel;

    public string CharacterId
    {
        set => _viewModel.LoadCharacterCommand.Execute(value);
    }

    public CharacterDetailPage(CharacterDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
}