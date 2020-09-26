using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.jiraData;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http.Headers;
using Nancy;
using Rg.Plugins.Popup.Services;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;

namespace RTMobile.issues
{
	public partial class CreateIssue : ContentPage
	{
		MediaFile _mediaFile { get; set; }
		List<Project> projects { get; set; }
		List<Fields> Fields { get; set; }

		ObservableCollection<User> user { get; set; }

		Label nameAttachmentLabel = new Label()
		{
			Text = "Отсутствует",
			TextColor = Color.FromHex("#F0F1F0"),
			FontSize = 18
		};
		public string Idf { get; set; }
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
				//Удаляем из типов задач подзадачи
				for (int j = 0; j < projects[i].issuetypes.Count; ++j)
				{
					if (projects[i].issuetypes[j].subtask == true)
					{
						projects[i].issuetypes.RemoveAt(j);
						--j;
					}
				}
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
				for (int i = 4; i < generalStackLayout.Children.Count;)
				{
					generalStackLayout.Children.RemoveAt(i);
				}
				for (int i = 0; i < necessarily_fields.Children.Count;)
				{
					necessarily_fields.Children.RemoveAt(i);
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
				for (int i = 4; i < generalStackLayout.Children.Count;)
				{
					generalStackLayout.Children.RemoveAt(i);
				}
				for (int i = 0; i < necessarily_fields.Children.Count;)
				{
					necessarily_fields.Children.RemoveAt(i);
				}

				buttonCreateIssue.IsEnabled = true;
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/createmeta?projectKeys={projects[projectPic.SelectedIndex].key}&issuetypeNames={typeIssuePic.Items[typeIssuePic.SelectedIndex]}&expand=projects.issuetypes.fields",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				Fields = request.GetFieldScreen();

				void determination_requered(Xamarin.Forms.StackLayout typeStack, Fields Field)
				{
					switch (Field.schema.type)
					{
						//Выгружаем список пользователей для данной задачи
						case "user":
						case "any":
							{
								ObservableCollection<User> user = new ObservableCollection<User>();
								ObservableCollection<InsightRoot> insights = new ObservableCollection<InsightRoot>();
								//Наименования пользователей или объектов Insight
								List<string> objectName = new List<string>();
								string urlRequestData = "";

								if (Field.autoCompleteUrl != null && Field.autoCompleteUrl.Length > 0)
								{
									urlRequestData = "/rest/api/2/user/picker?query=";

									JSONRequest jsonRequestUser = new JSONRequest
									{
										urlRequest = urlRequestData,
										methodRequest = "GET"
									};
									Request requestUser = new Request(jsonRequestUser);

									user = requestUser.GetResponses<RootObject>().users;

									for (int j = 0; j < user.Count; ++j)
									{
										objectName.Add(user[j].displayName);
									}
								}
								else
								{
									//Если это поле Insight, то применяем обработчик запроса информации по API Insight
									if (Field.schema.type.ToLower() == "any")
									{
										//Ищем подходящий номер схемы для поиска в Insight
										//Добавляем 18 символов которые обозначают изначальные данные поиска
										int startSearch = Field.editHtml.IndexOf("data-fieldconfig=") + 18;
										//Так же обрезаем на 19 символов для выбора только номера схемы в числовом формате
										int endSearch = Field.editHtml.IndexOf(" ", startSearch);
										int numberConfigSchema = -1;
										if (startSearch > -1 && endSearch > -1)
										{
											numberConfigSchema = Convert.ToInt32(Field.editHtml.Substring(startSearch, endSearch - startSearch - 1));
										}
										if (numberConfigSchema > -1)
										{
											//Ищем подходящую схему Insight, т.к может быть default или deprecated 
											int startSearchInsight = Field.editHtml.IndexOf("data-resource-path=") + 20;
											//Так же обрезаем на 19 символов для выбора только номера схемы в числовом формате
											int endSearchInsight = Field.editHtml.IndexOf(" ", startSearchInsight);

											urlRequestData = $"/rest/insight/1.0/{Field.editHtml.Substring(startSearchInsight, endSearchInsight - startSearchInsight - 1)}/{numberConfigSchema}/objects";


											JSONRequest jsonRequestInsight = new JSONRequest
											{
												urlRequest = urlRequestData,
												currentProject = projects[projectPic.SelectedIndex].id,
												methodRequest = "POST",
												currentReporter = CrossSettings.Current.GetValueOrDefault("login", string.Empty)

											};
											Request requestInsight = new Request(jsonRequestInsight);

											try
											{
												insights = requestInsight.GetResponses<RootObject>().objects;
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.Message);
											}
											for (int j = 0; j < user.Count; ++j)
											{
												objectName.Add(insights[j].Label);
											}
										}
									}
								}

								//Создаем поисковый бар для поиска и отображения пользователей имеющих доступ к задаче
								SearchBar searchBar = new SearchBar
								{
									Placeholder = "Заполните значение...",
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
										//Проверяем является ли данное поле Insight или это иное
										/** если "user", создается пользавательская ячейка с изображения пользователей и именем**/
										if (Field.schema.type == "user")
										{
											JSONRequest jsonRequestIssue = new JSONRequest
											{
												urlRequest = urlRequestData + keyword.ToLower(),
												methodRequest = "GET"
											};
											Request requestIssue = new Request(jsonRequestIssue);

											user = requestIssue.GetResponses<RootObject>().users;
											//создается привязка listview к коллекции "user"
											listView.ItemsSource = user;

											objectName.Clear();

											for (int j = 0; j < user.Count; ++j)
											{
												objectName.Add(user[j].displayName);
												user[j].avatarUrl = "https://sd.rosohrana.ru/secure/useravatar?ownerId=" + user[j].name;

											}
											//создлается пользовательская ячейка
											listView.ItemTemplate = new DataTemplate(() =>
											{
												// Ниже реализован загрузчик изображения с кэшированием
												CachedImage cacheImage = new CachedImage()
												{
													CacheDuration = TimeSpan.FromDays(7),
													DownsampleToViewSize = true,
													HeightRequest = 50,
													WidthRequest = 50,
													RetryCount = 0,
													RetryDelay = 250,
													BitmapOptimizations = false,
													Transformations = new System.Collections.Generic.List<FFImageLoading.Work.ITransformation>()
													{
														new CircleTransformation(),
													}

												};
												cacheImage.SetBinding(CachedImage.SourceProperty, "avatarUrl");
												Label name = new Label
												{
													TextColor = Color.White,
													VerticalOptions = LayoutOptions.Center


												};
												name.SetBinding(Label.TextProperty, "displayName");
												return new ViewCell
												{
													//создается структура пользовательской ячейки 
													View = new StackLayout
													{
														Padding = new Thickness(5, 5, 5, 5),
														Orientation = StackOrientation.Horizontal,
														Children = { cacheImage, name }

													}

												};

											});

											OnPropertyChanged(nameof(user));
											listView.IsVisible = true;



										}
										else
										{
											//Делаем запрос на поиск подходящих объектов
											JSONRequest jsonRequestInsightSearch = new JSONRequest
											{
												urlRequest = urlRequestData,
												currentProject = projects[projectPic.SelectedIndex].id,
												currentReporter = CrossSettings.Current.GetValueOrDefault("login", string.Empty),
												query = keyword.ToLower(),
												methodRequest = "POST"
											};
											Request requestIssue = new Request(jsonRequestInsightSearch);
											RootObject tmpRootObject = requestIssue.GetResponses<RootObject>();
											if (tmpRootObject.objects != null)
											{
												insights = tmpRootObject.objects;
											}

											objectName.Clear();

											for (int j = 0; j < insights.Count; ++j)
											{
												objectName.Add(insights[j].Label);
											}

											var suggestion = objectName.Where(c => c.ToLower().Contains(keyword.ToLower()));
											listView.ItemsSource = suggestion;

											listView.IsVisible = true;


										}


									}
									else
									{
										listView.IsVisible = false;
									}
								};

								//Заполняем поле выбранным элементом из списка
								listView.ItemTapped += (senders, args) =>
								{
									if (args.Item != null)
									{
										listView.IsVisible = true;
										User user_selecter = (User)args.Item;
										searchBar.Text = user_selecter.displayName;
									}
									listView.IsVisible = false;
								};
					 			typeStack.Children.Add(searchBar);
								typeStack.Children.Add(grid);
								DectionaryFields.Add(searchBar.Id, Field);
								break;
							}
						case "priority":
						case "option":
						case "option-with-child":
						case "resolution":
							{
								List<string> resolutionValues = new List<string>();
								if (Field.allowedValues != null)
								{
									for (int j = 0; j < Field.allowedValues.Count; ++j)
									{
										resolutionValues.Add(Field.allowedValues[j].value);
									}
								}
								Picker picker = new Picker
								{
									Title = "Выберите значение...",
									TextColor = Color.FromHex("#F0F1F0"),
									TitleColor = Color.FromHex("#F0F1F0"),
									HorizontalOptions = LayoutOptions.FillAndExpand,
									Margin = new Thickness(0, 0, 0, 0),
									FontSize = 16,
									ItemsSource = resolutionValues
								};

								//Если при выборе поля у него имеется "потомок" (доп. поле), то показываем его

								typeStack.Children.Add(picker);

								if (Field.allowedValues != null)
								{
									Picker pickerChild = new Picker
									{
										Title = "Заполните значение...",
										TextColor = Color.FromHex("#F0F1F0"),
										TitleColor = Color.FromHex("#F0F1F0"),
										HorizontalOptions = LayoutOptions.FillAndExpand,
										Margin = new Thickness(0, 0, 0, 20),
										FontSize = 16
									};
									pickerChild.Items.Add("Не заполнено");
									picker.SelectedIndexChanged += (senders, args) =>
									{
										if (picker.SelectedIndex > -1)
										{
											//Проверяем на наличие "потомков" 
											List<string> childName = new List<string>();
											Console.WriteLine(picker.Items[picker.SelectedIndex]);
											try
											{
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
											}
											catch (Exception ex)
											{
												Console.WriteLine(ex.Message);
											}
											pickerChild.ItemsSource = childName;
										}
									};
									typeStack.Children.Add(pickerChild);
								}
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

								typeStack.Children.Add(entry);
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
											button.Clicked += Button_Clicked1;
											typeStack.Children.Add(button);
											DectionaryFields.Add(button.Id, Field);
											break;
										}
									case "issuelinks":
										{
											Picker picker = new Picker
											{
												Title = "Выберите значение...",
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

											typeStack.Children.Add(picker);

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

											typeStack.Children.Add(searchBar);
											typeStack.Children.Add(grid);

											DectionaryFields.Add(searchBar.Id, Field);

											break;
										}
									case "component":
										{
											List<string> resolutionValues = new List<string>();
											for (int j = 0; j < Field.allowedValues.Count; ++j)
											{
												if (Field.allowedValues[j].name != null)
												{
													resolutionValues.Add(Field.allowedValues[j].name);
												}
											}
											Picker picker = new Picker
											{
												Title = "Выберите значение...",
												TextColor = Color.FromHex("#F0F1F0"),
												TitleColor = Color.FromHex("#F0F1F0"),
												HorizontalOptions = LayoutOptions.FillAndExpand,
												Margin = new Thickness(0, 0, 0, 20),
												FontSize = 16,
												ItemsSource = resolutionValues
											};

											typeStack.Children.Add(picker);
											DectionaryFields.Add(picker.Id, Field);
											break;
										}
								}
								break;
							}
						case "date":
						case "datetime":
							{
								DatePicker datePicker = new DatePicker
								{
									TextColor = Color.FromHex("#F0F1F0"),
									Date = DateTime.Now
								};
								typeStack.Children.Add(datePicker);

								DectionaryFields.Add(datePicker.Id, Field);
								break;
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
							Text = Fields[i].name,
							TextColor = Color.FromHex("#F0F1F0"),
							FontSize = 18
						};
						//Проверяем на обязательность данного поля
						if (Fields[i].required)
						{
							generalStackLayout.Children.Add(label);
							if (Fields[i].schema.type == "array" && Fields[i].schema.items == "attachment")
							{
								generalStackLayout.Children.Add(nameAttachmentLabel);
							}
							determination_requered(generalStackLayout, Fields[i]);
						}
						else
						{
							necessarily_fields.Children.Add(label);
							if (Fields[i].schema.type == "array" && Fields[i].schema.items == "attachment")
							{
								necessarily_fields.Children.Add(nameAttachmentLabel);
							}
							determination_requered(necessarily_fields, Fields[i]);
						}
						delimiter.IsVisible = true;
						extended_button.IsVisible = true;
					}

				}
			}
		}

		private async void Button_Clicked1(object sender, EventArgs e)
		{

			string st = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

			//Инициализируем проверку доступности разрешения работы с файловой системой
			await CrossMedia.Current.Initialize();

			string action = await DisplayActionSheet("Добавить данные", "Cancel", null, "Сделать фото", "Выбрать изображение");
			if (action == "Сделать фото")
			{
				if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
				{
					_mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
					{
						SaveToAlbum = true,
						Directory = "",
						Name = $"{DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss")}.jpg"
					});

				}
			}
			if (action == "Выбрать изображение")
			{
				if (CrossMedia.Current.IsPickPhotoSupported)
				{
					_mediaFile = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);
				}
			}

			if (_mediaFile != null)
			{
				nameAttachmentLabel.IsVisible = true;
				nameAttachmentLabel.Text = _mediaFile.Path;
			}
		}
		private void JsonFields(StackLayout Layout, ref string fields, ref bool checkRequeredFields)
		{
			try
			{
				//Проходимся по всем элементам на форме
				for (int i = 0; i < Layout.Children.Count && checkRequeredFields; ++i)
				{
					string str = Layout.Children[i].Id.ToString();
					if (DectionaryFields.ContainsKey(Layout.Children[i].Id))
					{
						switch (DectionaryFields[Layout.Children[i].Id].schema.type)
						{
							case "any":
								{
									if (DectionaryFields[Layout.Children[i].Id].required == true)
									{
										if (((SearchBar)Layout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
											break;
										}
									}
									//Увеличиваем счетчик для получения доступа к элементу после label
									ObservableCollection<InsightRoot> insights = new ObservableCollection<InsightRoot>();
									string urlRequestData = "";
									int startSearch = DectionaryFields[Layout.Children[i].Id].editHtml.IndexOf("data-fieldconfig=") + 18;
									//Так же обрезаем на 19 символов для выбора только номера схемы в числовом формате
									int endSearch = DectionaryFields[Layout.Children[i].Id].editHtml.IndexOf(" ", startSearch);
									int numberConfigSchema = -1;
									if (startSearch > -1 && endSearch > -1)
									{
										numberConfigSchema = Convert.ToInt32(DectionaryFields[Layout.Children[i].Id].editHtml.Substring(startSearch, endSearch - startSearch - 1));
									}
									if (numberConfigSchema > -1)
									{
										urlRequestData = $"/rest/insight/1.0/customfield/default/{numberConfigSchema}/objects";
									}

									JSONRequest jsonRequestInsightSearch = new JSONRequest
									{
										urlRequest = urlRequestData,
										currentReporter = CrossSettings.Current.GetValueOrDefault("login", string.Empty),
										query = ((SearchBar)Layout.Children[i]).Text,
										methodRequest = "POST"
									};
									Request requestIssue = new Request(jsonRequestInsightSearch);

									insights = requestIssue.GetResponses<RootObject>().objects;

									if (insights != null && insights.Count > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField + "\":[{\"key\":\"" + insights[0].ObjectKey + "\"}]";
									}
									break;
								}
							case "user":
								{
									if (DectionaryFields[Layout.Children[i].Id].required == true)
									{
										if (((SearchBar)Layout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
											break;
										}
									}
									//Увеличиваем счетчик для получения доступа к элементу после label
									List<User> user;
									JSONRequest jsonRequestIssue = new JSONRequest
									{
										urlRequest = $"/rest/api/2/user/picker?query=" + ((SearchBar)Layout.Children[i]).Text,
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
										fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField + "\":{\"name\":\"" + user[0].name + "\"}";
									}
									break;
								}
							case "priority":
							case "option":
							case "option-with-child":
								{
									if (((Picker)Layout.Children[i]).SelectedIndex == -1 && DectionaryFields[Layout.Children[i].Id].required == true)
									{
										checkRequeredFields = false;
										break;
									}

									if (((Picker)Layout.Children[i]).SelectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										//добавляем категорию
										fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField +
											"\":{\"value\":\"" + ((Picker)Layout.Children[i]).Items[((Picker)Layout.Children[i]).SelectedIndex] + "\"";


										if (DectionaryFields[Layout.Children[i].Id].allowedValues != null &&
											DectionaryFields[Layout.Children[i].Id].allowedValues[((Picker)Layout.Children[i]).SelectedIndex].children != null &&
											((Picker)Layout.Children[i + 1]).SelectedIndex > -1 &&
											((Picker)Layout.Children[i + 1]).Items[((Picker)Layout.Children[i + 1]).SelectedIndex] != null)
										{
											//Добавляем подкатегорию
											fields += ", \"child" +
													  "\":{\"value\":\"" + ((Picker)Layout.Children[i + 1]).Items[((Picker)Layout.Children[i + 1]).SelectedIndex] + "\"}";
										}
										fields += "}";
									}
									break;
								}
							case "resolution":
								{
									if (((Picker)Layout.Children[i]).SelectedIndex == -1 && DectionaryFields[Layout.Children[i].Id].required == true)
									{
										checkRequeredFields = false;
										break;
									}

									if (((Picker)Layout.Children[i]).SelectedIndex != -1)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField +
										"\":{\"id\":\"" + DectionaryFields[Layout.Children[i].Id].allowedValues[((Picker)Layout.Children[i]).SelectedIndex].id + "\"}";
									}
									break;
								}
							case "string":
								{
									if (DectionaryFields[Layout.Children[i].Id].required == true)
									{
										if (((Entry)Layout.Children[i]).Text == null)
										{
											checkRequeredFields = false;
											break;
										}
									}
									string strr = ((Entry)Layout.Children[i]).Text;
									if (((Entry)Layout.Children[i]).Text != null && ((Entry)Layout.Children[i]).Text.Length > 0)
									{
										if (fields.Length > 0)
										{
											fields += ", ";
										}
										fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField + "\":\"" + ((Entry)Layout.Children[i]).Text + "\"";

									}
									break;
								}
							case "array":
								{
									//Проверяем какой массив данных необходимо принять на вход
									switch (DectionaryFields[Layout.Children[i].Id].schema.items)
									{
										case "attachment":
											{
												break;
											}
										case "component":
											{
												if (DectionaryFields[Layout.Children[i].Id].required == true)
												{
													if (((SearchBar)Layout.Children[i]).Text == null && ((Picker)Layout.Children[i]).SelectedIndex == -1)
													{
														checkRequeredFields = false;
														break;
													}
												}
												int selectedIndex = ((Picker)Layout.Children[i]).SelectedIndex;
												if (selectedIndex != -1)
												{
													if (fields.Length > 0)
													{
														fields += ", ";
													}
													var saq = DectionaryFields[Layout.Children[i].Id].allowedValues[selectedIndex].id;
													fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField + "\":[{\"id\":\"" + DectionaryFields[Layout.Children[i].Id].allowedValues[selectedIndex].id + "\"}]";
												}											
												break;
											}
										case "issuelinks":
											{
												if (DectionaryFields[Layout.Children[i].Id].required == true)
												{
													if (((SearchBar)Layout.Children[i]).Text == null && ((Picker)Layout.Children[i]).SelectedIndex == -1)
													{
														checkRequeredFields = false;
														break;
													}
												}
												int selectedIndex = ((Picker)Layout.Children[i]).SelectedIndex;
												if (selectedIndex != -1)
												{
													if (fields.Length > 0)
													{
														fields += ", ";
													}
													fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField + "\":{\"name\":\"" + ((Picker)Layout.Children[i]).Items[selectedIndex] + "\"}";
												}
												//Увеличиваем счетчик полей на единицу, т.к. мы ранее создавали для этого типа поля два поля (searchBar и grid и picker)
												++i;
												if (((SearchBar)Layout.Children[i]).Text.Length > 0)
												{
													if (fields.Length > 0)
													{
														fields += ", ";
													}
													fields += "\"issuelinks\":{\"name\":\"" + ((SearchBar)Layout.Children[i]).Text + "\"}";
												}
												break;
											}
									}
									break;
								}
							case "date":
							case "datetime":
								{
									if (fields.Length > 0)
									{
										fields += ", ";
									}
									fields += "\"" + DectionaryFields[Layout.Children[i].Id].NameField + "\":\"" + ((DatePicker)Layout.Children[i]).Date.ToString("yyyy-MM-dd") + "\"";
									break;
								}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			try
			{
				//Создаем переменную для построения json-запроса для совершения перехода
				string jsonRequestCreate = "{ ";
				//Переменная для заполнения полей
				string fields = "";
				//Переменная для отлова незаполненного обязательного поля
				bool checkRequeredFields = true;

				//Проходимся по всем элементам на формах
				JsonFields(generalStackLayout, ref fields, ref checkRequeredFields);
				JsonFields(necessarily_fields, ref fields, ref checkRequeredFields);

				if (fields.Length > 0)
				{
					jsonRequestCreate += "\"fields\":{" + fields +
													  ", \"project\":{\"key\":\"" + projects[projectPic.SelectedIndex].key + "\"}," +
													  "\"issuetype\":{\"id\":\"" + projects[projectPic.SelectedIndex].issuetypes[typeIssuePic.SelectedIndex].id + "\"}" +
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
					try
					{
						AllowedValue allowedValue = request.GetResponses<AllowedValue>(jsonRequestCreate);

						if (allowedValue != null)
						{
							try
							{
								//Если имеются добавленные вложения, то после создания задачи добавляем эти вложения
								if (_mediaFile != null)
								{
									string boundary = DateTime.Now.Ticks.ToString("x");

									MultipartFormDataContent content = new MultipartFormDataContent(boundary);

									var streamContent = new StreamContent(_mediaFile.GetStream());
									//Задаем MimeType файлу
									streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypes.GetMimeType(_mediaFile.Path));

									content.Add(streamContent, "\"file\"", $"\"{_mediaFile.Path}\"");

									byte[] byteArray = await content.ReadAsByteArrayAsync().ConfigureAwait(true);
									JSONRequest jsonRequestAttachment = new JSONRequest()
									{
										urlRequest = $"/rest/api/2/issue/{allowedValue.key}/attachments",
										methodRequest = "POST",
										FileUpload = content,
										FileUploadByte = byteArray
									};
									//Отправка вложений в задачу
									Request requestAttachment = new Request(jsonRequestAttachment);
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.Message);
							}
							//Добавляем колесо загрузки
							try
							{
								await PopupNavigation.Instance.PushAsync(new StatusBar());
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.ToString());
							}
							//Открываем созданную задачу для просмотра
							await Navigation.PushAsync(new viewIssue.TabPageIssue(new Issue() { key = allowedValue.key }));
							try
							{
								await PopupNavigation.Instance.PopAsync(true);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex.ToString());
							}
							Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);

							MessagingCenter.Send<Page>(this, "RefreshIssueList");
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
				else
				{
					errorMessage.IsVisible = true;
					errorMessage.Text = "Заполните обязательные поля";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
