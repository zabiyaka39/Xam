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
using Rg.Plugins.Popup.Services;
using RTMobile.issues.eventIssue;

namespace RTMobile.issues.viewIssue
{
	public partial class People : ContentPage
	{
		public Issue issue { get; set; }
		public ObservableCollection<User> watchers { get; set; }
		public People(Issue issue)
		{
			InitializeComponent();
			this.issue = issue;
			warchersIssue();
			Subscribe();
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
		private void Subscribe()
		{
			MessagingCenter.Subscribe<AddWatchersModal>(this, "WatchersChange", (sender) => {
				warchersIssue();
				OnPropertyChanged(nameof(watchers));
			});
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
		//Высота блока с наблюдателями для скрытия и разворачивания
		private double heightWatcherPeople = 0;
		void showWatcherPeople_Clicked(System.Object sender, System.EventArgs e)
		{
			if (watcherPeople.IsVisible)
			{
				//Запоминаем высоту блока перед тем как его скрыть
				heightWatcherPeople = watcherPeople.HeightRequest;
				showWatcherPeople.Source = "arrowDown.png";
				watcherFrame.HeightRequest = 70;
				watcherPeople.IsVisible = false;
			}
			else
			{
				showWatcherPeople.Source = "arrowUp.png";
				//Указываем высоту блока равной запомненной ранее
				watcherFrame.HeightRequest = heightWatcherPeople;
				watcherPeople.IsVisible = true;
			}
		}
		//Высота блока с исполнителем и автором для скрытия и разворачивания
		private double heightGeneralPeople = 0;
		void showGeneralPeople_Clicked(System.Object sender, System.EventArgs e)
		{
			if (generalPeople.IsVisible)
			{
				//Запоминаем высоту блока перед тем как его скрыть
				heightGeneralPeople = generalFrame.HeightRequest;
				showGeneralPeople.Source = "arrowDown.png";
				generalFrame.HeightRequest = 70;
				generalPeople.IsVisible = false;
			}
			else
			{
				showGeneralPeople.Source = "arrowUp.png";
				//Указываем высоту блока равной запомненной ранее
				generalFrame.HeightRequest = heightGeneralPeople;
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
			string result = await DisplayActionSheet("Выберете действие", "Отмена", null, "Профиль пользователя", "Изменить исполнителя");
			switch (result)
			{
				case "Профиль пользователя":
					{
						await Navigation.PushAsync(new Profile(issue.fields.assignee.name)).ConfigureAwait(true);
						break;
					}
				case "Изменить исполнителя":
					{
						await PopupNavigation.Instance.PushAsync(new EditPeople(issue.key, true));
						JSONRequest jsonRequest = new JSONRequest()
						{
							//Показ всех полей, даже не видимых
							//urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,schema",
							//Показываем только видимые поля
							urlRequest = $"/rest/api/2/issue/{issue.key}?expand=names,editmeta",
							methodRequest = "GET"
						};

						Request requestIssue = new Request(jsonRequest);
						issue = requestIssue.GetResponses<Issue>();

						nameReporter.Text = issue.fields.assignee.displayName;

						break;
					}
			}
		}
		private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			User selectedWatcher = e.Item as User;
			if (selectedWatcher != null)
			{
				await Navigation.PushAsync(new Profile(selectedWatcher.name)).ConfigureAwait(true);
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
				for (int i = watchers.Count; i > 0; --i)
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
		void Choice_watchers(object sender, EventArgs e)
		{
			PopupNavigation.Instance.PushAsync(new AddWatchersModal(issue.key));
		}
	}
}
