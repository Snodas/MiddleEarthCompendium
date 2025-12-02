using MiddleEarthCompendium.Views;

namespace MiddleEarthCompendium
{
    public partial class AppShell : Shell
    {
        public AppShell(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            Routing.RegisterRoute("characterdetail", typeof(CharacterDetailPage));
        }
    }
}
