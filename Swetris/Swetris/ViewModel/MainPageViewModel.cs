using System;
using System.Collections.Generic;
using System.Text;
using Swetris.Model;
using Swetris.View;

namespace Swetris.ViewModel
{
    internal class MainPageViewModel : BaseViewModel
    {
        public MainPageViewModel(LoginModel model)
        {
            _loginModel = model;
        }

        private readonly LoginModel _loginModel;
        private Command _logoutCommand;
        private Command _playCommand;
        private Command _leaderboardCommand;
        private Command _nemesisCommand;

        public string Name => _loginModel.Name;
        //App.Current.NavigationProxy.PushModalAsync()
        public Command LogoutCommand => _logoutCommand ??= new Command(p=> App.PopFromStack());
        public Command PlayCommand => _playCommand ??= new Command(p => App.PushToStack(new Game(Name)));
        public Command LeaderboardCommand => _leaderboardCommand ??= new Command(p => App.PushToStack(new Leaderboard()));
        public Command NemesisCommand => _nemesisCommand ??= new Command(p => App.PushToStack(new Nemesis()));
    }
}
