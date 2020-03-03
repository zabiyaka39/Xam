﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Net;
using System.IO;
using RTMobile.profile;
using RTMobile.notification;
using RTMobile.insight;
using Microsoft.AppCenter.Crashes;

namespace RTMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class IssuePage : ContentPage
	{
		public ObservableCollection<Issue> issues { get; set; }
		private string filterIssue { get; set; }
		string typeSort = "";
		//List<Color> color { get; set; }

		public IssuePage()
		{
			InitializeComponent();

			filterIssue = "status not in  (Закрыта, Отклонена, Отменена, Активирована, Выполнено, 'Доставлена клиенту', Провалено) AND assignee in (currentUser())";
			issueStartPostRequest(true);
			//for (int i = 0; i < issues.Count; ++i)
			//{
			//    issues[i].fields.issuetype.iconUrl = issues[i].fields.issuetype.iconUrl;
			//    Console.WriteLine(issues[i].fields.issuetype.iconUrl);
			//}

			this.BindingContext = this;
		}
		/// <summary>
		/// Выгрузка всех задач
		/// </summary>
		async void issueStartPostRequest(bool firstRequest)
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest();
				jsonRequest.urlRequest = "/rest/api/2/search?";
				jsonRequest.methodRequest = "POST";
				jsonRequest.jql = filterIssue;
				jsonRequest.maxResults = 50;
				jsonRequest.startAt = 0;

				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);
				rootObject = request.GetResponses<RootObject>();

				if (rootObject.issues != null)
				{
					if (!firstRequest && rootObject.issues.Count > 0)
					{
						issues.Add(rootObject.issues[rootObject.issues.Count - 1]);
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
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
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
				JSONRequest jsonRequest = new JSONRequest();
				jsonRequest.urlRequest = "/rest/api/2/search?";
				jsonRequest.methodRequest = "POST";
				jsonRequest.jql = filterIssue + " ORDER BY " + sortField + " " + typeSort; 
				jsonRequest.maxResults = 50;
				jsonRequest.startAt = 0;

				RootObject rootObject = new RootObject();
				Request request = new Request(jsonRequest);


				rootObject = request.GetResponses<RootObject>();
				for (int i = issues.Count - 1; i >= 0; --i)
				{
					issues.RemoveAt(i);
				}
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
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}
		/// <summary>
		/// Тап перехода к задаче
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public async void OnItemTapped(object sender, ItemTappedEventArgs e)//обработка нажатия на элемент в ListView
		{
			Issue selectedIssue = e.Item as Issue;
			if (selectedIssue != null)
			{
				//await Navigation.PushAsync(new general(selectedIssue));
				//await DisplayAlert("Выбранная модель", $"{selectedIssue.key}", "OK");
			}
			((ListView)sender).SelectedItem = null;
		}

		/// <summary>
		/// Кнопка поиска задачи
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new searchIssue()).ConfigureAwait(true);
		}
		/// <summary>
		/// Кнопка создания задачи
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Button_Clicked_1(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new createIssue()).ConfigureAwait(true);
		}
		/// <summary>
		/// Тап перехода к задаче
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			Issue selectedIssue = sender as Issue;
			Console.WriteLine("sdadasdsadadasd" + selectedIssue.key);
			if (selectedIssue != null)
			{
				await DisplayAlert("Выбранная модель", $"{selectedIssue.key}", "OK").ConfigureAwait(true);

				//        await Navigation.PushAsync(new general(selectedIssue));
				//        //await DisplayAlert("Выбранная модель", $"{selectedIssue.key}", "OK");
				//    }
				//    ((ListView)sender).SelectedItem = null;
			}
		}
		/// <summary>
		/// Кнопка перехода к профилю
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ImageButton_Clicked(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Profile()).ConfigureAwait(true);
		}
		/// <summary>
		/// Кнопка перехода к уведомлениям
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ImageButton_Clicked_1(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Notification()).ConfigureAwait(true);
		}
		/// <summary>
		/// Кнопка перехода к разделу Insight
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void ImageButton_Clicked_2(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Insight()).ConfigureAwait(true);
		}
		/// <summary>
		/// Обработка сортировки
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pickerSort_SelectedIndexChanged(object sender, EventArgs e)
		{
			string sortField = "";

			switch (pickerSort.Items[pickerSort.SelectedIndex])
			{
				case "По дате":
					{
						sortField = "created";
						break;
					}
				case "По номеру":
					{
						sortField = "key";
						break;
					}
				case "По статусу":
					{
						sortField = "status";
						break;
					}
			}
			if (typeSort == "ACS")
			{
				typeSort = "DESC";
			}
			else
			{
				typeSort = "ASC";
			}

			issueStartPostRequest(true, sortField, typeSort);
		}
	}
}