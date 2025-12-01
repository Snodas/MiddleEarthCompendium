using MiddleEarthCompendium.Views;

namespace MiddleEarthCompendium
{
    public partial class AppShell : Shell
    {
        public AppShell(CharactersPage charactersPage)
        {
            InitializeComponent();

            Items.Add(new ShellContent
            {
                Title = "Characters",
                Route = "characters",
                Content = charactersPage
            });
        }
    }
}
