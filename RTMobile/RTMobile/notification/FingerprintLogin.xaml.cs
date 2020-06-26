using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.Fingerprint;


namespace RTMobile.notification
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FingerprintLogin : ContentPage
    {
        public FingerprintLogin()
        {
            InitializeComponent();
            getAuthAsync();

        }
        private async Task getAuthAsync()
        {
            var request = new Plugin.Fingerprint.Abstractions.AuthenticationRequestConfiguration("Prove you have fingers!", "Because without it you can't have access");
            var result = await CrossFingerprint.Current.AuthenticateAsync(request);
            if (result.Authenticated)
            {
                // do secret stuff :)
            }
            else
            {
                // not allowed to do secret stuff :(
            }
        }
    }
}