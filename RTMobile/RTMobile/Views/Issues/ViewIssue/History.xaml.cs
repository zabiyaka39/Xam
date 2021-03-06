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
		public string idIssue { get; set; }
		public string issueSummary { get; set; }
		public string issueKey { get; set; }
		public ObservableCollection<RTMobile.History> histories { get; set; }
		public History(string issueKey, string issueSummary, string idIssue)
		{
			this.idIssue = idIssue;
			issueKeySummary = issueKey + " - " + issueSummary;
			this.issueKey = issueKey;
			this.issueSummary = issueSummary;

			InitializeComponent();

			historyIssue(issueKey);

			this.BindingContext = this;
		}

		private void historyIssue(string issueKey, bool firstRequest = true)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{

					urlRequest = $"/rest/api/2/issue/{issueKey}?expand=changelog",
					methodRequest = "GET"
				};
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
			Navigation.PushAsync(new WorkJournal(issueKey, issueSummary,idIssue));
		}

		void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issueKey, issueSummary, idIssue));
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
