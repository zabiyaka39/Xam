using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class Transition : ContentPage
	{

		public List<Fields> fieldIssue { get; set; }//Поля заявки
		public Transition(int transitionId, string numberIssue)
		{
			InitializeComponent();

			Request request = new Request(/*CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty)
							  +*/ $"https://dev-sd.rosohrana.ru/rest/api/2/issue/{numberIssue}/transitions?expand=transitions.fields&transitionId=" + transitionId);
			fieldIssue = request.GetFieldTransitions();

			for (int i = 0; i < fieldIssue.Count; ++i)
			{
				//Проверяем на необходимость показа поля
				if (fieldIssue[i].hasScreen)
				{
					//Создаем label с названием получаемого аргумента для более понятного вида для пользователя
					Label label = new Label
					{
						Text = fieldIssue[i].displayName,
						TextColor = Color.FromHex("#F0F1F0"),
						FontSize = 14
					};
					//Проверяем на обязательность данного поля
					if (fieldIssue[i].required)
					{
						label.Text += "*";
					}
					generalStackLayout.Children.Add(label);
					//Проверяем на кастомные поля
					if (fieldIssue[i].schema.custom.Length == 0)
					{
						//Если поле системное, то проверяем тип поля
						//Проверяем тип поля и выводим соответствующее отображение
						switch (fieldIssue[i].schema.type)
						{
							//Выгружаем список пользователей для данной задачи
							case "user":
								{
									List<User> user = new List<User>();
									List<string> userDisplayName = new List<string>();
									if (fieldIssue[i].autoCompleteUrl.Length > 0)
									{
										Request requestUser = new Request(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty)+$"/rest/api/latest/user/assignable/search?issueKey={numberIssue}&username=");
										user = requestUser.GetResponsersProfileList();

										for (int j = 0; j < user.Count; ++j)
										{
											userDisplayName.Add(user[j].displayName);
										}
									}

									//Создаем поисковый бар для поиска и отображения пользователей имеющих доступ к задаче
									SearchBar searchBar = new SearchBar
									{
										Placeholder = fieldIssue[i].defaultValue,
										TextColor = Color.FromHex("#F0F1F0"),
										PlaceholderColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										CancelButtonColor = Color.FromHex("#F0F1F0"),
										Margin = new Thickness(-25, 0, 0, 0),
										FontSize = 18
									};

									Grid grid = new Grid();
									ListView listView = new ListView()
									{
										IsVisible = false,
										VerticalOptions = LayoutOptions.Start,
										HeightRequest = 250,
										BackgroundColor = Color.FromHex("#4A4C50")

									};
									grid.Children.Add(listView);
									//Событие при вводе символов (показываем только тех пользователей, которые подходят к начатаму вводу пользователя)
									searchBar.TextChanged += (sender, args) =>
										{

											var keyword = searchBar.Text;
											if (keyword.Length >= 1)
											{


												var suggestion = userDisplayName.Where(c => c.ToLower().Contains(keyword.ToLower()));
												listView.ItemsSource = suggestion;
												listView.IsVisible = true;
											}
											else
											{
												listView.IsVisible = false;
											}
										};
									//Заполняем поле выбранным элементом из списка
									listView.ItemTapped += (sender, e) =>
									 {
										 if (e.Item as string == null)
										 {
											 return;
										 }
										 else
										 {
											 listView.ItemsSource = userDisplayName.Where(c => c.Equals(e.Item as string));
											 listView.IsVisible = true;
											 searchBar.Text = e.Item as string;
										 }
										 listView.IsVisible = false;
									 };

									generalStackLayout.Children.Add(searchBar);
									generalStackLayout.Children.Add(grid);
									break;
								}
							case "option":
							case "resolution":
								{
									List<string> resolutionValues = new List<string>();
									for (int j = 0; j < fieldIssue[i].allowedValues.Count; ++j)
									{
										resolutionValues.Add(fieldIssue[i].allowedValues[j].value);
									}
									Picker picker = new Picker
									{
										Title = fieldIssue[i].defaultValue,
										TextColor = Color.FromHex("#F0F1F0"),
										TitleColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 18
									};
									picker.Title = "Выберите значение...";
									picker.ItemsSource = resolutionValues;

									generalStackLayout.Children.Add(picker);
									break;
								}
							case "string":
								{
									Entry entry = new Entry()
									{
										Placeholder = "Введите текст",
										TextColor = Color.FromHex("#F0F1F0"),
										PlaceholderColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 18
									};

									generalStackLayout.Children.Add(entry);
									break;
								}
							case "array":
								{
									//Проверяем какой массив данных необходимо принять на вход
									switch (fieldIssue[i].schema.items)
									{
										case "attachment":
											{
												Button button = new Button()
												{
													Text = "Загрузить файлы...",
													HorizontalOptions = LayoutOptions.FillAndExpand,
													Margin = new Thickness(0, 0, 0, 20),
												};
												generalStackLayout.Children.Add(button);
												break;
											}
										case "issuelinks":
											{
												Picker picker = new Picker
												{
													Title = fieldIssue[i].defaultValue,
													TextColor = Color.FromHex("#F0F1F0"),
													TitleColor = Color.FromHex("#F0F1F0"),
													HorizontalOptions = LayoutOptions.FillAndExpand,
													Margin = new Thickness(0, 0, 0, 20),
													FontSize = 18
												};

												Request requestIssuelinks = new Request(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty)+$"/rest/api/2/issueLinkType");
												List<Issuelink> issuelinks = requestIssuelinks.GetResponses().issueLinkTypes;
												for (int j = 0; j < issuelinks.Count; ++j)
												{
													picker.Items.Add(issuelinks[j].outward);
													picker.Items.Add(issuelinks[j].inward);
												}
												picker.Title = "Выберите значение...";

												generalStackLayout.Children.Add(picker);



												List<Issue> issue = new List<Issue>();
												//List<string> userDisplayName = new List<string>();
												//if (fieldIssue[i].autoCompleteUrl.Length > 0)
												//{
												//	Request requestUser = new Request(fieldIssue[i].autoCompleteUrl);

												//	issue = requestUser.GetResponsersProfileList();

												//	for (int j = 0; j < issue.Count; ++j)
												//	{
												//		userDisplayName.Add(issue[j].key + " - " + issue[j].fields.summary);
												//	}
												//}

												////Создаем поисковый бар для поиска и отображения списка подходящих задач к данной для связывания
												//SearchBar searchBar = new SearchBar
												//{
												//	Placeholder = fieldIssue[i].defaultValue,
												//	TextColor = Color.FromHex("#F0F1F0"),
												//	PlaceholderColor = Color.FromHex("#F0F1F0"),
												//	HorizontalOptions = LayoutOptions.FillAndExpand,
												//	CancelButtonColor = Color.FromHex("#F0F1F0"),
												//	Margin = new Thickness(-25, 0, 0, 0),
												//	FontSize = 18
												//};

												//Grid grid = new Grid();
												//ListView listView = new ListView()
												//{
												//	IsVisible = false,
												//	VerticalOptions = LayoutOptions.Start,
												//	HeightRequest = 250,
												//	BackgroundColor = Color.FromHex("#4A4C50")

												//};
												//grid.Children.Add(listView);
												////Событие при вводе символов (показываем только тех пользователей, которые подходят к начатаму вводу пользователя)


												////TODO добавить при заполнении пересчет задач связанных
												//searchBar.TextChanged += (sender, args) =>
												//{
												//	var keyword = searchBar.Text;
												//	if (keyword.Length >= 1)
												//	{
												//		var suggestion = userDisplayName.Where(c => c.ToLower().Contains(keyword.ToLower()));
												//		listView.ItemsSource = suggestion;
												//		listView.IsVisible = true;
												//	}
												//	else
												//	{
												//		listView.IsVisible = false;
												//	}
												//};
												////Заполняем поле выбранным элементом из списка
												//listView.ItemTapped += (sender, e) =>
												//{
												//	if (e.Item as string == null)
												//	{
												//		return;
												//	}
												//	else
												//	{
												//		listView.ItemsSource = userDisplayName.Where(c => c.Equals(e.Item as string));
												//		listView.IsVisible = true;
												//		searchBar.Text = e.Item as string;
												//	}
												//	listView.IsVisible = false;
												//};

												//generalStackLayout.Children.Add(searchBar);
												//generalStackLayout.Children.Add(grid);






												break;
											}
									}
									break;
								}
							case "issuetype":
								{
									Label lblField = new Label()
									{
										Text = fieldIssue[i].allowedValues[0].value,
										TextColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 18
									};
									generalStackLayout.Children.Add(lblField);
									break;
								}
							case "date":
								{
									DatePicker datePicker = new DatePicker
									{
										TextColor = Color.FromHex("#F0F1F0"),
										Date = DateTime.Now
									};
									generalStackLayout.Children.Add(datePicker);
									break;
								}
						}
					}
					else
					{

					}
				}

			}
			//this.Content = layout;
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
	}
}
