using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiApp4.Models;
using MauiApp4.DatabaseContexts;

namespace MauiApp4.ViewModels
{
    public class TVShowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<TVShow> _tvshows;
        public ObservableCollection<TVShow> TVShows
        {
            get { return _tvshows; }
            set
            {
                _tvshows = value;
                OnPropertyChanged(nameof(TVShows));

            }
        }

        private TVShow? _selectedTVShow;
        public TVShow? SelectedTVShow
        {
            get { return _selectedTVShow; }
            set
            {
                if (_selectedTVShow != value)
                {
                    _selectedTVShow = value;
                    OnPropertyChanged(nameof(SelectedTVShow));
                    AreButtonsVisible = (_selectedTVShow != null);
                    AreFirstButtonsVisible = (_selectedTVShow == null);
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




        private readonly TVShowDBContext _dbcontext;

        public TVShowViewModel(TVShowDBContext dbcontext)
        {
            _dbcontext = dbcontext;
            TVShows = new ObservableCollection<TVShow>();
        }

        public async Task LoadTVShows()
        {
            var tv_shows = await _dbcontext.GetTVShowsAsync(Global.UserId);
            if (tv_shows.Any())
            {
                TVShows = new ObservableCollection<TVShow>(tv_shows);
            }
        }


        private async Task AddTVShowAsync()
        {
            var newTVShow = new TVShow()
            {
                Rating = 5,
                UserId = Global.UserId,
                WatchDate1 = DateTime.Now,
                WatchDate2 = DateTime.Now
        };

            TVShows.Add(newTVShow);
            await _dbcontext.SaveTVShowAsync(newTVShow, Global.UserId);
        }

        public Command AddNewCommand
        {
            get
            {
                return new Command(async () =>
                await AddTVShowAsync());
            }
        }

        public Command SaveAllCommand
        {
            get
            {
                return new Command(async () =>
                await _dbcontext.SaveAllTVShowsAsync(TVShows, Global.UserId));
            }
        }

        public Command SaveSelectedCommand
        {
            get
            {
                return new Command(async () =>
                {
                    if (SelectedTVShow != null)
                    {
                        await _dbcontext.SaveTVShowAsync(SelectedTVShow, Global.UserId);

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
                    
                    if (SelectedTVShow != null)
                    {
                      
                        await _dbcontext.DeleteTVShowAsync(SelectedTVShow);
                        TVShows.Remove(SelectedTVShow);
                        SelectedTVShow = null;
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
                    SelectedTVShow = null;
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
