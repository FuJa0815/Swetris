using Swetris.Model;
using Swetris.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Swetris.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Login : ContentPage
    {
        public Login(LoginModel model)
        {
            var lvm = new LoginViewModel(model);
            this.BindingContext = lvm;
            InitializeComponent();
        }
    }
}