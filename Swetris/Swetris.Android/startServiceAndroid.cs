using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Swetris.Droid;
using Swetris.Service;
using Xamarin.Forms;

[assembly: Dependency(typeof(startServiceAndroid))]
namespace Swetris.Droid
{
    public class startServiceAndroid : IStartService
    {
        public void StartForegroundServiceCompact()
        {
            var intent = new Intent(MainActivity.Instance, typeof(MyNetworkService));

            /*if(Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                MainActivity.Instance.StartForegroundService(intent);
            }
            else
            {*/
                MainActivity.Instance.StartService(intent);
            //}
        }
    }
}