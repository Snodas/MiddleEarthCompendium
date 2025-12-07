using MiddleEarthCompendium.Views;
using MiddleEarthCompendium.Views.Characters;
using MiddleEarthCompendium.Views.Movies;

namespace MiddleEarthCompendium
{
    public partial class AppShell : Shell
    {
        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Routing.RegisterRoute("characterdetail", typeof(CharacterDetailPage));
            Routing.RegisterRoute("moviedetail", typeof(MovieDetailPage));
        }
    }
}
