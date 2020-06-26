using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Android.Widget;
using Swetris.Annotations;
using Swetris.ViewModel;
using Xamarin.Forms;

namespace Swetris.ViewModel
{
    internal class GameViewModel : BaseViewModel
    {
        public static GameViewModel Game;
        public GameViewModel(string name)
        {
            Game = this;
            this.name = name;
        }
        private string name;

        private int score = 0;
        public int Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }

        public void UploadScore()
        {
            if (Score == 0) return;
            var values = new Dictionary<string, string>
            {
                { "Score", Score.ToString() },
                { "Id",  Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) }
            };

            var content = new FormUrlEncodedContent(values);
            try
            {
                new HttpClient().PostAsync(Properties.Resources.URL + "swetris.php", content);
            } catch(Exception ex)
            {
                
            }
        }
	}
}
