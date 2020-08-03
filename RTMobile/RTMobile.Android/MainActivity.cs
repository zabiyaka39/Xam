using System;
using Plugin.Permissions;
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
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace RTMobile.Droid
{

	[Activity(Name = "packagename.MainActivity", Label = "RTMobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	[IntentFilter(new[] { Android.Content.Intent.ActionView },
					   AutoVerify = true,
					   Categories = new[]
					   {
							Android.Content.Intent.CategoryDefault,
							Android.Content.Intent.CategoryBrowsable
					   },
					   DataScheme = "https",
					   DataHost = "sd.rosohrana.ru")]
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
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			Rg.Plugins.Popup.Popup.Init(this, savedInstanceState);

			var action = Intent?.Action;
			var data = Intent?.Data;

	
			global::Xamarin.Forms.Forms.SetFlags("Shell_Experimental", "Visual_Experimental", "CollectionView_Experimental");
			global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

			//выбор аргумента для запуска приложения. Если приложение открывается через ссылку, App инстанцируется с текстом ссылки, которую передает в мейнпейдж
			// если пиложенизе запускается через иконку, то передается со стрoкой empty
			string linkBegin;
			linkBegin = (data != null) ? (string)data : "empty";
			LoadApplication(new App(linkBegin));


		}


		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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