using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class General : ContentPage
	{
		/// <summary>
		/// Поля задачи
		/// </summary>
		public List<Fields> fieldIssue { get; set; }
		/// <summary>
		/// /Время в задаче (дата создания, SLA, ...)
		/// </summary>
		ObservableCollection<Fields> timeIssue { get; set; }
		/// Переходы по задачи
		/// </summary>
		public List<RTMobile.Transition> transition { get; set; }
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
				try
				{
					if (issue != null && issue.fields != null)
					{
						ObservableCollection<Fields> tmpTimeIssue = new ObservableCollection<Fields>();
						tmpTimeIssue.Add(new Fields { name = "Дата создания:", value = issue.fields.created });

						if (issue.fields.updated != null)
						{
							tmpTimeIssue.Add(new Fields { name = "Обновлено:", value = issue.fields.updated });
						}
						if (issue.fields.resolutiondate != null)
						{
							tmpTimeIssue.Add(new Fields { name = "Дата решения:", value = issue.fields.resolutiondate });
						}
						JSONRequest jsonRequestSLA = new JSONRequest()
						{
							//Получаем все доступные SLA
							urlRequest = $"/rest/servicedeskapi/request/{issue.key}/sla",
							methodRequest = "GET"
						};
						Request requestSLA = new Request(jsonRequestSLA);
						SLA timeSLAIssue = requestSLA.GetResponses<SLA>();

						for (int i = 0; i < timeSLAIssue.Values.Count; ++i)
						{
							if (timeSLAIssue.Values[i].OngoingCycle != null)
							{
								tmpTimeIssue.Add(new Fields { name = timeSLAIssue.Values[i].name, value = timeSLAIssue.Values[i].OngoingCycle.RemainingTime.Friendly });
							}
						}
						timeIssue = tmpTimeIssue;
						dateIssue.ItemsSource = timeIssue;
					}
					//Делаем запрпос на получение расширенных данных по задаче				
					JSONRequest jsonRequest = new JSONRequest()
					{
						//Показ всех полей, даже не видимых
						//urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,schema",
						//Показываем только видимые поля
						urlRequest = $"/rest/api/2/issue/{issue.key}/editmeta",
						methodRequest = "GET"
					};
					Request request = new Request(jsonRequest);

					fieldIssue = request.GetCustomField();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Crashes.TrackError(ex);
				}
				//Проверяем наличие полей, при отсутствии значения скрываем их
				if (issue.fields.resolution == null)
				{
					issue.fields.resolution = new Resolution { name = "Нет решения" };
				}
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
				propertyFrame.HeightRequest = 150;
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
				dateFrame.HeightRequest = 150;
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
				detailFrame.HeightRequest = 150;
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
