using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class WorkJournal : ContentPage
	{
		public string issueKeySummary { get; set; }
		public string issueSummary { get; set; }
		public string issueKey { get; set; }
		private List<RTMobile.Transition> transition { get; set; }
		public ObservableCollection<Worklog> worklogs = new ObservableCollection<Worklog>();
		public WorkJournal()
		{
			InitializeComponent();
		}
		public WorkJournal(string issueKey, string issueSummary)
		{
			this.issueSummary = issueSummary;
			this.issueKey = issueKey;
			InitializeComponent();
			transitionIssue();
			issueStartPostRequest();
			this.BindingContext = this;
		}
		void ImageButton_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Calendar());
		}
		private void transitionIssue()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issueKey}/transitions/",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);

				transition = request.GetResponses<RootObject>().transitions;
				for (int i = 0; i < transition.Count; ++i)
				{
					ToolbarItem tb = new ToolbarItem
					{
						Text = transition[i].name,
						Order = ToolbarItemOrder.Secondary,
						Priority = i + 1
					};
					tb.Clicked += async (sender, args) =>
					{
						await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issueKey)).ConfigureAwait(true);
					};
					ToolbarItems.Add(tb);
				}
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
				Console.WriteLine(ex.ToString());
			}
		}
		void issueStartPostRequest()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{issueKey}/worklog",
					methodRequest = "GET",
					maxResults = 50,
					startAt = 0
				};
				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);

				rootObject = request.GetResponses<RootObject>();
				if (rootObject != null)
				{
					worklogs = rootObject.worklogs;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}

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

		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new History(issueKey, issueSummary));
		}

		void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issueKey, issueSummary));
		}


		void showHistory_Clicked(System.Object sender, System.EventArgs e)
		{

		}

		void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
		}
	}
}
