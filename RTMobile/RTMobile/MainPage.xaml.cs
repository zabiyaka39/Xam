using FFImageLoading;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.issues;
using RTMobile.settings;
using System;
using System.ComponentModel;
using System.Net.Http.Headers;
using Xamarin.Forms;
using Rg.Plugins.Popup.Services;
using RTMobile.jiraData;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;

namespace RTMobile
{
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{

		public MainPage()
		{
			InitializeComponent();

			login.Text = CrossSettings.Current.GetValueOrDefault("login", "");
			
			Request request = new Request();

			if (request.verifyServer())
			{
				frameLogin.IsEnabled = true;
				buttonLogin.IsEnabled = true;
				errorMessage.IsVisible = false;
				errorMessage.FontAttributes = FontAttributes.None;
				errorMessage.Margin = new Thickness(0, -15, 0, 0);
				if(CrossSettings.Current.GetValueOrDefault("login", string.Empty) != string.Empty && CrossSettings.Current.GetValueOrDefault("password", string.Empty) != string.Empty)
				{
					fingerAuth(true);
				}
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
			try
			{
				await PopupNavigation.Instance.PushAsync(new StatusBar());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			Request request = new Request();
			try
			{
				//Проверяем на пустые поля
				if (!string.IsNullOrWhiteSpace(login.Text) && !string.IsNullOrWhiteSpace(password.Text))
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
						errorMessage.IsVisible = true;
						errorMessage1.IsVisible = true;
						errorMessage.Text = "Вход не выполнен!";
						try
						{
							await PopupNavigation.Instance.PopAsync(true);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
				}
				else
				{
					errorMessage.IsVisible = true;
					errorMessage1.IsVisible = true;
				}
				try
				{
					await PopupNavigation.Instance.PopAsync(true);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
			catch (Exception ex)
			{
				await PopupNavigation.Instance.PopAsync(true);
				password.Text = "";
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
		public async void fingerAuth(bool first = false)
		{
			bool isFingerprintAvailable = await CrossFingerprint.Current.IsAvailableAsync(false);
			//Проверяем наличие датчика отпечатков и наличие загрузки приложение
			//Если первый раз запустили, то нет смысла выводить данное сообщение
			if (!isFingerprintAvailable && !first)
			{
				await DisplayAlert("Ошибка",
					"Вход по отпечатку пальца недоступен", "OK");
				return;
			}

			AuthenticationRequestConfiguration conf =
				new AuthenticationRequestConfiguration("Аутентификация",
				"Авторизируйтесь");

			var authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
			if (authResult.Authenticated)
			{
				try
				{

					try
					{
						await PopupNavigation.Instance.PushAsync(new StatusBar());
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
					Request request = new Request();
					if (request.authorization(CrossSettings.Current.GetValueOrDefault("login", login.Text.Trim(' ')), CrossSettings.Current.GetValueOrDefault("password", password.Text)))
					{

						errorMessage.IsVisible = false;
						errorMessage1.IsVisible = false;

						Analytics.TrackEvent("Выполнен вход в систему: пользователь - " + CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ", " + DateTime.Now);

						//Инициализируем данные о авторизации при подключении для получения изображений в FFImageLoading
						ImageService.Instance.Config.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
							Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{CrossSettings.Current.GetValueOrDefault("login", login.Text)}:{CrossSettings.Current.GetValueOrDefault("password", password.Text)}")));

						await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);

						try
						{
							await PopupNavigation.Instance.PopAsync(true);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
					else
					{
						errorMessage.IsVisible = true;
						errorMessage1.IsVisible = true;
						errorMessage.Text = "Ошибка входа! Ведите логин и пароль";
						try
						{
							await PopupNavigation.Instance.PopAsync(true);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.ToString());
						}
					}
				}
				catch (Exception ex)
				{

					Crashes.TrackError(ex);
					Console.WriteLine(ex.ToString());
				}

			}
		}
		private async void FPEntery(object sender, EventArgs e)
		{
			fingerAuth();
		}

		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			if (password.IsPassword == true)
			{
				visibilityButton.Source = "visibility_off_white.png";
				password.IsPassword = false;
			}
			else
			{
				visibilityButton.Source = "visibility_white.png";
				password.IsPassword = true;
			}
		}
	}
}
