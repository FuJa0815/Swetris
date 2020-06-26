using Android.App;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;

namespace Swetris.ViewModel
{
    internal class NemesisViewModel : BaseViewModel
    {
        public NemesisViewModel()
        {
            var values = new Dictionary<string, string>
            {
                { "Id",  Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) }
            };
            var content = new FormUrlEncodedContent(values);

            try
            {
                using HttpClient http = new HttpClient();
                var res = http.PostAsync(Properties.Resources.URL + "swetrisUser.php", content).Result;
                if (res.IsSuccessStatusCode)
                {
                    var con = res.Content;
                    var parts = con.ReadAsStringAsync().Result.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int s = 0; s < parts.Length; s++)
                    {
                        List.Add(parts[s]);
                    }
                }
            } catch(Exception ex)
            {
                Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
            }
        }

        private string _searchText;
        private Command _add;
        private Command _delete;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredList));
            }
        }

        public List<string> List { get; } = new List<string>();
        public List<string> FilteredList => List.Where(p=>p.ToLower().Contains((SearchText??"").ToLower())).ToList();
        public Command Add => _add ??= new Command(p=>
        {
            // Add nemesis
            var values = new Dictionary<string, string>
            {
                { "Id",  Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) },
                { "OtherName", SearchText },
                { "Act", "in" }
            };
            var content = new FormUrlEncodedContent(values);
            try
            {
                using HttpClient http = new HttpClient();
                var res = http.PostAsync(Properties.Resources.URL + "swetrisUser.php", content).Result;
                if (res.IsSuccessStatusCode)
                {
                    var con = res.Content;
                    if (con.ReadAsStringAsync().Result == "err")
                    {
                        Toast.MakeText(Application.Context, "Unknown User", ToastLength.Short).Show();
                    }
                    else
                    {
                        List.Add(SearchText);
                    }
                }
            } catch(Exception ex)
            {
                Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
            }
            OnPropertyChanged(nameof(List));
            OnPropertyChanged(nameof(FilteredList));

        });
        public Command Delete => _delete ??= new Command(p=>
        {
            // Delete nemesis
            var values = new Dictionary<string, string>
            {
                { "Id",  Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) },
                { "OtherName", p.ToString() },
                { "Act", "del" }
            };
            var content = new FormUrlEncodedContent(values);
            try
            {
                using HttpClient http = new HttpClient();
                var res = http.PostAsync(Properties.Resources.URL + "swetrisUser.php", content).Result;
                if (res.IsSuccessStatusCode)
                {
                    var con = res.Content;
                    if (con.ReadAsStringAsync().Result == "err")
                    {
                        Toast.MakeText(Application.Context, "Unknown User", ToastLength.Short);
                    }
                    else
                    {
                        List.Remove(p.ToString());
                    }
                }
            } catch(Exception ex)
            {
                Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
            }
            OnPropertyChanged(nameof(List));
            OnPropertyChanged(nameof(FilteredList));
        });
    }
}
