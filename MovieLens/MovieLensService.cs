using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace MauiApp4.MovieLens
{
    public class MovieLensService
    {
        private List<MovieLensMovie?> _movieLensDataset = new List<MovieLensMovie>();
        public MovieLensService()
        {
            LoadDataset();
        }

        private async Task LoadDataset()
        {
            try
            {
                var csvFileName = "movies.csv";
                using (var stream = await FileSystem.OpenAppPackageFileAsync(csvFileName))
                using (var reader = new StreamReader(stream))
                {
                    var csvContent = await reader.ReadToEndAsync();

                    _movieLensDataset = csvContent
                        .Split('\n')
                        .Skip(1)
                        .Select(line =>
                        {
                            var parts = line.Split(',');
                            if (parts.Length >= 2 && int.TryParse(parts[0], out var movieId))
                            {
                                return new MovieLensMovie
                                {
                                    MovieId = movieId,
                                    Title = parts[1]
                                };
                            }
                            return null;
                        })
                        .Where(movie => movie != null)
                        .ToList();
                }

                if (_movieLensDataset == null)
                {
                    Debug.WriteLine("_movieLensDataset is null after loading the dataset.");
                }
                else
                {
                    Debug.WriteLine($"Loaded {_movieLensDataset.Count} movies from the dataset.");
                }
            }
            catch (FormatException ex)
            {
                Debug.WriteLine($"Format exception while parsing CSV: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetMovieSuggestions(string userInput)
        {
            await Task.Delay(100);
            return _movieLensDataset
                .Where(movie => movie.Title.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
                .Select(movie => movie.Title)
                .ToList();
        }

        public MovieLensMovie GetMovieDetails(string movieName)
        {

            return _movieLensDataset.FirstOrDefault(movie => movie.Title.Equals(movieName, StringComparison.OrdinalIgnoreCase));
        }

        public string GetMovieName(int movieId)
        {
            var movie = _movieLensDataset.FirstOrDefault(m => m.MovieId == movieId);
            return movie?.Title ?? "Unknown Movie";
        }
    }



    public class MovieLensMovie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
    }
}
