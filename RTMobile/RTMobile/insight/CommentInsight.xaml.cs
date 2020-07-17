using Microsoft.AppCenter.Crashes;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.jiraData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.insight
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CommentInsight : ContentPage
	{
		public ObservableCollection<jiraData.CommentInsight> comments { get; set; }
		public string InsightSummary { get; set; }
		public string InsightKey { get; set; }
		public CommentInsight()
		{
			InitializeComponent();
		}
		public CommentInsight(string InsightKey, string InsightSummary)
		{
			this.InsightKey = InsightKey;
			this.InsightSummary = InsightSummary;
			InitializeComponent();
			issueStartPostRequest(InsightKey);

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
		void issueStartPostRequest(string InsightKey)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/insight/1.0/comment/object/{InsightKey}",
					methodRequest = "GET",
					maxResults = "50",
					startAt = "0"
				};

				RootInsightComment rootObject = new RootInsightComment();
				Request request = new Request(jsonRequest);
				comments = request.GetCommentInsight();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}
		void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			((ListView)sender).SelectedItem = null;
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
		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
		}
		private async void ImageButton_Clicked_4(object sender, EventArgs e)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"rest/insight/1.0/comment/create",
					methodRequest = "POST",
					comment = newComment.Text,
					objectId = InsightKey,
					role = "0"
				};

				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);

				rootObject = request.GetResponses<RootObject>();

				//Проверка на пустой список задач


				if (rootObject.id != 0)
				{
					newComment.Text = "";
					issueStartPostRequest(InsightKey);
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
	}
}