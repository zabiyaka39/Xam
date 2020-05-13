using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FFImageLoading;
using FFImageLoading.Forms;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Service.Shared.Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.issues
{
	public partial class CreateIssue : ContentPage
	{
		List<Project> projects { get; set; }
		List<Issuetype> typeIssue { get; set; }
		List<Fields> Fields { get; set; }

		//Поле для более удобного поиска и доступа к полям (полученные поля = нарисованным полям, если нет доп. полей)
		Dictionary<Guid, Fields> DectionaryFields = new Dictionary<Guid, Fields>();
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
			this.BindingContext = this;
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
				buttonCreateIssue.IsEnabled = false;
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

		void button_show_necessarily(object sender, EventArgs e)
		{
			if (necessarily_fields.IsVisible == false)
			{
				necessarily_fields.IsVisible = true;
				extended_button.BackgroundColor = Color.Transparent;
				extended_button.Text = "Скрыть необязательные поля";

				extended_button.FontSize = 16;
				OnPropertyChanged(nameof(extended_button));
			}
			else
			{
				necessarily_fields.IsVisible = false;
				extended_button.BackgroundColor = Color.Transparent;
				extended_button.Text = "Показать необязательные поля";
				extended_button.FontSize = 16;
				OnPropertyChanged(nameof(extended_button));
			}
		}

		private void typeIssuePic_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (typeIssuePic.SelectedIndex != -1)
			{
				buttonCreateIssue.IsEnabled = true;
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

				void determination_requered(Xamarin.Forms.StackLayout type_stack, Fields Field)
				{
					if (Field.schema.custom.Length == 0)
					{
						//Если поле системное, то проверяем тип поля
						//Проверяем тип поля и выводим соответствующее отображение
						switch (Field.schema.type)
						{
							//Выгружаем список пользователей для данной задачи
							case "user":
								{
									List<User> user = new List<User>();
									List<User> userDisplayName = new List<User>();

									//Создаем поисковый бар для поиска и отображения пользователей имеющих доступ к задаче
									SearchBar searchBar = new SearchBar
									{
										Placeholder = Field.defaultValue,
										TextColor = Color.FromHex("#F0F1F0"),
										PlaceholderColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										CancelButtonColor = Color.FromHex("#F0F1F0"),
										Margin = new Thickness(-25, 0, 0, 0),
										FontSize = 16
									};

									Grid grid = new Grid();
									ListView listView = new ListView(ListViewCachingStrategy.RecycleElement)
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

													//for (int j = 0; j < user.Count; ++j)
													//{
													//	userDisplayName.Add(user[j].displayName);
													//}
													List<User> userTmp = new List<User>();
											List<ImageSource> imageSource = new List<ImageSource>();
											for (int n = 0; n < user.Count; ++n)
											{
												if (user[n].displayName.ToLower().Contains(keyword.ToLower()))
												{
													userTmp.Add(user[n]);
												}
											}
													//List<string> suggestion = userDisplayName.FindAll(c => c.ToLower().Contains(keyword.ToLower()));

													//listView.ItemsSource = userTmp;
													//listView.IsVisible = true;
													//listView.ItemTemplate = new DataTemplate(() =>
													//{
													//	var grids = new Grid();
													//	var nameLabel = new Label { FontAttributes = FontAttributes.Bold };
													//	var ageLabel = new Image();


													//	nameLabel.SetBinding(Label.TextProperty, "displayName");


													//Изображение с динамической загрузкой
													//CachedImage cachedImage = new CachedImage
													//{
													//	Source = "https://sd.rosohrana.ru/secure/useravatar?size=small&ownerId=safronov&avatarId=13507",
													//	LoadingPlaceholder = "drawable/about.png",
													//	ErrorPlaceholder = "Ошибка",
													//	WidthRequest = 150,
													//	HeightRequest = 150,
													//	RetryCount = 5,
													//	RetryDelay = 450,
													//	DownsampleHeight=20,
													//	DownsampleWidth = 20,
													//	DownsampleToViewSize = true

													//};


													//cachedImage.SetBinding(CachedImage.SourceProperty, "maxFormat");

													//grids.Children.Add(nameLabel);
													//grids.Children.Add(cachedImage, 2, 0);
													//return new ViewCell { View = grids };
													//ImageCell imageCell = new ImageCell();
													//imageCell.SetBinding(ImageCell.TextProperty, "displayName");
													//imageCell.SetBinding(ImageCell.ImageSourceProperty, "avatar");
													//return imageCell;
													//});

													listView.ItemsSource = userTmp;

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
											searchBar.Text = (args.Item as User).displayName;
										}
										listView.IsVisible = false;
									};

									//Добавляем все в грид для удобства поиска элементов при переходе
									type_stack.Children.Add(searchBar);
									type_stack.Children.Add(grid);

									DectionaryFields.Add(searchBar.Id, Field);

									break;
								}
							case "priority":
							case "option":
							case "resolution":
								{
									List<string> resolutionValues = new List<string>();
									for (int j = 0; j < Field.allowedValues.Count; ++j)
									{
										resolutionValues.Add(Field.allowedValues[j].value);
									}
									Picker picker = new Picker
									{
										Title = Field.defaultValue,
										TextColor = Color.FromHex("#F0F1F0"),
										TitleColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 16
									};
									picker.Title = "Выберите значение...";
									picker.ItemsSource = resolutionValues;

									type_stack.Children.Add(picker);
									DectionaryFields.Add(picker.Id, Field);
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

									type_stack.Children.Add(entry);
									DectionaryFields.Add(entry.Id, Field);

									break;
								}
							case "array":
								{
									//Проверяем какой массив данных необходимо принять на вход
									switch (Field.schema.items)
									{
										case "attachment":
											{
												Button button = new Button()
												{
													Text = "Загрузить файлы...",
													HorizontalOptions = LayoutOptions.FillAndExpand,
													Margin = new Thickness(0, 0, 0, 20),
												};
												type_stack.Children.Add(button);
												DectionaryFields.Add(button.Id, Field);
												break;
											}
										case "issuelinks":
											{
												Picker picker = new Picker
												{
													Title = Field.defaultValue,
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

												type_stack.Children.Add(picker);

												List<string> issueDisplayName = new List<string>();
												if (Field.autoCompleteUrl.Length > 0)
												{
													//Удаляем адрес сервера для получения только остаточной части запроса API
													Field.autoCompleteUrl = Field.autoCompleteUrl.Remove(0, (Field.autoCompleteUrl.IndexOf(".ru") + 3));

													JSONRequest jsonRequestIssue = new JSONRequest
													{
														urlRequest = Field.autoCompleteUrl,
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

												type_stack.Children.Add(searchBar);
												type_stack.Children.Add(grid);

												DectionaryFields.Add(searchBar.Id, Field);

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
									type_stack.Children.Add(datePicker);

									DectionaryFields.Add(datePicker.Id, Field);
									break;
								}
						}
					}
					else
					{
						switch (Field.schema.type)
						{
							case "option-with-child":
								{
									List<string> optionChild = new List<string>();
									for (int j = 0; j < Field.allowedValues.Count; ++j)
									{
										optionChild.Add(Field.allowedValues[j].value);
									}
									Picker picker = new Picker
									{
										Title = Field.defaultValue,
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
										Title = Field.defaultValue,
										TextColor = Color.FromHex("#F0F1F0"),
										TitleColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 16
									};
									picker.Title = "Выберите значение...";
									//Присваиваем ID поля для дальнейшего поиска и выводу корректной информации
									Field.idFieldScreen = picker.Id;
									picker.SelectedIndexChanged += (senders, args) =>
									{
										if (picker.SelectedIndex > -1)
										{
											List<string> childName = new List<string>();
											Console.WriteLine(picker.Items[picker.SelectedIndex]);
											for (int j = 0; j < DectionaryFields[picker.Id].allowedValues.Count; ++j)
											{
												if (DectionaryFields[picker.Id].allowedValues[j].value == picker.Items[picker.SelectedIndex])
												{
													for (int k = 0; k < DectionaryFields[picker.Id].allowedValues[j].children.Count; ++k)
													{
														childName.Add(DectionaryFields[picker.Id].allowedValues[j].children[k].value);
													}
												}
											}

											pickerChild.ItemsSource = childName;
										}
									};

									type_stack.Children.Add(picker);
									type_stack.Children.Add(pickerChild);

									DectionaryFields.Add(picker.Id, Field);
									break;
								}
							case "option":
							case "resolution":
								{
									List<string> resolutionValues = new List<string>();
									for (int j = 0; j < Field.allowedValues.Count; ++j)
									{
										resolutionValues.Add(Field.allowedValues[j].value);
									}
									Picker picker = new Picker
									{
										Title = Field.defaultValue,
										TextColor = Color.FromHex("#F0F1F0"),
										TitleColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 16
									};
									picker.Title = "Выберите значение...";
									picker.ItemsSource = resolutionValues;

									type_stack.Children.Add(picker);

									DectionaryFields.Add(picker.Id, Field);
									break;
								}
							case "datetime":
								{
									DatePicker datePicker = new DatePicker
									{
										TextColor = Color.FromHex("#F0F1F0"),
										Date = DateTime.Now
									};
									type_stack.Children.Add(datePicker);

									DectionaryFields.Add(datePicker.Id, Field);
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
									type_stack.Children.Add(entry);
									DectionaryFields.Add(entry.Id, Field);
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

									type_stack.Children.Add(entry);
									DectionaryFields.Add(entry.Id, Field);
									break;
								}
							case "user":
								{
									List<User> user = new List<User>();
									List<string> userDisplayName = new List<string>();
									if (Field.autoCompleteUrl.Length > 0)
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
										Placeholder = Field.defaultValue,
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
									type_stack.Children.Add(searchBar);
									type_stack.Children.Add(grid);
									DectionaryFields.Add(searchBar.Id, Field);
									break;
								}
							case "any":
								{
									break;
								}
						}
					}

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
							generalStackLayout.Children.Add(label);
							determination_requered(generalStackLayout, Fields[i]);
						}
						else
						{
							necessarily_fields.Children.Add(label);
							determination_requered(necessarily_fields, Fields[i]);
						}
						delimiter.IsVisible = true; 
						extended_button.IsVisible = true;
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

			//Переменнаяы для заполнения полей
			string fields = "";
			//Создаем переменную для построения json-запроса для совершения перехода
			string jsonRequestCreate = "{ ";
			//Переменная для отлова незаполненного обязательного поля
			bool checkRequeredFields = true;
			bool checkFieldGuid = false;
			for (int i = 0; i < generalStackLayout.Children.Count && checkRequeredFields == true; ++i, checkFieldGuid = false)
			{
				if (DectionaryFields.ContainsKey(generalStackLayout.Children[i].Id))
				{
					checkFieldGuid = true;
				}
				//Если поле есть (guid совпали), то проверяем тип и добавляем в запрос
				if (checkFieldGuid)
				{
					if (DectionaryFields[generalStackLayout.Children[i].Id].schema.custom.Length == 0)
					{
						//Если поле системное, то проверяем тип поля
						//Проверяем тип поля и выводим соответствующее отображение
						switch (DectionaryFields[generalStackLayout.Children[i].Id].schema.type)
						{
							//Выгружаем список пользователей для данной задачи
							case "user":
								{
									if (DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										if (((SearchBar)generalStackLayout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
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
									if (user != null && user.Count > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + user[0].name + "\"}";
									}
									break;
								}
							case "priority":
							case "option":
							case "resolution":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										checkRequeredFields = false;
										break;
									}
									int selectedIndex = ((Picker)generalStackLayout.Children[i]).SelectedIndex;

									if (selectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
									}
									break;
								}
							case "string":
								{

									if (DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text != null && ((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										//fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"";
									}
									break;
								}
							case "array":
								{
									//Проверяем какой массив данных необходимо принять на вход
									switch (DectionaryFields[generalStackLayout.Children[i].Id].schema.items)
									{
										case "attachment":
											{
												break;
											}
										case "issuelinks":
											{
												if (DectionaryFields[generalStackLayout.Children[i].Id].required == true)
												{
													if (((SearchBar)generalStackLayout.Children[i]).Text == null && ((Picker)generalStackLayout.Children[i]).SelectedIndex == -1)
													{
														checkRequeredFields = false;
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
													fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
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
									//fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
									fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString("yyyy-MM-dd") + "\"";
									break;
								}
						}
					}
					else
					{
						switch (DectionaryFields[generalStackLayout.Children[i].Id].schema.type)
						{
							case "option-with-child":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										checkRequeredFields = false;
										break;
									}

									if (((Picker)generalStackLayout.Children[i]).SelectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										//добавляем категорию
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[((Picker)generalStackLayout.Children[i]).SelectedIndex] + "\"}";

										if (DectionaryFields[generalStackLayout.Children[i].Id].allowedValues != null && DectionaryFields[generalStackLayout.Children[i].Id].allowedValues[((Picker)generalStackLayout.Children[i]).SelectedIndex].children != null)
										{
											//Добавляем подкатегорию
											fields += ", \"" + DectionaryFields[generalStackLayout.Children[i].Id].name + ":1" +
													  "\":{\"value\":\"" + ((Picker)generalStackLayout.Children[i + 1]).Items[((Picker)generalStackLayout.Children[i + 1]).SelectedIndex] + "\"}";
										}
									}
									break;
								}
							case "option":
							case "resolution":
								{
									if (((Picker)generalStackLayout.Children[i]).SelectedIndex == -1 && DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										checkRequeredFields = false;
										break;
									}
									int selectedIndex = ((Picker)generalStackLayout.Children[i]).SelectedIndex;

									if (selectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
									}
									break;
								}
							case "datetime":
								{
									if (fields.Length > 0)
									{
										fields += ", ";
									}
									fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
									break;
								}
							case "string":
								{
									if (DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text != null && ((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "number":
								{
									if (DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										if (((Entry)generalStackLayout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
											break;
										}
									}
									if (((Entry)generalStackLayout.Children[i]).Text != null && ((Entry)generalStackLayout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										jsonRequestCreate += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"}";
									}
									break;
								}
							case "user":
								{
									if (DectionaryFields[generalStackLayout.Children[i].Id].required == true)
									{
										if (((SearchBar)generalStackLayout.Children[i]).Text == null && ((Picker)generalStackLayout.Children[i]).SelectedIndex == -1)
										{
											checkRequeredFields = false;
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
									if (user != null && user.Count > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].name + "\":{\"name\":\"" + user[0].name + "\"}";
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
				}
			}

			if (fields.Length > 0)
			{
				jsonRequestCreate += "\"fields\":{" + fields +
												  ", \"project\":{\"key\":\"" + projects[projectPic.SelectedIndex].key + "\"}," +
												  "\"issuetype\":{\"name\":\"" + projects[projectPic.SelectedIndex].issuetypes[typeIssuePic.SelectedIndex].name + "\"}" +
												  "}";
			}

			//Закрываем запрос
			jsonRequestCreate += "}";
			if (checkRequeredFields)
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
