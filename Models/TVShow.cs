using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SQLite;
using Newtonsoft.Json;

namespace MauiApp4.Models;


    [Table("TVShows")]
    public class TVShow : INotifyPropertyChanged
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

        //[NotNull]
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

        private DateTime _watchdate1;

        public DateTime WatchDate1
        {
            get { return _watchdate1; }
            set
            {
                this._watchdate1 = value;
                OnPropertyChanged(nameof(WatchDate1));
            }
        }

        private DateTime _watchdate2;

        public DateTime WatchDate2
        {
            get { return _watchdate2; }
            set
            {
                this._watchdate2 = value;
                OnPropertyChanged(nameof(WatchDate2));
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