using MiddleEarthCompendium.ViewModels;

namespace MiddleEarthCompendium
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
