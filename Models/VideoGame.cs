using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SQLite;
using Newtonsoft.Json;

namespace MauiApp4.Models;


    [Table("GamesTest")]
    public class VideoGame : INotifyPropertyChanged
    {
        private int _id;
        [PrimaryKey, AutoIncrement]
        public int ID
        {
            get { return _id; }
            set
            {
                this._id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                this._name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private int _rating;

        public int Rating
        {
            get { return _rating; }
            set
            {
                if (value >= 1 && value <= 10)
                {
                    _rating = value;
                }
                else
                {
                    _rating = Math.Max(1, Math.Min(10, value));
                }

                OnPropertyChanged(nameof(Rating));
            }
        }
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                this._comment = value;
                OnPropertyChanged(nameof(Comment));
            }
        }

        private DateTime _playdate1;

        public DateTime PlayDate1
        {
            get { return _playdate1; }
            set
            {
                this._playdate1 = value;
                OnPropertyChanged(nameof(PlayDate1));
            }
        }

        private DateTime _playdate2;

        public DateTime PlayDate2
        {
            get { return _playdate2; }
            set
            {
                this._playdate2 = value;
                OnPropertyChanged(nameof(PlayDate2));
            }
        }


    private int _totalPlayTime;

        public int TotalPlayTime
        {
            get { return _totalPlayTime; }
            set
            {
                this._totalPlayTime = value;
                OnPropertyChanged(nameof(TotalPlayTime));
            }
        }

        public string UserId { get; set; }

        [JsonProperty("id")]
        public string CosmosId { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }

    }