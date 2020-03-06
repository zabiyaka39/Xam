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
	public partial class History : ContentPage
	{
		public string issueKeySummary { get; set; }
		public string issueSummary { get; set; }
		public string issueKey { get; set; }
		private List<RTMobile.Transition> transition { get; set; }
		public ObservableCollection<RTMobile.History> histories { get; set; }
		public History(string issueKey, string issueSummary)
		{
			issueKeySummary = issueKey + " - " + issueSummary;
			this.issueKey = issueKey;
			this.issueSummary = issueSummary;

			InitializeComponent();

			historyIssue(issueKey);

			this.BindingContext = this;
		}
		
		private async void historyIssue(string issueKey, bool firstRequest = true)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest();
				jsonRequest.urlRequest = $"/rest/api/2/issue/{issueKey}?expand=changelog";
				jsonRequest.methodRequest = "GET";
				Request request = new Request(jsonRequest);

				RootObject historyIssues = new RootObject();
				historyIssues = request.GetResponses<RootObject>();
				//Проверяем наличие истории. Если первая то присваиваем, если обновляем, то добавляем последний элемент
				if (!firstRequest && historyIssues.changelog.histories.Count > 0)
				{
					histories.Add(historyIssues.changelog.histories[historyIssues.changelog.histories.Count - 1]);
				}
				else
				{
					histories = historyIssues.changelog.histories;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
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


		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new WorkJournal());
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
