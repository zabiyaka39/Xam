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
	public partial class People : ContentPage
	{
		public Issue issue { get; set; }
		public ObservableCollection<User> watchers { get; set; }
		private List<RTMobile.Transition> transition { get; set; }
		public People(Issue issue)
		{
			InitializeComponent();
			this.issue = issue;
			warchersIssue();

			this.BindingContext = this;
		}
		private void warchersIssue()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/watchers/",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				//Получаем список наблюдателей
				watchers = request.GetResponses<Watchers>().watchers;
				//Получаем логин пользователя под которым зашли
				string meUserName = CrossSettings.Current.GetValueOrDefault("login", "");
				//Проходимся по всем наблюдаелям и сравниваем с текущем профилем
				for (int i = 0; i < watchers.Count; ++i)
				{
					//Если нашли совпадения то устанавливаем флаг в true
					if (watchers[i].name.ToUpper() == meUserName.ToUpper())
					{
						//Изменяем изображение на "отменить наблюдение за задачей" и устанавливаем соответствующую надпись на label
						stopStartWatching.Text = "Прекратить наблюдение";
						stopStartWatchingImage.Source = "visibilityOff.png";
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
				Console.WriteLine(ex.ToString());
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
		void showWatcherPeople_Clicked(System.Object sender, System.EventArgs e)
		{
			if (watcherPeople.IsVisible)
			{
				showWatcherPeople.Source = "arrowDown.png";
				watcherFrame.HeightRequest = 70;
				watcherPeople.IsVisible = false;
			}
			else
			{
				showWatcherPeople.Source = "arrowUp.png";
				watcherFrame.HeightRequest = 250;
				watcherPeople.IsVisible = true;
			}
		}
		void showGeneralPeople_Clicked(System.Object sender, System.EventArgs e)
		{
			if (generalPeople.IsVisible)
			{
				showGeneralPeople.Source = "arrowDown.png";
				generalFrame.HeightRequest = 70;
				generalPeople.IsVisible = false;
			}
			else
			{
				showGeneralPeople.Source = "arrowUp.png";
				generalFrame.HeightRequest = 210;
				generalPeople.IsVisible = true;
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
		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Profile(issue.fields.creator.name)).ConfigureAwait(true);
		}
		private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Profile(issue.fields.assignee.name)).ConfigureAwait(true);
		}
		private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			Watchers selectedIssue = e.Item as Watchers;
			if (selectedIssue != null)
			{
				await Navigation.PushAsync(new Profile(selectedIssue.name)).ConfigureAwait(true);
			}
			((ListView)sender).SelectedItem = null;
		}
		private void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
		{
			if (stopStartWatching.Text == "Прекратить наблюдение")
			{
				//Запрос на удаление текущего пользователя из наблюдателей
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/watchers?username={CrossSettings.Current.GetValueOrDefault("login", string.Empty)}",
					methodRequest = "DELETE"
				};
				Request request = new Request(jsonRequest);
				//Получаем список наблюдателей
				request.GetResponses<RootObject>();

				JSONRequest jsonRequestWatchers = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/watchers/",
					methodRequest = "GET"
				};
				Request requestWatchers = new Request(jsonRequestWatchers);
				//Получаем список наблюдателей
				ObservableCollection<User> watchersTmp = requestWatchers.GetResponses<Watchers>().watchers;
				//Очищаем старый список
				for (int i = watchers.Count - 1; i >= 0; --i)
				{
					watchers.RemoveAt(0);
				}
				//Обновляем старый список новыми данными
				for (int i = 0; i < watchersTmp.Count; ++i)
				{
					watchers.Add(watchersTmp[i]);
				}
				stopStartWatching.Text = "Установить наблюдение";
				stopStartWatchingImage.Source = "visibility.png";
			}
			else
			{
				//Установить текущего пользователя наблюдателем за задачей
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/watchers?username={CrossSettings.Current.GetValueOrDefault("login", string.Empty)}",
					methodRequest = "POST"
				};
				Request request = new Request(jsonRequest);
				//Получаем список наблюдателей
				request.GetResponses<RootObject>();

				JSONRequest jsonRequestWatchers = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/watchers/",
					methodRequest = "GET"
				};
				Request requestWatchers = new Request(jsonRequestWatchers);
				//Получаем список наблюдателей
				ObservableCollection<User> watchersTmp = requestWatchers.GetResponses<Watchers>().watchers;
				//Очищаем старый список
				for (int i = watchers.Count - 1; i >= 0; --i)
				{
					watchers.RemoveAt(0);
				}
				//Обновляем старый список новыми данными
				for (int i = 0; i < watchersTmp.Count; ++i)
				{
					watchers.Add(watchersTmp[i]);
				}
				stopStartWatching.Text = "Прекратить наблюдение";
				stopStartWatchingImage.Source = "visibilityOff.png";
			}
		}
	}
}
