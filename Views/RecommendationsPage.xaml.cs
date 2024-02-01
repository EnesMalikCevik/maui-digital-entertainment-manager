using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using MauiApp4.MovieLens;
using MauiApp4.ViewModels;
using Microsoft.ML;

namespace MauiApp4.Views;

public partial class RecommendationsPage : ContentPage
{

    private ObservableCollection<RecommendationItem> recommendationsList = new ObservableCollection<RecommendationItem>();
    private readonly MovieLensService _movieLensService;
    private readonly MovieViewModel _movieViewModel;
    public RecommendationsPage(MovieLensService movieLensService, MovieViewModel movieViewModel)
    {
        InitializeComponent();
        _movieLensService = movieLensService;
        _movieViewModel = movieViewModel;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        DisplayTopRecommendations();
        recommendationsCollectionView.ItemsSource = recommendationsList;

        stopwatch.Stop();
        long elapsedTime = stopwatch.ElapsedMilliseconds;
        Debug.WriteLine($"---Loaded recommended movies--- time taken in ms: {elapsedTime}");
    }

    private void DisplayTopRecommendations()
    {
        var userIdString = Global.UserId;
        var userId = _movieViewModel.ConvertUserIdToInt(userIdString);

        var topRecommendations = MovieRecommenderModel.GetTopMovieRecommendations(userId);
        
        foreach (var recommendation in topRecommendations)
        {

            var movieId = (int)recommendation.MovieId;
            if (!_movieViewModel.MovieExists(movieId))
            {
                
                var movieName = _movieLensService.GetMovieName(movieId);
                var predictedScore = recommendation.Score;
                predictedScore = 2 * predictedScore;
                recommendationsList.Add(new RecommendationItem { MovieName = movieName, PredictedScore = predictedScore, IsRatingVisible = false });
            }
        }
    }
    private void OnToggleButtonClicked(object sender, EventArgs e)
    {
        foreach (var recommendation in recommendationsList)
        {
            recommendation.IsRatingVisible = !recommendation.IsRatingVisible;
        }
    }
}



public class RecommendationItem : INotifyPropertyChanged
{
    private bool _isRatingVisible;

    public string MovieName { get; set; }
    public double PredictedScore { get; set; }

    public bool IsRatingVisible
    {
        get { return _isRatingVisible; }
        set
        {
            if (_isRatingVisible != value)
            {
                _isRatingVisible = value;
                OnPropertyChanged(nameof(IsRatingVisible));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

