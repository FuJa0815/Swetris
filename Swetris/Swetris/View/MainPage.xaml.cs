using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swetris.Model;
using Swetris.Service;
using Swetris.ViewModel;
using Xamarin.Forms;

namespace Swetris.View
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage(LoginModel model)
        {
            this.BindingContext = new MainPageViewModel(model);
            InitializeComponent();
            DependencyService.Get<IStartService>().StartForegroundServiceCompact();
        }
    }
}
