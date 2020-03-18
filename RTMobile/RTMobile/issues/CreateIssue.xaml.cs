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
		//Поле для более удобного поиска и доступа к полям (полученные поля = нарисованным полям, если нет доп. полей)
		Dictionary<Guid, Fields> DectionaryFields {get;set;}
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
			//Полученные ранее поля при выгрузке проектов добавляем как ItemSource
			//в тему задачи для выбранного типа проекта
			if (projectPic.SelectedIndex != -1)
			{
				List<string> typeIssueName = new List<string>();
				for (int i = 0; i < projects[projectPic.SelectedIndex].issuetypes.Count; ++i)
				{
					typeIssueName.Add(projects[projectPic.SelectedIndex].issuetypes[i].name);
				}
				//Все поля кроме первых 4 (тип и проект с label) удаляем
				for (int i = 4; i < generalStackLayout.Children.Count; ++i)
				{
					generalStackLayout.Children.RemoveAt(i);
				}
				typeIssuePic.ItemsSource = typeIssueName;
				//Показываем  label и выпадающий список с типами задач
				typeIssuePic.IsVisible = true;
				lblTypeIssue.IsVisible = true;
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

				//Все поля кроме первых 4 (тип и проект с label) удаляем
				for (int i = 4; i < generalStackLayout.Children.Count; ++i)
				{
					generalStackLayout.Children.RemoveAt(i);
				}

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

										DectionaryFields.Add(searchBar.Id, Fields[i]);

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
										DectionaryFields.Add(picker.Id, Fields[i]);
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
										DectionaryFields.Add(entry.Id, Fields[i]);
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
													DectionaryFields.Add(button.Id, Fields[i]);
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

													DectionaryFields.Add(searchBar.Id, Fields[i]);

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

										DectionaryFields.Add(datePicker.Id, Fields[i]);
										break;
									}
							}
						}
						else
						{
							switch (Fields[i].schema.type)
							{
								case "option-with-child":
									{
										List<string> optionChild = new List<string>();
										for (int j = 0; j < Fields[i].allowedValues.Count; ++j)
										{
											optionChild.Add(Fields[i].allowedValues[j].value);
										}
										Picker picker = new Picker
										{
											Title = Fields[i].defaultValue,
											TextColor = Color.FromHex("#F0F1F0"),
											TitleColor = Color.FromHex("#F0F1F0"),
											HorizontalOptions = LayoutOptions.FillAndExpand,
											Margin = new Thickness(0, 0, 0, 10),
											FontSize = 16
										};
										picker.Title = "Выберите значение...";
										picker.ItemsSource = optionChild;


										Picker pickerChild = new Picker
										{
											Title = Fields[i].defaultValue,
											TextColor = Color.FromHex("#F0F1F0"),
											TitleColor = Color.FromHex("#F0F1F0"),
											HorizontalOptions = LayoutOptions.FillAndExpand,
											Margin = new Thickness(0, 0, 0, 20),
											FontSize = 16
										};
										picker.Title = "Выберите значение...";
										//Присваиваем ID поля для дальнейшего поиска и выводу корректной информации
										Fields[i].idFieldScreen = picker.Id;
										picker.SelectedIndexChanged += (senders, args) =>
										{
											if (picker.SelectedIndex > -1)
											{
												int numberField = 0;
												for (numberField = 0; numberField < Fields.Count; ++numberField)
												{
													if (Fields[numberField].idFieldScreen == picker.Id)
													{
														break;
													}
												}
													if (Fields[numberField].allowedValues[picker.SelectedIndex].children != null)
													{
														List<string> childName = new List<string>();
														for (int j = 0; j < Fields[numberField].allowedValues[picker.SelectedIndex].children.Count; ++j)
														{
															childName.Add(Fields[numberField].allowedValues[picker.SelectedIndex].children[j].value);
														}
														pickerChild.ItemsSource = childName;
													}
											}
										};

										generalStackLayout.Children.Add(picker);
										generalStackLayout.Children.Add(pickerChild);

										DectionaryFields.Add(picker.Id, Fields[i]);
										break;
									}
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

										DectionaryFields.Add(picker.Id, Fields[i]);
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

										DectionaryFields.Add(datePicker.Id, Fields[i]);
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
										DectionaryFields.Add(entry.Id, Fields[i]);
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
										DectionaryFields.Add(entry.Id, Fields[i]);
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
										DectionaryFields.Add(searchBar.Id, Fields[i]);
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

			//Проходимся по всем полям
			//Проверяем наличие ключа и поля
			//Если есть - сравниваем со схемой 
			//В зависимости от схемы получаем значение 
			//Полученное значение добавляем в JSON для отправки













			string fields = "";
			string commentField = "";
			//Переменная для отлова незаполненного обязательного поля
			bool checkTransition = true;
			//Создаем переменную для построения json-запроса для совершения перехода
			string jsonRequestCreate = "{ ";
			for (int i = 1, j = 0; i < generalStackLayout.Children.Count && checkTransition; ++i)
			{
				//Увеличиваем значение только в том случае если поле не является label (шапкой/дополнением к основному полю)
				if (generalStackLayout.Children[i].GetType() != typeof(Label))
				{
					if (Fields[j].schema.custom.Length == 0)
					{
						//Если поле системное, то проверяем тип поля
						//Проверяем тип поля и выводим соответствующее отображение
						switch (Fields[j].schema.type)
						{
							//Выгружаем список пользователей для данной задачи
							case "user":
								{
									if (Fields[j].required == true)
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
										urlRequest = $"/rest/api/2/user/picker?query=" + ((SearchBar)generalStackLayout.Children[i]).Text,
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
										fields += "\"" + Fields[j].name + "\":{\"name\":\"" + user[0].name + "\"}";
									}
									//Увеличиваем счетчик полей на единицу, т.к. мы ранее создавали для этого типа поля два поля (searchBar и grid)
									++i;

									break;
								}
							case "priority":
							case "option":
							case "resolution":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && Fields[j].required == true)
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
										fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
									}
									break;
								}
							case "string":
								{

									if (Fields[j].required == true)
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
										fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "array":
								{
									//Проверяем какой массив данных необходимо принять на вход
									switch (Fields[j].schema.items)
									{
										case "attachment":
											{
												break;
											}
										case "issuelinks":
											{
												if (Fields[j].required == true)
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
													fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
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
									fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
									break;
								}
							case "comment":
								{
									if (Fields[j].required == true)
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
						switch (Fields[i].schema.type)
						{
							case "option-with-child":
								{
									i += 2;
									break;
								}
							case "option":
							case "resolution":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && Fields[j].required == true)
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
										fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
									}

									break;
								}
							case "datetime":
								{
									if (fields.Length > 0)
									{
										fields += ", ";
									}
									fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
									break;
								}
							case "string":
								{
									if (Fields[j].required == true)
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
										fields += "\"" + Fields[j].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "number":
								{
									if (Fields[j].required == true)
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
										jsonRequestCreate += "\"" + Fields[j].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "user":
								{
									if (Fields[j].required == true)
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
										urlRequest = $"/rest/api/2/user/picker?query=" + ((SearchBar)generalStackLayout.Children[i]).Text,
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
										fields += "\"" + Fields[j].name + "\":{\"name\":\"" + user[0].name + "\"}";
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
				jsonRequestCreate += ", \"fields\":{" + fields + "}";
			}
			if (commentField.Length > 0)
			{

				jsonRequestCreate += ", " + commentField;
			}
			//Закрываем запрос
			jsonRequestCreate += "}";
			if (checkTransition)
			{
				//Совершаем переход с полученными данными
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue",
					methodRequest = "POST"
				};
				Request request = new Request(jsonRequest);

				Errors errors = request.GetResponses<Errors>(jsonRequestCreate);
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
	}
}
