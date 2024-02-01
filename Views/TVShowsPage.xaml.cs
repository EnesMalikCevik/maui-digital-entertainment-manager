namespace MauiApp4.Views;
using MauiApp4.ViewModels;
public partial class TVShowsPage : ContentPage
{
    public readonly TVShowViewModel _tvShowViewModel;
    public TVShowsPage(TVShowViewModel tvShowViewModel)
    {
        InitializeComponent();
        _tvShowViewModel = tvShowViewModel;
        BindingContext = _tvShowViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _tvShowViewModel.LoadTVShows();
    }
}