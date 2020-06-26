using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swetris.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swetris.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Leaderboard : ContentPage
    {
        public Leaderboard()
        {
            this.BindingContext = new LeaderboardViewModel();
            InitializeComponent();
        }
    }
}