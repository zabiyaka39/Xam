using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		IssueJSONSearch issueJSONSearch = new IssueJSONSearch();
		private string filterIssue { get; set; }
		string typeSort = "";
		public AllIssuesView()
		{
			InitializeComponent();
			filterIssue = "status not in  (Закрыта, Отклонена, Отменена, Активирована, Выполнено, 'Доставлена клиенту', Провалено) AND assignee in (currentUser())";
			issueStartPostRequest(true);


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
				await Navigation.PushAsync(new TabPageIssue(selectedIssue));
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
		async void issueStartPostRequest(bool firstRequest)
		{
			try
			{
				issueJSONSearch.jql = filterIssue;
				issueJSONSearch.maxResults = 50;
				issueJSONSearch.startAt = 0;

				RootObject rootObject = new RootObject();
				Request request = new Request(issueJSONSearch);

				rootObject = request.GetResponses();

				//Проверка на пустой список задач
				try
				{
					if (rootObject.issues != null)
					{
						if (!firstRequest && rootObject.issues.Count > 0)
						{
							issues.Add(rootObject.issues[rootObject.issues.Count - 1]);
						}
						else
						{
							issues = rootObject.issues;
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
				await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
			}
		}
		/// <summary>
		/// Выгрузка задач с применением сортировки
		/// </summary>
		/// <param name="firstRequest"></param>
		/// <param name="sortField"></param>
		/// <param name="typeSort"></param>
		async void issueStartPostRequest(bool firstRequest, string sortField = "", string typeSort = "")
		{
			try
			{
				issueJSONSearch.jql = filterIssue + " ORDER BY " + sortField + " " + typeSort;
				issueJSONSearch.maxResults = 50;
				issueJSONSearch.startAt = 0;

				RootObject rootObject = new RootObject();
				Request request = new Request(issueJSONSearch);

				rootObject = request.GetResponses();
				for (int i = issues.Count - 1; i >= 0; --i)
				{
					issues.RemoveAt(i);
				}
				//Проверка на пустой список задач
				try
				{
					if (rootObject.issues != null)
					{
						if (firstRequest && rootObject.issues.Count > 0)
						{
							for (int i = 0; i < rootObject.issues.Count; ++i)
							{
								issues.Add(rootObject.issues[i]);
							}
							//issues.Add(rootObject.issues[rootObject.issues.Count - 1]);
						}
						else
						{
							issues = rootObject.issues;
							//for (int i = 0; i < issues.Count; ++i)
							//{
							//	//ColorTypeConverter converter = new ColorTypeConverter();

							//	//Color colors = (Color)(converter.ConvertFromInvariantString(issues[i].fields.status.statusCategory.colorName));
							//	//color.Add(colors);
							//}

						}
					}
				}
				catch (Exception ex)
				{
					await DisplayAlert("Error", ex.ToString(), "OK");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await DisplayAlert("Error issues", ex.ToString(), "OK");
			}
		}

		private void ImageButton_Clicked_4(object sender, EventArgs e)
		{

		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(721)).ConfigureAwait(true);
		}
	}
}
