using Swetris.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swetris.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Nemesis : ContentPage
    {
        public Nemesis()
        {
            this.BindingContext = new NemesisViewModel();
            InitializeComponent();
        }

        private void MenuItem_Clicked(object sender, System.EventArgs e)
        {
            (this.BindingContext as NemesisViewModel).Delete.Execute(((MenuItem)sender).CommandParameter);
        }
    }
}