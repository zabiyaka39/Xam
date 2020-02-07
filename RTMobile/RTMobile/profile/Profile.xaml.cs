using System;
using System.Collections.Generic;
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
		public RootObject rootObject { get; set; }
		public List<Item> groups { get; set; }
		public Profile(string user)
		{
			InitializeComponent();

			Title = "Профиль " + issueStartPostRequest(user);

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
			CrossSettings.Current.Remove("tmpLogin");
			CrossSettings.Current.Remove("tmpPassword");
			CrossSettings.Current.Remove("CookieAuthJira");
			CrossSettings.Current.Remove("saveAuthorizationData");
			await Navigation.PopToRootAsync().ConfigureAwait(true);
		}
		string issueStartPostRequest(string user)
		{
			try
			{
				string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/user?username=" + user + @"&expand=groups,applicationRoles";
				Request request = new Request(getIssue);

				rootObject = request.GetResponsersProfile();

				groups = rootObject.groups.items;
				username.Text = user;
				exit.IsVisible = false;
				return rootObject.displayName;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return "";
			}
			return "";
		}
		void issueStartPostRequest()
		{
			try
			{
				string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/user?username=" + CrossSettings.Current.GetValueOrDefault("tmpLogin", string.Empty) + @"&expand=groups,applicationRoles";
				Request request = new Request(getIssue);

				rootObject = request.GetResponsersProfile();

				groups = rootObject.groups.items;
				username.Text = CrossSettings.Current.GetValueOrDefault("tmpLogin", string.Empty);
			}
			catch (Exception ex)
			{
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
	}
}
