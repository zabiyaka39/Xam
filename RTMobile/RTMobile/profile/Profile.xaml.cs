using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.settings;
using Xamarin.Forms;

namespace RTMobile.profile
{
	public partial class Profile : ContentPage
	{
		public User user { get; set; }
		public Profile(string user)
		{
			InitializeComponent();

			Title = "Профиль " + issueStartPostRequest(user);
			edit.IsVisible = false;
			dataEntry.IsVisible = false;
			exit.IsVisible = false;

			this.BindingContext = this;
		}
		public Profile()
		{
			InitializeComponent();

			issueStartPostRequest();

			Title = "Мой профиль";

			this.BindingContext = this;
		}

		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			CrossSettings.Current.Remove("login");
			CrossSettings.Current.Remove("password");
			CrossSettings.Current.Remove("CookieAuthJira");
			await Navigation.PopToRootAsync().ConfigureAwait(true);
		}
		string issueStartPostRequest(string user)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = new Uri($"/rest/api/2/user?username={user}&expand=groups,applicationRoles"),
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);

				this.user = request.GetResponses<User>();
				return this.user.displayName;
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
				Console.WriteLine(ex.ToString());
				return "";
			}
			return "";
		}
		void issueStartPostRequest()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = new Uri($"/rest/api/2/user?username={CrossSettings.Current.GetValueOrDefault("login", string.Empty)}&expand=groups,applicationRoles"),
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);

				this.user = request.GetResponses<User>();
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
				Console.WriteLine(ex.ToString());
			}
		}
		void ImageButton_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Calendar());
		}

		void ImageButton_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new RTMobile.insight.Insight());
		}

		void ImageButton_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Filter());
		}

		void ImageButton_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Navigation.PopToRootAsync();
		}

		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Settings());
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			Application.Current.MainPage = new MainPage();
		}
	}
}
