using FFImageLoading;
using FFImageLoading.Config;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.issues;
using RTMobile.settings;
using Service.Shared.Clients;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace RTMobile
{
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();			

			login.Text = CrossSettings.Current.GetValueOrDefault("login", "");
			password.Text = CrossSettings.Current.GetValueOrDefault("password", "");
			Request request = new Request();

			//request.uploadFile();


			if (request.verifyServer())
			{
				frameLogin.IsEnabled = true;
				buttonLogin.IsEnabled = true;
				errorMessage.IsVisible = false;
				errorMessage.FontAttributes = FontAttributes.None;
				errorMessage.Margin = new Thickness(0, -15, 0, 0);
			}
			else
			{
				frameLogin.IsEnabled = false;
				buttonLogin.IsEnabled = false;
				errorMessage.IsVisible = true;
				errorMessage.Text = "Сервер не доступен!";
				errorMessage1.Text = "Повторите попытку позже!";
				errorMessage.FontAttributes = FontAttributes.Bold;
				errorMessage.Margin = new Thickness(0, -15, 0, 15);
			}
			//ToolbarItem toolbar = new ToolbarItem
			//{
			//    Text = "Настройки",
			//    Order = ToolbarItemOrder.Primary,
			//    Priority = 0,
			//    Icon = new FileImageSource
			//    {
			//        File = "settings.png"
			//    }

			//};
			//ToolbarItems.Add(toolbar);
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			Request request = new Request();
			try
			{
				if (login.Text != null && login.Text.Length > 0 && password.Text != null && password.Text.Length > 0)
				{
					if (request.authorization(login.Text.Trim(' '), password.Text))
					{
						errorMessage.IsVisible = false;
						errorMessage1.IsVisible = false;

						CrossSettings.Current.AddOrUpdateValue("login", login.Text.Trim(' '));
						CrossSettings.Current.AddOrUpdateValue("password", password.Text);
						Analytics.TrackEvent("Выполнен вход в систему: пользователь - " + CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ", " + DateTime.Now);

						//Инициализируем данные о авторизации при подключении для получения изображений в FFImageLoading
						ImageService.Instance.Config.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
							Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{CrossSettings.Current.GetValueOrDefault("login", login.Text)}:{CrossSettings.Current.AddOrUpdateValue("password", password.Text)}")));


						await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);

					}
					else
					{
						CrossSettings.Current.Remove("login");
						CrossSettings.Current.Remove("password");
						errorMessage.IsVisible = true;
						errorMessage1.IsVisible = true;
						errorMessage.Text = "Вход не выполнен!";
					}
				}
				else
				{
					errorMessage.IsVisible = true;
					errorMessage1.IsVisible = true;
				}
			}
			catch (Exception ex)
			{
				Crashes.TrackError(ex);
				Console.WriteLine(ex.ToString());
			}
		}

		private async void Button_Clicked_1(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new About()).ConfigureAwait(true);
		}

		private async void Button_Clicked_2(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new Settings()).ConfigureAwait(true);
		}

		private void Button_Clicked_3(object sender, EventArgs e)
		{


		}


	}
}
