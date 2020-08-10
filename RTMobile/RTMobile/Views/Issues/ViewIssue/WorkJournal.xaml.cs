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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace RTMobile.issues.viewIssue
{
	public partial class WorkJournal : ContentPage
	{
		public string issueKeySummary { get; set; }
		public string issueSummary { get; set; }
		public string issueKey { get; set; }
		public string idIssue { get; set; }
		private List<RTMobile.Transition> transition { get; set; }
		public ObservableCollection<Worklog> worklogs { get; set; }


		public WorkJournal()
		{
			InitializeComponent();
		}
		public WorkJournal(string issueKey, string issueSummary, string idIssue)
		{
			this.idIssue = idIssue;
			this.issueSummary = issueSummary;
			this.issueKey = issueKey;
			this.issueKeySummary = issueKey + " - " + issueSummary;
			InitializeComponent();
			transitionIssue();
			issueStartPostRequest();
			this.BindingContext = this;
			GoToback();

		}
		async void SendIssueClicked(System.Object sender, System.EventArgs e)
		{
			await ShereIssue();
		}
		//метод вызова диалогового окна, в котором можно выбрать способ отпраки ссылки на задачу
		public async Task ShereIssue()
		{
			await Share.RequestAsync(new ShareTextRequest
			{
				Uri = String.Format("https://sd.rosohrana.ru/browse/{0}", issueKey),
				Text = String.Format("С вами поделились задачей:\n{0} - {1}", issueKey, issueSummary),
				Title = String.Format("Вы хотите поделиться задачей: {0} - {1}", issueKey, issueSummary)
			});
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
						await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issueKey, idIssue)).ConfigureAwait(true);
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
		public void GoToback()
		{
			MessagingCenter.Subscribe<NewWorkjornal>(this, "RefreshMainPage", (sender) =>
			{
				Console.WriteLine("text");
				issueStartPostRequest();
				this.BindingContext = this;
			});
		}

		void issueStartPostRequest()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{issueKey}/worklog/",
					methodRequest = "GET",
					maxResults = "50",
					startAt = "0"
				};
				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);

				rootObject = request.GetResponses<RootObject>();
				if (rootObject != null)
				{

					if (worklogs != null)
					{
						for (int i = worklogs.Count; i > 0; --i)
						{
							worklogs.RemoveAt(0);
						}
						for (int i = 0; i < rootObject.worklogs.Count; ++i)
						{
							worklogs.Add(rootObject.worklogs[i]);
						}

					}
					else
						worklogs = rootObject.worklogs;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}

		}

		void Add_new_worklog(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new NewWorkjornal(issueKey, issueSummary, idIssue));
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
			Navigation.PushAsync(new History(issueKey, issueSummary, idIssue));
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
