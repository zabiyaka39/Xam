using System;
using System.Collections.Generic;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class General : ContentPage
	{
		public List<Fields> fieldIssue { get; set; }//поля заявки
		public List<RTMobile.Transition> transition;//Переходы по заявке
		public Issue issue { get; set; }
		public General()
		{
			InitializeComponent();
			this.BindingContext = this;
		}
		public General(Issue issues)
		{
			InitializeComponent();
			issue = issues;
			//Проверяем на существование задачи в памяти
			if (issue != null && issue.key != null)
			{
				//Делаем запрпос на получение расширенных данных по задаче
				Request request = new Request(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + $"/rest/api/2/issue/{issue.key}?expand=names,schema");
				fieldIssue = request.GetCustomField();
				transitionIssue();

				//Проверяем наличие полей, при отсутствии значения скрываем их
				if (issue.fields.resolution == null)
				{
					issue.fields.resolution = new Resolution { name = "Нет решения" };
				}
			}
			this.BindingContext = this;
		}
		private void transitionIssue()
		{
			string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/issue/" + issue.key + "/transitions/";

			Request request = new Request(getIssue);
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
					await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(int.Parse(transition[((ToolbarItem)sender).Priority-1].id), issue.key)).ConfigureAwait(true);
				};
				ToolbarItems.Add(tb);
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

		void showPropertyIssue_Clicked(System.Object sender, System.EventArgs e)
		{
			if (propertyIssue.IsVisible)
			{
				showPropertyIssue.Source = "arrowDown.png";
				propertyFrame.HeightRequest = 70;
				propertyIssue.IsVisible = false;
			}
			else
			{
				showPropertyIssue.Source = "arrowUp.png";
				propertyFrame.HeightRequest = 200;
				propertyIssue.IsVisible = true;
			}
		}

		void showDate_Clicked(System.Object sender, System.EventArgs e)
		{
			if (dateIssue.IsVisible)
			{
				showDate.Source = "arrowDown.png";
				dateFrame.HeightRequest = 70;
				dateIssue.IsVisible = false;
			}
			else
			{
				showDate.Source = "arrowUp.png";
				dateFrame.HeightRequest = 200;
				dateIssue.IsVisible = true;
			}
		}

		void showDetailIssue_Clicked(System.Object sender, System.EventArgs e)
		{
			if (detailIssue.IsVisible)
			{
				showDetailIssue.Source = "arrowDown.png";
				detailFrame.HeightRequest = 70;
				detailIssue.IsVisible = false;
			}
			else
			{
				showDetailIssue.Source = "arrowUp.png";
				detailFrame.HeightRequest = 200;
				detailIssue.IsVisible = true;
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

	}
}
