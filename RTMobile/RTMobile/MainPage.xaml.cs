﻿using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.about;
using RTMobile.issues;
using RTMobile.settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace RTMobile
{
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
			login.Text = CrossSettings.Current.GetValueOrDefault("login", "");
			password.Text = CrossSettings.Current.GetValueOrDefault("password", "");
			//ToolbarItem toolbar = new ToolbarItem
			//{
			//    Text = "Настройки",
			//    Order = ToolbarItemOrder.Primary,
			//    Priority = 0,
			//    Icon = new FileImageSource
			//    {
			//        File = "settings.png"
			//    }

			//};
			//ToolbarItems.Add(toolbar);
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			Request request = new Request();
			try
			{
				if (login.Text != null && login.Text.Length > 0 && password.Text != null && password.Text.Length > 0)
				{
					if (request.authorization(login.Text.Trim(' '), password.Text))
					{
						errorMessage.IsVisible = false;
						errorMessage1.IsVisible = false;
						errorMessage.IsVisible = false;

						CrossSettings.Current.AddOrUpdateValue("login", login.Text.Trim(' '));
						CrossSettings.Current.AddOrUpdateValue("password", password.Text);

						await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);
					}
					else
					{
						CrossSettings.Current.Remove("login");
						CrossSettings.Current.Remove("password");
						errorMessage.IsVisible = true;
						errorMessage1.IsVisible = true;
					}
				}
				else
				{
					errorMessage.IsVisible = true;
					errorMessage1.IsVisible = true;
				}
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
				Console.WriteLine(ex.ToString());
			}
		}

		private async void Button_Clicked_1(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new About()).ConfigureAwait(true);
		}

		private async void Button_Clicked_2(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Settings()).ConfigureAwait(true);
		}

		private void Button_Clicked_3(object sender, EventArgs e)
		{

		}
	}
}
