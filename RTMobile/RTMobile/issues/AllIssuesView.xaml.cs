using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.issues.viewIssue;
using RTMobile.notification;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues
{
	public partial class AllIssuesView : ContentPage
	{
		public ObservableCollection<Issue> issues { get; set; }
		private string filterIssue { get; set; }
		//string typeSort = "";
		public AllIssuesView()
		{
			InitializeComponent();
			filterIssue = "status not in  (Закрыта, Отклонена, Отменена, Активирована, Выполнено, 'Доставлена клиенту', Провалено) AND assignee in (currentUser())";
			issueStartPostRequest();
			if (this.issues != null && this.issues.Count > 0)
			{
				issuesList.IsVisible = true;
				noneIssue.IsVisible = false;
			}
			else
			{
				issuesList.IsVisible = false;
				noneIssue.IsVisible = true;
			}
			this.BindingContext = this;
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
		async void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			Issue selectedIssue = e.Item as Issue;
			if (selectedIssue != null)
			{
				await Navigation.PushAsync(new TabPageIssue(selectedIssue)).ConfigureAwait(true);
			}
			((ListView)sender).SelectedItem = null;
		}
		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Notification());
		}
		/// <summary>
		/// Выгрузка всех задач
		/// </summary>
		async void issueStartPostRequest()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = "/rest/api/2/search",
					methodRequest = "POST",
					jql = filterIssue,
					maxResults = "50",
					startAt = "0"
				};

				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);

				rootObject = request.GetResponses<RootObject>();

				//Проверка на пустой список задач
				try
				{
					if (issues != null)
					{
						//Очищаем список задач
						for (int i = issues.Count; i > 0; --i)
						{
							issues.RemoveAt(0);
						}
					}
					else
					{
						issues = new ObservableCollection<Issue>();
					}
					//Заполняем списком полученным при запросе
					if (rootObject != null && rootObject.issues != null)
					{
						for(int i =0; i < rootObject.issues.Count; ++i)
						{
							issues.Add(rootObject.issues[i]);
						}
					}
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.ToString(), "OK").ConfigureAwait(true);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				Crashes.TrackError(ex);
				await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
			}
		}
		private void ImageButton_Clicked_4(object sender, EventArgs e)
		{

		}
		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(721, "IT-3757")).ConfigureAwait(true);
		}

		private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
		{
			filterIssue = $"text ~ \"{searchIssue.Text}\"";
			issueStartPostRequest();
			if (this.issues != null && this.issues.Count > 0)
			{
				issuesList.IsVisible = true;
				noneIssue.IsVisible = false;
			}
			else
			{
				issuesList.IsVisible = false;
				noneIssue.IsVisible = true;
			}
		}
	}
}
