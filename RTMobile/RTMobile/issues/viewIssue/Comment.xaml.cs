using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class Comment : ContentPage
	{
		public ObservableCollection<RTMobile.Comment> comments { get; set; }
		private List<RTMobile.Transition> transition { get; set; }//Переходы по заявке

		public string issueKeySummary { get; set; }
		public string issueSummary { get; set; }
		public string issueKey { get; set; }
		public Comment()
		{
			InitializeComponent();
			transitionIssue(issueKey);
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
		public Comment(string issueKey, string issueSummary)
		{
			this.issueKey = issueKey;
			this.issueSummary = issueSummary;
			this.issueKeySummary = issueKey + " - " + issueSummary;
			InitializeComponent();
			issueStartPostRequest(issueKey);
			transitionIssue(issueKey);

			if (this.comments != null && this.comments.Count > 0)

			{
				listComment.IsVisible = true;
				noneComment.IsVisible = false;
			}
			else
			{
				listComment.IsVisible = false;
				noneComment.IsVisible = true;
			}
			this.BindingContext = this;
		}
		private void transitionIssue(string issueKey)
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
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}
		/// <summary>
		/// Выгрузка всех задач
		/// </summary>
		void issueStartPostRequest(string issueKey, bool firstRequest = true)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{issueKey}/comment",
					methodRequest = "GET",
					maxResults = "50",
					startAt = "0"
				};

				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);

				rootObject = request.GetResponses<RootObject>();
				//проверка на наличие комментариев. При отсутствии комментариев добавляем все, при наличии добавляем только последний

				if (!firstRequest && rootObject.comments.Count > 0)
				{
					comments.Add(rootObject.comments[rootObject.comments.Count - 1]);
				}
				else
				{
					comments = rootObject.comments;
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

		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new History(issueKey, issueSummary));
		}

		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new WorkJournal(issueKey, issueSummary));
		}

		void showHistory_Clicked(System.Object sender, System.EventArgs e)
		{

		}

		void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
		}

		private async void ImageButton_Clicked_4(object sender, EventArgs e)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issueKey}/comment",
					methodRequest = "POST",
					body = newComment.Text
				};

				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);

				rootObject = request.GetResponses<RootObject>();

				//Проверка на пустой список задач


				if (rootObject.id != 0)
				{
					newComment.Text = "";
					issueStartPostRequest(issueKey, false);
					//await DisplayAlert("Готово", "Комментарий добавлен", "OK").ConfigureAwait(true);
				}
				else
				{
					await DisplayAlert("Ошибка", "Ошибка добавления комментария в систему", "OK").ConfigureAwait(true);
				}

				if (this.comments.Count > 0)
				{
					listComment.IsVisible = true;
					noneComment.IsVisible = false;
				}
				else
				{
					listComment.IsVisible = false;
					noneComment.IsVisible = true;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
			}
		}

		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
		}
	}
}
