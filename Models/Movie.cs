using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using SQLite;
using Newtonsoft.Json;

namespace MauiApp4.Models;

    
    [Table("MoviesTest")]
    public class Movie : INotifyPropertyChanged
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
        private double _rating;
        public double Rating
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

        private DateTime _watchdate;

        public DateTime WatchDate
        {
            get { return _watchdate; }
            set
            {
                this._watchdate = value;
                OnPropertyChanged(nameof(WatchDate));
            }
        }
        public string UserId { get; set; }
    
        public int MovieId { get; set; }

        [JsonProperty("id")]
        public string CosmosId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName)
            {
                this.PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(propertyName));
            }
}








