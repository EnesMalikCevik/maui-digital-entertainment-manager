using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp4.DatabaseContexts;
using MauiApp4.Models;


namespace MauiApp4.ViewModels
{
    public class VideoGameViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<VideoGame> _videogames;
        public ObservableCollection<VideoGame> VideoGames
        {
            get { return _videogames; }
            set
            {
                _videogames = value;
                OnPropertyChanged(nameof(VideoGames));
            }
        }

        private VideoGame? _selectedVideoGame;
        public VideoGame? SelectedVideoGame
        {
            get { return _selectedVideoGame; }
            set
            {
                if (_selectedVideoGame != value)
                {
                    _selectedVideoGame = value;
                    OnPropertyChanged(nameof(SelectedVideoGame));
                    AreButtonsVisible = (_selectedVideoGame != null);
                    AreFirstButtonsVisible = (_selectedVideoGame == null);
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

        public List<int> Ratings { get; } = Enumerable.Range(1, 10).ToList();




        private readonly VideoGameDBContext _dbcontext;

        public VideoGameViewModel(VideoGameDBContext dbcontext)
        {
            _dbcontext = dbcontext;
            VideoGames = new ObservableCollection<VideoGame>();
        }

        public async Task LoadVideoGames()
        {
            var videogames = await _dbcontext.GetVideoGamesAsync(Global.UserId);
            if (videogames.Any())
            {
                VideoGames = new ObservableCollection<VideoGame>(videogames);
            }
        }

        private async Task AddGameAsync()
        {
            var newVideoGame = new VideoGame()
            {
                Rating = 5,
                UserId = Global.UserId,
                PlayDate1 = DateTime.Now,
                PlayDate2 = DateTime.Now
        };

            VideoGames.Add(newVideoGame);
            await _dbcontext.SaveVideoGameAsync(newVideoGame, Global.UserId);
        }

        public Command AddNewCommand
        {
            get
            {
                return new Command(async () =>
                await AddGameAsync());
            }
        }

        public Command SaveAllCommand
        {
            get
            {
                return new Command(async () =>
                await _dbcontext.SaveAllVideoGamesAsync(VideoGames, Global.UserId));
            }
        }

        public Command SaveSelectedCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (SelectedVideoGame != null)
                    {
                        await _dbcontext.SaveVideoGameAsync(SelectedVideoGame, Global.UserId);

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
                    if (SelectedVideoGame != null)
                    {

                        await _dbcontext.DeleteVideoGameAsync(SelectedVideoGame);
                        VideoGames.Remove(SelectedVideoGame);

                        SelectedVideoGame = null;
                        AreButtonsVisible = false;
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
                    SelectedVideoGame = null;
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
