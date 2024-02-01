namespace MauiApp4.Views;
using MauiApp4.ViewModels;
public partial class VideoGamesPage : ContentPage
{
    private readonly VideoGameViewModel _videoGameViewModel;

    public VideoGamesPage(VideoGameViewModel videoGameViewModel)
    {
        InitializeComponent();
        _videoGameViewModel = videoGameViewModel;
        BindingContext = _videoGameViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _videoGameViewModel.LoadVideoGames();
    }
}