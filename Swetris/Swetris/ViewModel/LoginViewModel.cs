using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.Widget;
using Swetris.Model;
using Swetris.View;

namespace Swetris.ViewModel
{
    internal class LoginViewModel : BaseViewModel
    {
        private string originalName;
        private bool nameInServer = false;
        private string GetName()
        {
            var values = new Dictionary<string, string>
            {
                { "Id",  Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) },
                { "get", "0" }
            };
            var content = new FormUrlEncodedContent(values);
            try
            {
                using HttpClient http = new HttpClient();
                var res = http.PostAsync(Properties.Resources.URL + "swetrisUser.php", content).Result;
                if (res.IsSuccessStatusCode)
                {
                    var con = res.Content;
                    nameInServer = true;
                    originalName = con.ReadAsStringAsync().Result;
                    return originalName;
                }
            }
            catch(Exception ex) {
                Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
            }
            return "";
        }

        public LoginViewModel()
        {
        }
        public LoginViewModel(LoginModel model)
        {
            _model = model;
            _name = _model.Name;
        }

        private readonly LoginModel _model;
        private string _name = Xamarin.Forms.Application.Current.Properties.ContainsKey("Name") ?
                                Xamarin.Forms.Application.Current.Properties["Name"]?.ToString():
                                null;
        private Command _useNameCommand;
        public bool MayClickButton => !string.IsNullOrWhiteSpace(Name);
        public string Name
        {
            get => _name??= GetName();
            set
            {
                SetProperty(ref _name, value);
                OnPropertyChanged(nameof(MayClickButton));
                UseNameCommand.OnCanExecuteChanged();
            }
        }

        public Command UseNameCommand => _useNameCommand ??= new Command(p =>
        {
            if (!nameInServer || originalName != Name)
            {
                var values = new Dictionary<string, string>
                {
                    { "Id",  Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId) },
                    { "put", "0" },
                    { "name", Name }
                };
                var content = new FormUrlEncodedContent(values);
                try
                {
                    using HttpClient http = new HttpClient();
                    var res = http.PostAsync(Properties.Resources.URL + "swetrisUser.php", content).Result;
                } catch(Exception ex) {

                    Toast.MakeText(Android.App.Application.Context, "No internet", ToastLength.Short).Show();
                }
            }
            Xamarin.Forms.Application.Current.Properties["Name"] = Name;
            Xamarin.Forms.Application.Current.SavePropertiesAsync();
            _model.Name = Name;
            App.PushToStack(new MainPage(_model));
        }, p=>MayClickButton);

    } 
}
