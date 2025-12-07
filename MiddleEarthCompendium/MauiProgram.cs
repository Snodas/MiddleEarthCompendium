using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infrastructure;
using MiddleEarthCompendium.ViewModels.Characters;
using MiddleEarthCompendium.ViewModels.Movies;
using MiddleEarthCompendium.ViewModels.Quotes;
using MiddleEarthCompendium.ViewModels;
using MiddleEarthCompendium.Views.Movies;
using MiddleEarthCompendium.Views.Characters;

namespace MiddleEarthCompendium;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ringbearer.ttf", "Ringbearer");
            });

        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream("MiddleEarthCompendium.appsettings.json");

        var configBuilder = new ConfigurationBuilder();

        if (stream != null)
        {
            configBuilder.AddJsonStream(stream);
        }

        configBuilder.AddUserSecrets(assembly, optional: true);

        var config = configBuilder.Build();

        builder.Services.AddInfrastructure(config);

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

        //viewmodels
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<CharactersViewModel>();
        builder.Services.AddTransient<CharacterDetailViewModel>();
        builder.Services.AddTransient<MoviesViewModel>();
        builder.Services.AddTransient<MovieDetailViewModel>();
        builder.Services.AddTransient<QuotesViewModel>();

        //pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<CharactersPage>();
        builder.Services.AddTransient<CharacterDetailPage>();
        builder.Services.AddTransient<MoviesPage>();
        builder.Services.AddTransient<MovieDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}