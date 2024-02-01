using CommunityToolkit.Maui;
using MauiApp4.AuthServices;
using MauiApp4.DatabaseContexts;
using MauiApp4.ViewModels;
using MauiApp4.Views;
using MauiApp4.MovieLens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
namespace MauiApp4;

public static class MauiProgram
{
    private static MauiApp? _current;
    public static MauiApp Current
    {
        get
        {
            if (_current == null)
            {
                throw new InvalidOperationException("MauiProgram has not been initialized.");
            }
            return _current;
        }
    }


    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


       
        builder.Services.RegisterServices();
        builder.Services.AddSingleton<MovieViewModel>();

        builder.Services.AddSingleton<RecommendationsPage>();
        builder.Services.AddSingleton<ContentsPage>();
        builder.Services.AddSingleton<MovieDBContext>();
        builder.Services.AddSingleton<TVShowDBContext>();
        builder.Services.AddSingleton<VideoGameDBContext>();
        builder.Services.AddSingleton<MoviesPage>();
        builder.Services.AddSingleton<MovieLensService>();
        builder.Services.AddSingleton<TVShowViewModel>();
        builder.Services.AddSingleton<TVShowsPage>();
        builder.Services.AddSingleton<VideoGameViewModel>();
        builder.Services.AddSingleton<VideoGamesPage>();

        builder.Services.AddSingleton<AddMoviePage>();
        builder.Services.AddSingleton<AddMovieViewModel>();
        builder.UseMauiCommunityToolkit();
        builder.Services.AddScoped<AzureB2CPage>();
        

        _current = builder.Build();

        return _current;
    }
}

