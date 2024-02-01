using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using MauiApp4.AuthServices;
using MauiApp4.Views;
using MauiApp4.DatabaseContexts;
using MauiApp4.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MauiApp4.MovieLens;
using System.Security.Cryptography;
using System.Globalization;




namespace MauiApp4.ViewModels
{
    public class MovieViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Movie> _movies;
        private ObservableCollection<Movie> _originalMovies;
        public ObservableCollection<Movie> Movies
        {
            get { return _movies; }
            set { _movies = value;
                OnPropertyChanged(nameof(Movies));
                OnPropertyChanged(nameof(MoviesCountText));
            }
        }
        public string MoviesCountText => $"Total Movies: {Movies?.Count ?? 0}";

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged(nameof(SelectedMovie));
                    AreButtonsVisible = (_selectedMovie != null);
                    AreFirstButtonsVisible = (_selectedMovie == null);
                }
            }
        }

        private bool _areButtonsVisible;

        public bool AreButtonsVisible
        {
            get { return _areButtonsVisible; }
            set
            {
                if (_areButtonsVisible != value)
                {
                    _areButtonsVisible = value;
                    OnPropertyChanged(nameof(AreButtonsVisible));
                }
            }
        }

        private bool _areFirstButtonsVisible = true;

        public bool AreFirstButtonsVisible
        {
            get { return _areFirstButtonsVisible; }
            set
            {
                if (_areFirstButtonsVisible != value)
                {
                    _areFirstButtonsVisible = value;
                    OnPropertyChanged(nameof(AreFirstButtonsVisible));
                }
            }
        }


        private readonly MovieDBContext _dbcontext;
        public MovieViewModel(MovieDBContext dbcontext)
        {
            
            _dbcontext = dbcontext;
            
            Movies = new ObservableCollection<Movie>();
           
        }
    

        public async Task LoadMoviesAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var movies = await GetMoviesAsync();
            _originalMovies = new ObservableCollection<Movie>(movies);

            Movies = new ObservableCollection<Movie>(_originalMovies);
            
            stopwatch.Stop();
            long elapsedTime = stopwatch.ElapsedMilliseconds;
            Debug.WriteLine($"---Loaded movies--- time taken in ms: {elapsedTime}");

        }

        public async Task<List<Movie>> GetMoviesAsync()
        {
            var userId = Global.UserId;

            var movies = await _dbcontext.GetMoviesAsync(userId);
            return movies;
        }

        private async Task AddMovieAsync()
        {
            var newMovie = new Movie()
            {
                Rating = 5,
                UserId = Global.UserId,
                WatchDate = DateTime.Now
        };
            
            Movies.Add(newMovie);
        }


        public async Task SaveMovieAsync(Movie movie)
        {
            movie.UserId = Global.UserId;
            await _dbcontext.SaveMovieAsync(movie);
            var useridtoint = ConvertUserIdToInt(movie.UserId);
            await WriteToCsv(useridtoint, movie.MovieId, movie.Rating);
            
            SelectedMovie = null;
            AreButtonsVisible = false;
        }

        public int ConvertUserIdToInt(string userId)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] userIdBytes = Encoding.UTF8.GetBytes(userId);
                byte[] hashBytes = sha256.ComputeHash(userIdBytes);
                int userIdAsInt = BitConverter.ToInt32(hashBytes, 0);
                userIdAsInt = Math.Abs(userIdAsInt) % int.MaxValue + 1000;

                return userIdAsInt;
            }
        }

        private async Task WriteToCsv(int userId, int movieId, double rating)
        {
            try
            {
                rating /= 2.0;
                var csvFileName = "ratings.csv";
                var csvFilePath = Path.Combine(FileSystem.AppDataDirectory, csvFileName);
                Debug.WriteLine($"CSV File Path--------------------------------------------------: {csvFilePath}");
                
                if (File.Exists(csvFilePath))
                {
                    using (var writer = new StreamWriter(csvFilePath, true))
                    {
                        await writer.WriteLineAsync($"{userId},{movieId},{rating}");
                    }
                }
                else
                {
                    Debug.WriteLine("CSV file not found!--------------------------------------------------------");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing to CSV file: {ex.Message}");
            }
        }


        public bool MovieExists(int movieId)
        {
            Movies = new ObservableCollection<Movie>(_originalMovies);
            return Movies.Any(movie => movie.MovieId == movieId);
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                }
                
            }
        }
        public async Task SearchMovies()
       {
            
            if (string.IsNullOrEmpty(SearchText))
            {
                Movies = new ObservableCollection<Movie>(_originalMovies);
            }
            else
            {
                await Task.Delay(100);
                var filteredMovies =  _originalMovies
                    .Where(movie => movie.Name.StartsWith(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Movies = new ObservableCollection<Movie>(filteredMovies);
            }
        }

        public Command SearchCommand
        {
            get
            {
                return new Command(async () =>
                await SearchMovies());
            }
        }

        public Command AddNewCommand
        {
            get
            {
                return new Command(async() =>
                await AddMovieAsync());
            }
        }

        public Command SaveSelectedCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (SelectedMovie != null)
                    {
                        await SaveMovieAsync(SelectedMovie);
                      
                    }
                });
            }
        }

        public Command DeleteCommand
        {
            get
            {
                return new Command(async () =>
                {
                    
                    if (SelectedMovie != null)
                    {
                        
                        await _dbcontext.DeleteMovieAsync(SelectedMovie);
                        Movies.Remove(SelectedMovie);
                        SelectedMovie = null;

                        AreButtonsVisible = false;
                        OnPropertyChanged(nameof(MoviesCountText));
                        
                    }
                });
            }
        }

        public Command UnselectCommand
        {
            get
            {
                return new Command(() =>
                {
                    SelectedMovie = null;
                    AreButtonsVisible = false;
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