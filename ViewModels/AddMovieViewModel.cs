using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp4.Models;
using MauiApp4.MovieLens;

namespace MauiApp4.ViewModels
{
    
    public class AddMovieViewModel: INotifyPropertyChanged
    {
        private readonly MovieViewModel _movieViewModel;
        private readonly MovieLensService _movieLensService;
        public Movie MovieDetails { get; set; }
        public AddMovieViewModel(MovieViewModel movieViewModel, MovieLensService movieLensService)
        {
            _movieViewModel = movieViewModel;
            _movieLensService = movieLensService;
            MovieDetails = new Movie
            {
                Name = "",
                Rating = 5,
                Comment = string.Empty,
                WatchDate = DateTime.Now
            };
            AutoCompleteSuggestions = new ObservableCollection<string>();
        }
        

        /////////Movie selection  

        private ObservableCollection<string> _autoCompleteSuggestions;
        public ObservableCollection<string> AutoCompleteSuggestions
        {
            get { return _autoCompleteSuggestions; }
            set
            {
                _autoCompleteSuggestions = value ?? new ObservableCollection<string>();
                OnPropertyChanged(nameof(AutoCompleteSuggestions));
            }
        }
        private string? _selectedMovieName;
        public string? SelectedMovieName
        {
            get { return _selectedMovieName; }
            set
            {
                if (_selectedMovieName != value)
                {
                    _selectedMovieName = value;
                    OnPropertyChanged(nameof(SelectedMovieName));
                    
                }
            }
        }
        public bool IsAutoCompleteVisible => AutoCompleteSuggestions.Any();

        


        public async Task UpdateMovieSuggestions(string userInput)
        {
            
            const int minCharsForSuggestions = 3;
            Debug.WriteLine($"UpdateMovieSuggestions - User Input: {userInput}");
            if (userInput != null)
            {
                if (userInput.Length >= minCharsForSuggestions)
                {
                    Debug.WriteLine("UpdateMovieSuggestions - Clearing suggestions.");
                    AutoCompleteSuggestions?.Clear();
                    Debug.WriteLine($"Suggestions Count After Clear: {AutoCompleteSuggestions.Count}");
                    var suggestions = await _movieLensService.GetMovieSuggestions(userInput);

                    foreach (var suggestion in suggestions)
                    {
                        AutoCompleteSuggestions.Add(suggestion);
                    }

                    OnPropertyChanged(nameof(IsAutoCompleteVisible));
                    Debug.WriteLine($"Suggestions Count: {AutoCompleteSuggestions.Count}");
                }


                else
                {
                    AutoCompleteSuggestions?.Clear();
                    Debug.WriteLine($"Suggestions Count After 'Else' Clear: {AutoCompleteSuggestions.Count}");
                    OnPropertyChanged(nameof(IsAutoCompleteVisible));
                }
            }

        }

        public async Task SelectMovie(string selectedMovieName)
        {

            SelectedMovieName = selectedMovieName;
            MovieDetails.Name = selectedMovieName;
            OnPropertyChanged(nameof(IsAutoCompleteVisible));
        }

        /////////



        public async Task LoadMoviesAsync()
        {
            await _movieViewModel.LoadMoviesAsync();
        }


        public Command SaveCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var _movie = MovieDetails;
                    var movieDetails = _movieLensService.GetMovieDetails(_movie.Name);
                    _movie.MovieId = movieDetails.MovieId;
                    await _movieViewModel.SaveMovieAsync(_movie);
                    MovieDetails = new Movie
                    {
                        Name = string.Empty,
                        Rating = 5, 
                        Comment = string.Empty, 
                        WatchDate = DateTime.Now
                    };
                    SelectedMovieName = "";
                    AutoCompleteSuggestions.Clear();
                });

            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
