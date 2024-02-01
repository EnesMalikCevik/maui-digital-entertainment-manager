using MauiApp4.ViewModels;

namespace MauiApp4.Views;

public partial class AddMoviePage : ContentPage
{
    private readonly AddMovieViewModel _addMovieViewModel;
    private readonly MovieViewModel _movieViewModel;
    public AddMoviePage(AddMovieViewModel addMovieViewModel)
	{
		InitializeComponent();
        _addMovieViewModel = addMovieViewModel;
        BindingContext = _addMovieViewModel;
        this._movieViewModel = MauiProgram.Current.Services.GetRequiredService<MovieViewModel>();
    }

    private async void OnMovieNameChanged(object sender, TextChangedEventArgs e)
    {
        var userInput = e.NewTextValue;
        await _addMovieViewModel.UpdateMovieSuggestions(userInput);
    }

    private async void OnMovieSelected(object sender, ItemTappedEventArgs e)
    {
        var selectedMovie = e.Item as string;
        await _addMovieViewModel.SelectMovie(selectedMovie);
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        await Task.Delay(1000);
        await Navigation.PushAsync(new MoviesPage(_movieViewModel));
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        MovieNameEntry.TextChanged -= OnMovieNameChanged;
    }

}