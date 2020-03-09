﻿using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class People : ContentPage
	{
		public Issue issue { get; set; }
		public List<Watches> watchers { get; set; }
		private List<RTMobile.Transition> transition { get; set; }
		public People(Issue issue)
		{
			InitializeComponent();
			this.issue = issue;
			warchersIssue();

			this.BindingContext = this;
		}
		private void warchersIssue()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = new Uri($"/rest/api/2/issue/{issue.key}/watchers/"),
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);

				watchers = request.GetResponses<Watches>().watchers;
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
			Navigation.PushAsync(new Insight());
		}

		void ImageButton_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Filter());
		}

		void ImageButton_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Navigation.PopToRootAsync();
		}

		void showWatcherPeople_Clicked(System.Object sender, System.EventArgs e)
		{
			if (watcherPeople.IsVisible)
			{
				showWatcherPeople.Source = "arrowDown.png";
				watcherFrame.HeightRequest = 70;
				watcherPeople.IsVisible = false;
			}
			else
			{
				showWatcherPeople.Source = "arrowUp.png";
				watcherFrame.HeightRequest = 250;
				watcherPeople.IsVisible = true;
			}
		}

		void showGeneralPeople_Clicked(System.Object sender, System.EventArgs e)
		{
			if (generalPeople.IsVisible)
			{
				showGeneralPeople.Source = "arrowDown.png";
				generalFrame.HeightRequest = 70;
				generalPeople.IsVisible = false;
			}
			else
			{
				showGeneralPeople.Source = "arrowUp.png";
				generalFrame.HeightRequest = 210;
				generalPeople.IsVisible = true;
			}
		}
		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new History(issue.key, issue.fields.summary));
		}

		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new WorkJournal(issue.key, issue.fields.summary));
		}

		void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issue.key, issue.fields.summary));
		}


		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Profile(issue.fields.creator.name)).ConfigureAwait(true);
		}

		private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Profile(issue.fields.assignee.name)).ConfigureAwait(true);
		}

		private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			Watches selectedIssue = e.Item as Watches;
			if (selectedIssue != null)
			{
				await Navigation.PushAsync(new Profile(selectedIssue.name)).ConfigureAwait(true);
			}
			((ListView)sender).SelectedItem = null;
		}
	}
}
