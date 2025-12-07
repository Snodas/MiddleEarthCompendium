using MiddleEarthCompendium.ViewModels;

namespace MiddleEarthCompendium.Views.Characters;

public partial class CharactersPage : ContentPage
{
	public CharactersPage(CharactersViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is CharactersViewModel vm && vm.Characters.Count == 0)
        {
            vm.LoadCharactersCommand.Execute(null);
        }
    }
}