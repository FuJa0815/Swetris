using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.View.Menu;
using Android.Widget;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;

namespace Swetris.Droid
{
    [Service]
    internal class MyNetworkService : Android.App.Service
    {
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;
            var channelName = "Nemesis info";
            var channelDescription = "Notifications about your nemesis beating you";
            var channel = new NotificationChannel("Swetris.Nemesis", channelName, NotificationImportance.Low) { Description = channelDescription };
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
        private void Notificate(string title, string text)
        {
            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "Swetris.Nemesis")
                .SetContentTitle(title)
                .SetContentText(text)
                .SetSmallIcon(Resource.Drawable.Logo);

            Notification notification = builder.Build();
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }

        private List<string> GetKnownBetterNemenis()
        {
            var s = Xamarin.Forms.Application.Current.Properties["KnownBetterNemesis"].ToString();
            return (JsonConvert.DeserializeObject<List<string>>(s));
        }

        private async void SetKnownBetterNemenis(List<string> value)
        {
            var s = JsonConvert.SerializeObject(value);
            Xamarin.Forms.Application.Current.Properties["KnownBetterNemesis"] = s;
            await Xamarin.Forms.Application.Current.SavePropertiesAsync();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            if (!Xamarin.Forms.Application.Current.Properties.ContainsKey("KnownBetterNemesis"))
                SetKnownBetterNemenis(new List<string>());

            Device.StartTimer(TimeSpan.FromSeconds(30), () =>
            {
                if (Xamarin.Forms.Application.Current.Properties.ContainsKey("Name")) {
                    var values = new Dictionary<string, string>
                    {
                        { "Id", Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) },
                        { "nem",  "0" },
                        { "name", Xamarin.Forms.Application.Current.Properties["Name"].ToString() }
                    };

                    var content = new FormUrlEncodedContent(values);

                    try {
                        using HttpClient http = new HttpClient();
                        var res = http.PostAsync(Properties.Resources.URL + "swetrisUser.php", content).Result;
                        if (res.IsSuccessStatusCode)
                        {
                            var con = res.Content;
                            var parts = con.ReadAsStringAsync().Result.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                            for (int s = 0; s < parts.Length; s++)
                            {
                                if (!GetKnownBetterNemenis().Contains(parts[s]))
                                {
                                    // Score beaten
                                    var temp = GetKnownBetterNemenis();
                                    temp.Add(parts[s]);
                                    SetKnownBetterNemenis(temp);

                                    Notificate("Highscore beaten", $"{parts[s]} beat your high score");
                                }
                            }
                        }
                    } catch (Exception ex)
                    {
                        Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
                    }
                }
                return true;
            });

            return base.OnStartCommand(intent, flags, startId);
        }
    }
}