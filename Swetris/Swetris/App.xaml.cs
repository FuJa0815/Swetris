using Android.Content;
using Swetris.Model;
using Swetris.View;
using Swetris.ViewModel;
using Xamarin.Forms;

namespace Swetris
{
    public partial class App : Application
    {
        public static void PushToStack(Page page)
        {
            Current.MainPage.Navigation.PushAsync(page);
        }

        public static void PopFromStack()
        {
            Current.MainPage.Navigation.PopAsync();
        }

        public App()
        {
            InitializeComponent();
            var l = new Login(new LoginModel());
            MainPage = new NavigationPage(l);
            var vm = (l.BindingContext as LoginViewModel);
            if (!string.IsNullOrEmpty(vm.Name))
                vm.UseNameCommand.Execute(null);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
