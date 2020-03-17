using System;
using System.Collections.Generic;
using System.Linq;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Forms;

namespace RTMobile.issues
{
	public partial class CreateIssue : ContentPage
	{
		List<Project> projects { get; set; }
		List<Issuetype> typeIssue { get; set; }
		List<Fields> Fields { get; set; }
		public CreateIssue()
		{

			InitializeComponent();
			JSONRequest jsonRequest = new JSONRequest()
			{
				//urlRequest = $"/rest/api/2/project",
				//Получаем только проекты которые доступны пользователю
				urlRequest = $"/rest/api/2/issue/createmeta",
				methodRequest = "GET"
			};

			Request request = new Request(jsonRequest);

			RootObject rootObject = request.GetResponses<RootObject>();
			projects = rootObject.projects;

			List<string> projectName = new List<string>();
			for (int i = 0; i < projects.Count; ++i)
			{
				projectName.Add(projects[i].name);
			}

			projectPic.ItemsSource = projectName;
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

		private void Picker_SelectedIndexChanged(object sender, EventArgs e)
		{
			//В запросе на получение проектов получны данные о списке типов задач. Выгружать этот список присваивая Item picker значение списка из project.issueType
			if (projectPic.SelectedIndex != -1)
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/createmeta?projectKeys={projects[projectPic.SelectedIndex].key}",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				RootObject rootObject = request.GetResponses<RootObject>();
				if (rootObject != null && rootObject.projects != null && rootObject.projects.Count > 0)
				{
					typeIssue = rootObject.projects[0].issuetypes;
					List<string> typeIssueName = new List<string>();
					for (int i = 0; i < typeIssue.Count; ++i)
					{
						typeIssueName.Add(typeIssue[i].name);
					}
					typeIssuePic.ItemsSource = typeIssueName;
					typeIssuePic.ItemsSource = projects[projectPic.SelectedIndex].issuetypes;
					typeIssuePic.IsVisible = true;
					lblTypeIssue.IsVisible = true;
				}

			}
			//Добавить отрисовку полей
		}

		private void typeIssuePic_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (typeIssuePic.SelectedIndex != -1)
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/createmeta?projectKeys={projects[projectPic.SelectedIndex].key}&issuetypeNames={typeIssuePic.Items[typeIssuePic.SelectedIndex]}&expand=projects.issuetypes.fields",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				Fields = request.GetFieldScreenCreate();


				//Удалить поля которые могут быть добавлены с прошлого типа задач (сократить их количество на форме до 4 шт - lbl, project,lbl,issuetype



				for (int i = 0; i < Fields.Count; ++i)
				{
					//Проверяем на необходимость показа поля
					if (Fields[i].hasScreen)
					{
						//Создаем label с названием получаемого аргумента для более понятного вида для пользователя
						Label label = new Label
						{
							Text = Fields[i].displayName,
							TextColor = Color.FromHex("#F0F1F0"),
							FontSize = 14
						};
						//Проверяем на обязательность данного поля
						if (Fields[i].required)
						{
							label.Text += "*";
						}
						generalStackLayout.Children.Add(label);
						//Проверяем на кастомные поля
						if (Fields[i].schema.custom.Length == 0)
						{
							//Если поле системное, то проверяем тип поля
							//Проверяем тип поля и выводим соответствующее отображение
							switch (Fields[i].schema.type)
							{
								//Выгружаем список пользователей для данной задачи
								case "user":
									{
										List<User> user = new List<User>();
										List<string> userDisplayName = new List<string>();


										//Создаем поисковый бар для поиска и отображения пользователей имеющих доступ к задаче
										SearchBar searchBar = new SearchBar
										{
											Placeholder = Fields[i].defaultValue,
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
										searchBar.TextChanged += (senders, args) =>
										{
											var keyword = searchBar.Text;
											if (keyword.Length >= 1)
											{
												JSONRequest jsonRequestUser = new JSONRequest
												{
													urlRequest = $"/rest/api/2/user/picker?query=" + keyword.ToLower(),
													methodRequest = "GET"
												};
												Request requestUser = new Request(jsonRequestUser);

												user = requestUser.GetResponses<RootObject>().users;

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
										listView.ItemTapped += (senders, args) =>
										{
											if (args.Item as string == null)
											{
												return;
											}
											else
											{
												listView.ItemsSource = userDisplayName.Where(c => c.Equals(args.Item as string));
												listView.IsVisible = true;
												searchBar.Text = args.Item as string;
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
										for (int j = 0; j < Fields[i].allowedValues.Count; ++j)
										{
											resolutionValues.Add(Fields[i].allowedValues[j].value);
										}
										Picker picker = new Picker
										{
											Title = Fields[i].defaultValue,
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
										switch (Fields[i].schema.items)
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
														Title = Fields[i].defaultValue,
														TextColor = Color.FromHex("#F0F1F0"),
														TitleColor = Color.FromHex("#F0F1F0"),
														HorizontalOptions = LayoutOptions.FillAndExpand,
														Margin = new Thickness(0, 0, 0, 0),
														FontSize = 16,
													};

													JSONRequest jsonRequestLink = new JSONRequest
													{
														urlRequest = $"/rest/api/2/issueLinkType",
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
													if (Fields[i].autoCompleteUrl.Length > 0)
													{
														//Удаляем адрес сервера для получения только остаточной части запроса API
														Fields[i].autoCompleteUrl = Fields[i].autoCompleteUrl.Remove(0, (Fields[i].autoCompleteUrl.IndexOf(".ru") + 3));

														JSONRequest jsonRequestIssue = new JSONRequest
														{
															urlRequest = Fields[i].autoCompleteUrl,
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

													searchBar.TextChanged += (senders, args) =>
													{
														var keyword = searchBar.Text;
														if (keyword.Length >= 1)
														{

															JSONRequest jsonRequestIssue = new JSONRequest
															{
																urlRequest = $"/rest/api/2/user/picker?query=" + keyword.ToLower(),
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
													listView.ItemTapped += (senders, args) =>
													{
														if (args.Item as string == null)
														{
															return;
														}
														else
														{
															listView.ItemsSource = issueDisplayName.Where(c => c.Equals(args.Item as string));
															listView.IsVisible = true;
															searchBar.Text = args.Item as string;
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
							switch (Fields[i].schema.type)
							{
								case "option":
								case "resolution":
									{
										List<string> resolutionValues = new List<string>();
										for (int j = 0; j < Fields[i].allowedValues.Count; ++j)
										{
											resolutionValues.Add(Fields[i].allowedValues[j].value);
										}
										Picker picker = new Picker
										{
											Title = Fields[i].defaultValue,
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
										if (Fields[i].autoCompleteUrl.Length > 0)
										{
											JSONRequest jsonRequestUser = new JSONRequest
											{
												urlRequest = $"/rest/api/2/user/picker?query=",
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
											Placeholder = Fields[i].defaultValue,
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
										searchBar.TextChanged += (senders, args) =>
										{
											var keyword = searchBar.Text;
											if (keyword.Length >= 1)
											{
												JSONRequest jsonRequestIssue = new JSONRequest
												{
													urlRequest = $"/rest/api/2/user/picker?query=" + keyword.ToLower(),
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
										listView.ItemTapped += (senders, args) =>
										{
											if (args.Item as string == null)
											{
												return;
											}
											else
											{
												listView.ItemsSource = userDisplayName.Where(c => c.Equals(args.Item as string));
												listView.IsVisible = true;
												searchBar.Text = args.Item as string;
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

			}
		}

		private void Button_Clicked(object sender, EventArgs e)
		{

		}
	}
}
