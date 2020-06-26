using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Swetris.Annotations;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Swetris.ViewModel
{
    internal abstract class BaseViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            void Action() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (MainThread.IsMainThread)
                Action();
            else 
                MainThread.BeginInvokeOnMainThread(Action);
        }
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            VerifyPropertyName(propertyName);
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new Exception("Invalid property name: " + propertyName);
        }
    }
}
