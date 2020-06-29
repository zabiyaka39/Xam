using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile;
using RTMobile.calendar;
using RTMobile.insight;
using RTMobile.profile;

using Xamarin.Forms;

namespace RTMobile.filter
{
	public partial class Filter : ContentPage
	{
		public ObservableCollection<Filters> favorites { get; set; }
		public ObservableCollection<Filters> lastFilters { get; set; }
		public ObservableCollection<Filters> allFilters { get; set; }
		/// <summary>
		/// Тело запроса
		/// </summary>
		private string jqlFilter = "";
		/// <summary>
		/// Номер выбранного фильтра
		/// </summary>
		private int numberFilter = 0;
		/// <summary>
		/// Конструктор для вызова с главной страницы
		/// </summary>
		public Filter()
		{
			InitializeComponent();
			if (lastFilters == null)
			{
				lastFilters = new ObservableCollection<Filters>();
			}
			try
			{
				var tmpFilter = CrossSettings.Current.GetValueOrDefault("lastFilters","");

				if (tmpFilter.Length > 0)
				{
					string[] listTmpFilters = tmpFilter.Split(',');
					for (int i = 0, countFilters = listTmpFilters.Count(); i < countFilters; ++i)
					{
						//Проходимся по всему списку фильтров которые ранее смотрели
						try
						{
							JSONRequest jsonRequest = new JSONRequest()
							{
								urlRequest = $"/rest/api/2/filter/{listTmpFilters[i]}",
								methodRequest = "GET"
							};
							Request request = new Request(jsonRequest);
							//Получаем список избранных фильтров
							lastFilters.Add(request.GetResponses<Filters>());
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
							Crashes.TrackError(ex);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}

			//Задаем список последних фильтров (стандартные фильтры)
			lastFilters.Add(new Filters()
			{
				Id = -1,
				Name = "Мои открытые задачи",
				Jql = "assignee = currentUser() AND resolution = Unresolved order by updated DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -2,
				Name = "Сообщенные мной",
				Jql = "reporter = currentUser() order by created DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -3,
				Name = "Все задачи",
				Jql = "order by created DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -4,
				Name = "Открытые задачи",
				Jql = "resolution = Unresolved order by priority DESC,updated DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -5,
				Name = "Завершенные задачи",
				Jql = "statusCategory = Done order by updated DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -6,
				Name = "Недавно просмотренные",
				Jql = "issuekey in issueHistory() order by lastViewed DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -7,
				Name = "Недавно созданные",
				Jql = "created >= -1w order by created DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -8,
				Name = "Недавно решенные",
				Jql = "resolutiondate >= -1w order by updated DESC"
			});
			lastFilters.Add(new Filters()
			{
				Id = -9,
				Name = "Недавно обновленные",
				Jql = "updated >= -1w order by updated DESC"
			});

			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/filter/favourite",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				//Получаем список избранных фильтров
				favorites = request.GetResponses<ObservableCollection<Filters>>();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
			//Получаем список всех фильтров
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/gadget/1.0/pickers/filters",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				//Получаем список избранных фильтров
				RootObject rootObject = request.GetResponses<RootObject>();
				if (rootObject != null)
				{
					allFilters = rootObject.filters;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
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
		}
		void ImageButton_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Navigation.PopToRootAsync();
		}
		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			if (grouping.IsVisible == false)
			{
				buttonShowGroup.Text = "▲";
				grouping.IsVisible = true;
			}
			else
			{
				buttonShowGroup.Text = "▼";
				grouping.IsVisible = false;
			}
		}
		private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{
			if (sorted.IsVisible == false)
			{
				buttonShowSorted.Text = "▲";
				sorted.IsVisible = true;
			}
			else
			{
				buttonShowSorted.Text = "▼";
				sorted.IsVisible = false;
			}
		}
		private void ToolbarItem_Clicked(object sender, EventArgs e)
		{
			var tmpFilter = CrossSettings.Current.GetValueOrDefault("lastFilters", "");
			//Добавляем номер выбранного фильтра в список последних использованных фильтров
			if (tmpFilter.Length == 0)
			{
				CrossSettings.Current.AddOrUpdateValue("lastFilters", numberFilter.ToString());				
			}
			else
			{
				CrossSettings.Current.AddOrUpdateValue("lastFilters", tmpFilter + "," + numberFilter.ToString());
			}
			string sorted = "";			

			JSONRequest jsonRequestFilter = new JSONRequest()
			{
				urlRequest = $"/rest/api/2/filter/{numberFilter.ToString()}",
				methodRequest = "GET"
			};

			Request request = new Request(jsonRequestFilter);
			Filters filter = request.GetResponses<Filters>();
			if (filter == null)
			{
				filter = new Filters();
				filter.Jql = "";
			}
			
			if (filter.Jql.IndexOf("ORDER") == -1)
			{
				if (typeSort.SelectedIndex == 0)
				{
					sorted = "ORDER BY key ASC";
				}
				else
				{
					sorted = "ORDER BY key DESC";
				}
			}

			JSONRequest jsonRequest = new JSONRequest()
			{
				urlRequest = "/rest/api/2/search",
				methodRequest = "POST",
				jql = filter.Jql + " " + sorted,
				maxResults = "50",
				startAt = "0"
			};
			MessagingCenter.Send(this, "RefreshMainPage", jsonRequest);
			Navigation.PopToRootAsync();
		}
		private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
		{
			if (allFilterList.IsVisible == false)
			{
				buttonShowAllFilters.Text = "▲";
				allFilterList.IsVisible = true;
			}
			else
			{
				buttonShowAllFilters.Text = "▼";
				allFilterList.IsVisible = false;
			}
		}
		private void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
		{
			if (lastFilterList.IsVisible == false)
			{
				buttonShowLastFilters.Text = "▲";
				lastFilterList.IsVisible = true;
			}
			else
			{
				buttonShowLastFilters.Text = "▼";
				lastFilterList.IsVisible = false;
			}
		}
		private void favoritesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItemIndex != -1)
			{
				numberFilter = favorites[e.SelectedItemIndex].Id;
				jqlFilter = favorites[e.SelectedItemIndex].Jql;
				lastFilterList.SelectedItem = -1;
				allFilterList.SelectedItem = -1;
			}
		}
		private void lastFilterList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItemIndex != -1)
			{
				numberFilter = lastFilters[e.SelectedItemIndex].Id;
				jqlFilter = lastFilters[e.SelectedItemIndex].Jql;
				favoritesList.SelectedItem = -1;
				allFilterList.SelectedItem = -1;
			}
		}
		private void allFilterList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItemIndex != -1)
			{
				numberFilter = allFilters[e.SelectedItemIndex].Id;
				jqlFilter = allFilters[e.SelectedItemIndex].Jql;
				lastFilterList.SelectedItem = -1;
				favoritesList.SelectedItem = -1;
			}
		}
	}
}
