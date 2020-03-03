using RTMobile.insight;
using RTMobile.calendar;
using RTMobile.settings;
using RTMobile.about;
using Xamarin.Forms;
using RTMobile.profile;
using System;
using Plugin.Settings;
using Microsoft.AppCenter.Crashes;

namespace RTMobile.issues
{
	public partial class AllIssues : MasterDetailPage
	{
		User user { get; set; }
		public AllIssues()
		{
			InitializeComponent();

			string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/user?username=" + CrossSettings.Current.GetValueOrDefault("login", string.Empty) + @"&expand=groups,applicationRoles";
			Request requestUser = new Request(getIssue);
			user = requestUser.GetResponsersProfile<User>();
			userName.Text = user.displayName;
			userEmail.Text = user.emailAddress;
			userImage.Source = user.AvatarUrls.image;
			Detail = new NavigationPage(new AllIssuesView());
		}

		void Button_Clicked(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Calendar());
			IsPresented = false;
		}

		void Button_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Insight());
			IsPresented = false;
		}

		void Button_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new CreateIssue());
			IsPresented = false;
		}

		void Button_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Settings());
			IsPresented = false;
		}

		void Button_Clicked_4(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new About());
			IsPresented = false;
		}

		void Button_Clicked_5(System.Object sender, System.EventArgs e)
		{
			IsPresented = false;
		}

		void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Profile());
			IsPresented = false;
		}

		private async void Button_Clicked_6(object sender, System.EventArgs e)
		{
			Application.Current.MainPage = new MainPage();
		}
	}
}
