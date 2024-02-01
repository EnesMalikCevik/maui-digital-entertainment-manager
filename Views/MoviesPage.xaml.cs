namespace MauiApp4.Views;
using MauiApp4.MovieLens;
using MauiApp4.ViewModels;
public partial class MoviesPage : ContentPage
{
    private readonly MovieViewModel _movieViewModel;
    private readonly MovieLensService movieLensService;
    private readonly AddMovieViewModel addMovieViewModel;
    public MoviesPage(MovieViewModel movieViewModel)
	{
        InitializeComponent();
        _movieViewModel = movieViewModel;
        BindingContext = _movieViewModel;
        this.movieLensService = MauiProgram.Current.Services.GetRequiredService<MovieLensService>();
        this.addMovieViewModel = MauiProgram.Current.Services.GetRequiredService<AddMovieViewModel>();
    }


    private async void Recommendations_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RecommendationsPage(movieLensService, _movieViewModel));
    }

    private async void AddNewButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddMoviePage(addMovieViewModel));
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ContentsPage());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _movieViewModel.LoadMoviesAsync();
    }

    private async void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        await _movieViewModel.SearchMovies();
    }

}