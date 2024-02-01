using MauiApp4.AuthServices;
using MauiApp4.ViewModels;

namespace MauiApp4.Views;

public partial class ContentsPage : ContentPage
{

    private readonly MovieViewModel movieViewModel;
    private readonly TVShowViewModel tvshowViewModel;
    private readonly VideoGameViewModel videogameViewModel;
    private readonly AuthServiceB2C azureB2CPage;
    public ContentsPage()
    {
        InitializeComponent();
        this.movieViewModel = MauiProgram.Current.Services.GetRequiredService<MovieViewModel>();
        this.tvshowViewModel = MauiProgram.Current.Services.GetRequiredService<TVShowViewModel>();
        this.videogameViewModel = MauiProgram.Current.Services.GetRequiredService<VideoGameViewModel>();
        this.azureB2CPage = MauiProgram.Current.Services.GetRequiredService<AuthServiceB2C>();
        Task.Run(async () =>
        {
            await this.movieViewModel.LoadMoviesAsync();
            await this.tvshowViewModel.LoadTVShows();
            await this.videogameViewModel.LoadVideoGames();
        }); 
    }


    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AzureB2CPage(azureB2CPage));
    }

    private async void MovieNavBtn_Clicked(object sender, EventArgs e)
    {
        loadingIndicator.IsVisible = true;
        await Task.Delay(250);
        loadingIndicator.IsVisible = false;
        var moviesPage = new MoviesPage(this.movieViewModel);
        await Navigation.PushAsync(moviesPage);
    }

    private async void TVShowNavBtn_Clicked(object sender, EventArgs e)
    {
        loadingIndicator.IsVisible = true;
        await Task.Delay(500);
        loadingIndicator.IsVisible = false;
        var tvshowsPage = new TVShowsPage(this.tvshowViewModel);
        await Navigation.PushAsync(tvshowsPage);
    }
    private async void GameNavBtn_Clicked(object sender, EventArgs e)
    {
        loadingIndicator.IsVisible = true;
        await Task.Delay(500);
        loadingIndicator.IsVisible = false;
        var videogamesPage = new VideoGamesPage(this.videogameViewModel);
        await Navigation.PushAsync(videogamesPage);
    }
}