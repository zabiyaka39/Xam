﻿
using Plugin.Settings;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class authorization : ContentPage
	{
		public authorization()
		{
			InitializeComponent();
			((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Black;
			((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;
			//this.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
			//Application.Current.MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			Request request = new Request();
			if (request.authorization(login.Text.Trim(' '), password.Text))
			{
				CrossSettings.Current.AddOrUpdateValue("tmpLogin", login.Text.Trim(' '));
				CrossSettings.Current.AddOrUpdateValue("tmpPassword", password.Text);
				if (checkSaveAuthorization.IsChecked)
				{
					CrossSettings.Current.AddOrUpdateValue("login", login.Text.Trim(' '));
					CrossSettings.Current.AddOrUpdateValue("password", password.Text);

					CrossSettings.Current.AddOrUpdateValue("saveAuthorizationData", "true");
				}
				else
				{
					CrossSettings.Current.Remove("login");
					CrossSettings.Current.Remove("password");

					CrossSettings.Current.AddOrUpdateValue("saveAuthorizationData", "false");
				}
				errorAuthorization.IsVisible = false;
				//var mainPage = new IssuePage();//this could be content page
				//var rootPage = new NavigationPage(mainPage);
				//NavigationPage(new IssuePage());
				await Navigation.PushAsync(new IssuePage()).ConfigureAwait(true);
			}
			else
			{
				CrossSettings.Current.Remove("login");
				CrossSettings.Current.Remove("password");
				CrossSettings.Current.Remove("tmpLogin");
				CrossSettings.Current.Remove("tmpPassword");
				CrossSettings.Current.Remove("saveAuthorizationData");
				errorAuthorization.IsVisible = true;
			}
		}
	}
}