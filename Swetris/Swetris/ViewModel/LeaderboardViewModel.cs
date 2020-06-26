using Android.Widget;
using Swetris.Model;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;

namespace Swetris.ViewModel
{
    internal class LeaderboardViewModel : BaseViewModel
    {
        private Command _refresh;
        public Command Refresh => _refresh ??= new Command(p=>Load());
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }

        private void Load()
        {
            IsRefreshing = true;
            List.Clear();
            try
            {
                using HttpClient http = new HttpClient();
                var res = http.GetAsync(Properties.Resources.URL + "swetris.php").Result;
                if (res.IsSuccessStatusCode)
                {
                    var content = res.Content;
                    var parts = content.ReadAsStringAsync().Result.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int s = 0; s < parts.Length; s += 3)
                    {
                        List.Add(new LeaderboardEntryModel() { Name = parts[s], Score = int.Parse(parts[s + 1]) });
                    }
                }
            }
            catch (Exception ex)
            {

                Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public LeaderboardViewModel()
        {
            Load();
        }
        public ObservableCollection<LeaderboardEntryModel> List { get; } = new ObservableCollection<LeaderboardEntryModel>();
    }
}
