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
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using FFImageLoading.Helpers;
using System.Text.RegularExpressions;
using RTMobile.issues.viewIssue;
using RTMobile.insight;

namespace RTMobile
{

	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		static public class MeUser
		{
			static public User User { get; set; }
		}
		string data { get; set; }
		public MainPage(string data)
		{
			InitializeComponent();
			IGeolocator locator = CrossGeolocator.Current;
			login.Text = CrossSettings.Current.GetValueOrDefault("login", "");

			Request request = new Request();
			this.data = data;

			if (request.verifyServer())
			{
				frameLogin.IsEnabled = true;
				buttonLogin.IsEnabled = true;
				errorMessage.IsVisible = false;
				errorMessage.FontAttributes = FontAttributes.None;
				errorMessage.Margin = new Thickness(0, -15, 0, 0);

				//Если логин и пароль существуют, то даем пользователю входить по отпечатку
				if (CrossSettings.Current.GetValueOrDefault("login", string.Empty) != string.Empty && CrossSettings.Current.GetValueOrDefault("password", string.Empty) != string.Empty)
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
						//Если авторизация прошла успешно, то записываем значения в память приложения для дальнейшего быстрого входа в систему
						CrossSettings.Current.AddOrUpdateValue("login", login.Text.Trim(' '));
						CrossSettings.Current.AddOrUpdateValue("password", password.Text);
						Analytics.TrackEvent("Выполнен вход в систему: пользователь - " + CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ", " + DateTime.Now);

						//Инициализируем данные о авторизации при подключении для получения изображений в FFImageLoading
						ImageService.Instance.Config.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
							Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{CrossSettings.Current.GetValueOrDefault("login", login.Text)}:{CrossSettings.Current.AddOrUpdateValue("password", password.Text)}")));
						


						if (data == "empty")
						{
							await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);

						}
						else
						{
							//регулярные выражения обрабатывают полученые ссылки
							// и если ссылка соответсвует одному из регклярных выражений, то вызывается страница
							// соответсвующая тому или инному регулярному выражению
							// если ссылка не соответствует ни одному регулярному выражению, то открывается глаынй экран приложенгия


							Regex regex = new Regex(@"(objectId=(\w{2,100}$)|(id=(.{2,1000}$)))");
							Match match2 = regex.Match(data);
							if (match2.Success)
							{
								string input = Regex.Replace(match2.Value, @"(^\w{2,100}=)", "");
								JSONRequest jsonRequest = new JSONRequest()
								{
									urlRequest = $"/rest/insight/1.0/object/{input}",
									methodRequest = "GET"
								};
								request = new Request(jsonRequest);

								ObjectEntry insightObject = request.GetResponses<ObjectEntry>();
								await Navigation.PushModalAsync(new NavigationPage(new TabPageObjectInsight(insightObject))).ConfigureAwait(true);
							}

							if (!match2.Success)
							{
								regex = new Regex(@"(\w{2,10}-+\d{2,100}$)");
								Match match1 = regex.Match(data);
								if (match1.Success)
								{
									Issue issue = new Issue() { key = match1.Value };
									await Navigation.PushModalAsync(new NavigationPage(new TabPageIssue(issue))).ConfigureAwait(true);
								}
								if (!match1.Success && !match2.Success)
								{
									await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);
								}
							}

						}

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

			//Проверяем прошла ли авторизация по отпечатку
			FingerprintAuthenticationResult authResult = await CrossFingerprint.Current.AuthenticateAsync(conf);
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
					//Проверяем  авторизацию по логину и паролю, если все ок, то выводим задачи пользователю, если нет, то выводим ошибку
					if (request.authorization(CrossSettings.Current.GetValueOrDefault("login", login.Text.Trim(' ')), CrossSettings.Current.GetValueOrDefault("password", password.Text)))
					{

						errorMessage.IsVisible = false;
						errorMessage1.IsVisible = false;

						Analytics.TrackEvent("Выполнен вход в систему: пользователь - " + CrossSettings.Current.GetValueOrDefault("login", string.Empty) + ", " + DateTime.Now);

						//Инициализируем данные о авторизации при подключении для получения изображений в FFImageLoading
						ImageService.Instance.Config.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
							Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{CrossSettings.Current.GetValueOrDefault("login", login.Text)}:{CrossSettings.Current.GetValueOrDefault("password", password.Text)}")));

						// если дата емпти, запрускает главный экран приложения
						if (data == "empty")
						{
							await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);

						}
						else
						{
							//регулярные выражения обрабатывают полученые ссылки
							// и если ссылка соответсвует одному из регклярных выражений, то вызывается страница
							// соответсвующая тому или инному регулярному выражению
							// если ссылка не соответствует ни одному регулярному выражению, то открывается глаынй экран приложенгия


							Regex regex = new Regex(@"(objectId=(\w{2,100}$)|(id=(.{2,1000}$)))");
							Match match2 = regex.Match(data);
							if (match2.Success)
							{
								string input = Regex.Replace(match2.Value, @"(^\w{2,100}=)", "");
								JSONRequest jsonRequest = new JSONRequest()
								{
									urlRequest = $"/rest/insight/1.0/object/{input}",
									methodRequest = "GET"
								};
								request = new Request(jsonRequest);

								ObjectEntry insightObject = request.GetResponses<ObjectEntry>();
								await Navigation.PushModalAsync(new NavigationPage (new TabPageObjectInsight(insightObject))).ConfigureAwait(true);
							}

                            if (!match2.Success)
                            {
								regex = new Regex(@"(\w{2,10}-+\d{2,100}$)");
								Match match1 = regex.Match(data);
								if (match1.Success)
								{
									Issue issue = new Issue() { key = match1.Value };
									await Navigation.PushModalAsync(new NavigationPage(new TabPageIssue(issue))).ConfigureAwait(true);
								}
								if (!match1.Success && !match2.Success)
								{
									await Navigation.PushModalAsync(new AllIssues()).ConfigureAwait(true);
								}
							}

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
			//Обрабатываем значения нажатия кнопки просмотра пароля
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
