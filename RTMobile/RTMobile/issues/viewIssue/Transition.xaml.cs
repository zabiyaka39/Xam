﻿using Plugin.Settings;
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
		string numberIssue { get; set; }
		int transitionId { get; set; }
		public Transition(int transitionId, string numberIssue)
		{
			InitializeComponent();

			this.numberIssue = numberIssue;
			this.transitionId = transitionId;

			JSONRequest jsonRequest = new JSONRequest
			{
				urlRequest = new Uri($"/rest/api/2/issue/{numberIssue}/transitions?expand=transitions.fields&transitionId=" + transitionId),
				methodRequest = "GET"
			};
			Request request = new Request(jsonRequest);

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
										JSONRequest jsonRequestUser = new JSONRequest
										{
											urlRequest = new Uri($"/rest/api/latest/user/assignable/search?issueKey={numberIssue}&username="),
											methodRequest = "GET"
										};
										Request requestUser = new Request(jsonRequestUser);

										user = requestUser.GetResponses<List<User>>();

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
										FontSize = 16
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

									//Добавляем все в грид для удобства поиска элементов при переходе

									generalStackLayout.Children.Add(searchBar);
									generalStackLayout.Children.Add(grid);


									break;
								}
							case "priority":
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
										FontSize = 16
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
										FontSize = 16
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
													Margin = new Thickness(0, 0, 0, 0),
													FontSize = 16,
												};

												JSONRequest jsonRequestLink = new JSONRequest
												{
													urlRequest = new Uri($"/rest/api/2/issueLinkType"),
													methodRequest = "GET"
												};
												Request requestIssuelinks = new Request(jsonRequestLink);

												List<Issuelink> issuelinks = requestIssuelinks.GetResponses<RootObject>().issueLinkTypes;
												for (int j = 0; j < issuelinks.Count; ++j)
												{
													picker.Items.Add(issuelinks[j].outward);
													picker.Items.Add(issuelinks[j].inward);
												}
												picker.Title = "Выберите значение...";

												generalStackLayout.Children.Add(picker);

												List<string> issueDisplayName = new List<string>();
												if (fieldIssue[i].autoCompleteUrl.Length > 0)
												{
													//Удаляем адрес сервера для получения только остаточной части запроса API
													fieldIssue[i].autoCompleteUrl = fieldIssue[i].autoCompleteUrl.Remove(0, (fieldIssue[i].autoCompleteUrl.IndexOf(".ru") + 3));

													JSONRequest jsonRequestIssue = new JSONRequest
													{
														urlRequest = new Uri(fieldIssue[i].autoCompleteUrl),
														methodRequest = "GET"
													};
													Request requestIssue = new Request(jsonRequestIssue);

													List<Issue> issue = requestIssue.GetResponses<RootObject>().sections[0].issues;

													for (int j = 0; j < issue.Count; ++j)
													{
														issueDisplayName.Add(issue[j].key + " - " + issue[j].summary);
													}
												}

												//Создаем поисковый бар для поиска и отображения списка подходящих задач к данной для связывания
												SearchBar searchBar = new SearchBar
												{
													Placeholder = "Поиск по истории",
													TextColor = Color.FromHex("#F0F1F0"),
													PlaceholderColor = Color.FromHex("#F0F1F0"),
													HorizontalOptions = LayoutOptions.FillAndExpand,
													CancelButtonColor = Color.FromHex("#F0F1F0"),
													Margin = new Thickness(-25, 0, 0, 0),
													FontSize = 16
												};

												Grid grid = new Grid();
												ListView listView = new ListView()
												{
													IsVisible = false,
													VerticalOptions = LayoutOptions.Start,
													HeightRequest = 250,
													BackgroundColor = Color.FromHex("#4A4C50"),
												};
												grid.Children.Add(listView);
												//Событие при вводе символов (показываем только тех пользователей, которые подходят к начатаму вводу пользователя)

												searchBar.TextChanged += (sender, args) =>
												{
													var keyword = searchBar.Text;
													if (keyword.Length >= 1)
													{

														JSONRequest jsonRequestIssue = new JSONRequest
														{
															urlRequest = new Uri($"/rest/api/2/issue/picker?currentProjectId=&showSubTaskParent=true&showSubTasks=true&currentIssueKey={numberIssue}&query=" + keyword.ToLower()),
															methodRequest = "GET"
														};
														Request requestIssue = new Request(jsonRequestIssue);

														List<Issue> issue = requestIssue.GetResponses<RootObject>().sections[0].issues;

														issueDisplayName.Clear();
														for (int j = 0; j < issue.Count; ++j)
														{
															issueDisplayName.Add(issue[j].key + " - " + issue[j].summaryText);
														}

														var suggestion = issueDisplayName.Where(c => c.ToLower().Contains(keyword.ToLower()));
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
														listView.ItemsSource = issueDisplayName.Where(c => c.Equals(e.Item as string));
														listView.IsVisible = true;
														searchBar.Text = e.Item as string;
													}
													listView.IsVisible = false;
												};

												generalStackLayout.Children.Add(searchBar);
												generalStackLayout.Children.Add(grid);

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
										FontSize = 16
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
						switch (fieldIssue[i].schema.type)
						{
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
										FontSize = 16
									};
									picker.Title = "Выберите значение...";
									picker.ItemsSource = resolutionValues;

									generalStackLayout.Children.Add(picker);
									break;
								}
							case "datetime":
								{
									DatePicker datePicker = new DatePicker
									{
										TextColor = Color.FromHex("#F0F1F0"),
										Date = DateTime.Now
									};
									generalStackLayout.Children.Add(datePicker);
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
										FontSize = 16
									};

									generalStackLayout.Children.Add(entry);
									break;
								}
							case "number":
								{
									Entry entry = new Entry()
									{
										Placeholder = "Введите значение",
										TextColor = Color.FromHex("#F0F1F0"),
										PlaceholderColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 16,
										Keyboard = Keyboard.Numeric
									};

									generalStackLayout.Children.Add(entry);
									break;
								}
							case "user":
								{
									List<User> user = new List<User>();
									List<string> userDisplayName = new List<string>();
									if (fieldIssue[i].autoCompleteUrl.Length > 0)
									{
										JSONRequest jsonRequestUser = new JSONRequest
										{
											urlRequest = new Uri($"/rest/api/2/user/picker?query="),
											methodRequest = "GET"
										};
										Request requestUser = new Request(jsonRequestUser);

										user = requestUser.GetResponses<RootObject>().users;

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
										FontSize = 16
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
											JSONRequest jsonRequestIssue = new JSONRequest
											{
												urlRequest = new Uri($"/rest/api/2/user/picker?query=" + keyword.ToLower()),
												methodRequest = "GET"
											};
											Request requestIssue = new Request(jsonRequestIssue);

											user = requestIssue.GetResponses<RootObject>().users;

											userDisplayName.Clear();

											for (int j = 0; j < user.Count; ++j)
											{
												userDisplayName.Add(user[j].displayName);
											}

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
							case "any":
								{
									break;
								}
						}
					}
				}

			}

			fieldIssue.Add(new Fields
			{
				name = "comment",
				schema = new Schema
				{
					type = "comment",
					system = "comment"
				},
				required = true

			});

			//Создаем label с названием получаемого аргумента для более понятного вида для пользователя
			Label labelComment = new Label
			{
				Text = "Комментарий*",
				TextColor = Color.FromHex("#F0F1F0"),
				FontSize = 14
			};
			generalStackLayout.Children.Add(labelComment);
			Entry comment = new Entry()
			{
				Placeholder = "Введите текст",
				TextColor = Color.FromHex("#F0F1F0"),
				PlaceholderColor = Color.FromHex("#F0F1F0"),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Margin = new Thickness(0, 0, 0, 20),
				FontSize = 16
			};

			generalStackLayout.Children.Add(comment);
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
		private void Button_Clicked(object sender, EventArgs e)
		{
			string fields = "";
			string commentField = "";
			//Переменная для отлова незаполненного обязательного поля
			bool checkTransition = true;
			//Создаем переменную для построения json-запроса для совершения перехода
			string jsonRequestTransitions = "{ \"transition\": \"" + transitionId.ToString() + "\"";

			for (int i = 1, j = 0; i < generalStackLayout.Children.Count && checkTransition; ++i)
			{
				//Увеличиваем значение только в том случае если поле не является label (шапкой/дополнением к основному полю)
				if (generalStackLayout.Children[i].GetType() != typeof(Label))
				{
					if (fieldIssue[j].schema.custom.Length == 0)
					{
						//Если поле системное, то проверяем тип поля
						//Проверяем тип поля и выводим соответствующее отображение
						switch (fieldIssue[j].schema.type)
						{
							//Выгружаем список пользователей для данной задачи
							case "user":
								{
									if (fieldIssue[j].required == true)
									{
										if (((SearchBar)generalStackLayout.Children[i]).Text == null)
										{
											checkTransition = false;
											break;
										}
									}
									//Увеличиваем счетчик для получения доступа к элементу после label
									List<User> user;
									JSONRequest jsonRequestIssue = new JSONRequest
									{
										urlRequest = new Uri($"/rest/api/latest/user/assignable/search?issueKey={numberIssue}&username=" + ((SearchBar)generalStackLayout.Children[i]).Text),
										methodRequest = "GET"
									};
									Request requestUser = new Request(jsonRequestIssue);

									user = requestUser.GetResponses<List<User>>();
									if (user.Count > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + user[0].name + "\"}";
									}
									//Увеличиваем счетчик полей на единицу, т.к. мы ранее создавали для этого типа поля два поля (searchBar и grid)
									++i;

									break;
								}
							case "priority":
							case "option":
							case "resolution":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && fieldIssue[j].required == true)
									{
										checkTransition = false;
										break;
									}
									int selectedIndex = ((Picker)generalStackLayout.Children[i]).SelectedIndex;

									if (selectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
									}
									break;
								}
							case "string":
								{

									if (fieldIssue[j].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null)
										{
											checkTransition = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "array":
								{
									//Проверяем какой массив данных необходимо принять на вход
									switch (fieldIssue[j].schema.items)
									{
										case "attachment":
											{
												break;
											}
										case "issuelinks":
											{
												if (fieldIssue[j].required == true)
												{
													if (((SearchBar)generalStackLayout.Children[i]).Text == null && ((Picker)generalStackLayout.Children[i]).SelectedIndex == -1)
													{
														checkTransition = false;
														i += 2;
														break;
													}
												}
												int selectedIndex = ((Picker)generalStackLayout.Children[i]).SelectedIndex;
												if (selectedIndex != -1)
												{
													if (fields.Length > 0)
													{
														fields += ", ";
													}
													fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
												}
												//Увеличиваем счетчик полей на единицу, т.к. мы ранее создавали для этого типа поля два поля (searchBar и grid и picker)
												++i;
												if (((SearchBar)generalStackLayout.Children[i]).Text.Length > 0)
												{
													if (fields.Length > 0)
													{
														fields += ", ";
													}
													fields += "\"issuelinks\":{\"name\":\"" + ((SearchBar)generalStackLayout.Children[i]).Text + "\"}";
												}
												//Увеличиваем счетчик полей на единицу, т.к. мы ранее создавали для этого типа поля два поля (searchBar и grid и picker)
												++i;

												break;
											}
									}
									break;
								}
							//Нет реализации для данного поля
							case "issuetype":
								{
									break;
								}
							case "date":
								{
									if (fields.Length > 0)
									{
										fields += ", ";
									}
									fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
									break;
								}
							case "comment":
								{
									if (fieldIssue[j].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null)
										{
											checkTransition = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										commentField = "\"update\":{\"comment\":[{\"add\":{\"body\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}}]}";
									}

									break;
								}
						}
					}
					else
					{
						switch (fieldIssue[i].schema.type)
						{
							case "option":
							case "resolution":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && fieldIssue[j].required == true)
									{
										checkTransition = false;
										break;
									}
									int selectedIndex = ((Picker)generalStackLayout.Children[i]).SelectedIndex;

									if (selectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
									}

									break;
								}
							case "datetime":
								{
									if (fields.Length > 0)
									{
										fields += ", ";
									}
									fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
									break;
								}
							case "string":
								{
									if (fieldIssue[j].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null )
										{
											checkTransition = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "number":
								{
									if (fieldIssue[j].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null)
										{
											checkTransition = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										jsonRequestTransitions += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "user":
								{
									if (fieldIssue[j].required == true)
									{
										if (((SearchBar)generalStackLayout.Children[i]).Text == null && ((Picker)generalStackLayout.Children[i]).SelectedIndex == -1)
										{
											checkTransition = false;
											break;
										}
									}
									//Увеличиваем счетчик для получения доступа к элементу после label
									List<User> user;

									JSONRequest jsonRequestUser = new JSONRequest
									{
										urlRequest = new Uri($"/rest/api/latest/user/assignable/search?issueKey={numberIssue}&username=" + ((SearchBar)generalStackLayout.Children[i]).Text),
										methodRequest = "GET"
									};
									Request requestUser = new Request(jsonRequestUser);

									user = requestUser.GetResponses<List<User>>();
									if (user.Count > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + fieldIssue[j].name + "\":{\"name\":\"" + user[0].name + "\"}";
									}

									//Увеличиваем счетчик полей на единицу, т.к. мы ранее создавали для этого типа поля два поля (searchBar и grid)
									++i;
									break;
								}
							case "any":
								{
									break;
								}
						}
					}
					++j;
				}
			}

			if (fields.Length > 0)
			{
				jsonRequestTransitions += ", \"fields\":{" + fields + "}";
			}
			if (commentField.Length > 0)
			{

				jsonRequestTransitions += ", " + commentField;
			}
			//Закрываем запрос
			jsonRequestTransitions += "}";
			if (checkTransition)
			{
				//Совершаем переход с полученными данными
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = new Uri($"/rest/api/2/issue/{numberIssue}/transitions"),
					methodRequest = "POST"
				};
				Request request = new Request(jsonRequest);

				Errors errors = request.GetResponses<Errors>(jsonRequestTransitions);
				if (errors == null || (errors.comment == null && errors.assignee == null))
				{
					Application.Current.MainPage = new AllIssues();
				}
			}
			else
			{
				errorMessage.IsVisible = true;
				errorMessage.Text = "Заполните обязательные поля помечанные *";
			}
		}

		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			await Navigation.PopAsync().ConfigureAwait(true);
		}
	}
}
