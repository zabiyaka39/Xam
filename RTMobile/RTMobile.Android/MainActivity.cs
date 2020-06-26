using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Drawing;
using Xamarin.Forms.Platform.Android;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Net.Http;
using FFImageLoading.Config;
using FFImageLoading.Forms.Platform;
using FFImageLoading;
using Service.Shared.Clients;
using Plugin.Settings;
using System.Net.Http.Headers;
using Rg.Plugins.Popup.Services;
using Plugin.Fingerprint;
using Plugin.CurrentActivity;

namespace RTMobile.Droid
{
	[Activity(Label = "RTMobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			AppCenter.Start("70eacacc-10db-4aed-98da-d55ac5b5940d",
				   typeof(Analytics), typeof(Crashes));
			AppCenter.Start("70eacacc-10db-4aed-98da-d55ac5b5940d",
							   typeof(Analytics), typeof(Crashes));
			CrossFingerprint.SetCurrentActivityResolver(() => this);
			CrossCurrentActivity.Current.Init(this, savedInstanceState);
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

			CachedImageRenderer.InitImageViewHandler();

			HttpClient httpClient = new HttpClient(new HttpLoggingHandler());

			ImageService.Instance.Initialize(new Configuration
			{
				HttpClient = httpClient,
				VerboseLogging = true				
			});
			ZXing.Net.Mobile.Forms.Android.Platform.Init();


			base.OnCreate(savedInstanceState);

			Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental");
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
			LoadApplication(new App());
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			global::ZXing.Net.Mobile.Forms.Android.Platform.Init();
		}


		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}
		public override void OnBackPressed()
		{
			if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed))
			{
				PopupNavigation.Instance.PopAsync();
			}
		}
	
	}
}