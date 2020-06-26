using Swetris.TetrisGame;
using Swetris.ViewModel;
using Urho;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swetris.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Game : ContentPage
    {
        public Game(string name)
        {
            var gvm = new GameViewModel(name);
            this.BindingContext = gvm;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Surface.Show<Tetris>(new ApplicationOptions(null));
        }

        protected override void OnDisappearing()
        {
            GameViewModel.Game.UploadScore();
            base.OnDisappearing();
        }
    }
}