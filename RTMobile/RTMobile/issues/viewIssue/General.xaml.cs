using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace RTMobile.issues.viewIssue
{
	public partial class General : ContentPage
	{
		/// <summary>
		/// Поля задачи
		/// </summary>
		public ObservableCollection<Fields> fieldIssue { get; set; }
		/// <summary>
		/// /Время в задаче (дата создания, SLA, ...)
		/// </summary>
		public ObservableCollection<Fields> timeIssue { get; set; }
		public Issue issue { get; set; }
		public General()
		{
			InitializeComponent();

			this.BindingContext = this;
		}
		void Subscribe()
		{
			MessagingCenter.Subscribe<Page>(this, "RefreshIssueUpdate", (sender) =>
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					//Показ всех полей, даже не видимых
					//urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,schema",
					//Показываем только видимые поля
					urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,editmeta",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);

				fieldIssue = request.GetFieldsIssue();

				Request requestIssue = new Request(jsonRequest);
				issue = requestIssue.GetResponses<Issue>();

				OnPropertyChanged(nameof(issue));
				dataIssue();
			});
		}
		private void dataIssue()
		{
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
						if (issue.fields.duedate != null)
						{
							tmpTimeIssue.Add(new Fields { name = "Срок исполнения:", value = issue.fields.duedate });
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
						if (timeIssue != null)
						{
							for (int i = timeIssue.Count; i > 0; --i)
							{
								timeIssue.RemoveAt(0);
							}
						}
						if (timeIssue == null)
						{
							timeIssue = new ObservableCollection<Fields>();
						}
						for (int i = 0; i < tmpTimeIssue.Count; ++i)
						{
							timeIssue.Add(tmpTimeIssue[i]);
						}

					}
					//Делаем запрпос на получение расширенных данных по задаче				
					JSONRequest jsonRequest = new JSONRequest()
					{
						//Показ всех полей, даже не видимых
						//urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,schema",
						//Показываем только видимые поля
						urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,editmeta",
						methodRequest = "GET"
					};
					Request request = new Request(jsonRequest);

					fieldIssue = request.GetFieldsIssue();

					OnPropertyChanged(nameof(fieldIssue));
					OnPropertyChanged(nameof(timeIssue));
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
				if (issue.fields.Components == null)
				{
					issue.fields.Components = new List<Component> ();
					issue.fields.Components.Add(new Component { name = "Отсутствуют" });
				}
				componentsList.HeightRequest = issue.fields.Components.Count * 20;
			}
		}
		public General(Issue issues)
		{
			InitializeComponent();
			issue = issues;
			//Проверяем на существование задачи в памяти
			dataIssue();
			Subscribe();
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
			Application.Current.MainPage = new AllIssues();
		}
		/// <summary>
		/// Высота блока перед сворачиванием
		/// </summary>
		private double propertyHeight = 0;
		void showPropertyIssue_Clicked(System.Object sender, System.EventArgs e)
		{
			if (propertyIssue.IsVisible)
			{
				showPropertyIssue.Source = "arrowDown.png";
				propertyHeight = propertyFrame.HeightRequest;
				propertyFrame.HeightRequest = 70;
				propertyIssue.IsVisible = false;
			}
			else
			{
				showPropertyIssue.Source = "arrowUp.png";
				propertyFrame.HeightRequest = propertyHeight;
				propertyIssue.IsVisible = true;
			}
		}
		/// <summary>
		/// Высота блока перед сворачиванием
		/// </summary>
		private double dateHeight = 0;
		void showDate_Clicked(System.Object sender, System.EventArgs e)
		{
			if (dateIssue.IsVisible)
			{
				showDate.Source = "arrowDown.png";
				dateHeight = dateFrame.HeightRequest;
				dateFrame.HeightRequest = 70;
				dateIssue.IsVisible = false;
			}
			else
			{
				showDate.Source = "arrowUp.png";
				dateFrame.HeightRequest = dateHeight;
				dateIssue.IsVisible = true;
			}
		}
		/// <summary>
		/// Высота блока перед сворачиванием
		/// </summary>
		private double detailHeight = 0;
		void showDetailIssue_Clicked(System.Object sender, System.EventArgs e)
		{
			if (detailIssue.IsVisible)
			{
				showDetailIssue.Source = "arrowDown.png";
				detailHeight = detailFrame.HeightRequest;
				detailFrame.HeightRequest = 70;
				detailIssue.IsVisible = false;
			}
			else
			{
				showDetailIssue.Source = "arrowUp.png";
				detailFrame.HeightRequest = detailHeight;
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


		private void propertyIssue_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (sender is ListView lv)
			{
				lv.SelectedItem = null;
			}
		}
	}
}
