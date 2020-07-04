using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using FFImageLoading;
using FFImageLoading.Forms;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.jiraData;
using Service.Shared.Clients;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading.Tasks;

namespace RTMobile.issues
{
	public partial class CreateIssue : ContentPage
	{
		List<Project> projects { get; set; }
		List<Issuetype> typeIssue { get; set; }
		List<Fields> Fields { get; set; }
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
				for(int j=0; j<projects[i].issuetypes.Count;++j)
				{
					if(projects[i].issuetypes[j].subtask ==true)
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
								List<User> user = new List<User>();
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
												methodRequest = "POST"
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
									BackgroundColor = Color.FromHex("#4A4C50")

								};
								grid.Children.Add(listView);

								//Событие при вводе символов (показываем только тех пользователей, которые подходят к начатаму вводу пользователя)
								searchBar.TextChanged += (senders, args) =>
								{
									var keyword = searchBar.Text;
									if (keyword.Length >= 1)
									{
										//Проверяем является ли данное поле Insight или это иное
										if (Field.schema.type == "user")
										{
											JSONRequest jsonRequestIssue = new JSONRequest
											{
												urlRequest = urlRequestData + keyword.ToLower(),
												methodRequest = "GET"
											};
											Request requestIssue = new Request(jsonRequestIssue);

											user = requestIssue.GetResponses<RootObject>().users;

											objectName.Clear();

											for (int j = 0; j < user.Count; ++j)
											{
												objectName.Add(user[j].displayName);
											}

											var suggestion = objectName.Where(c => c.ToLower().Contains(keyword.ToLower()));
											listView.ItemsSource = suggestion;
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
									if (args.Item as string == null)
									{
										return;
									}
									else
									{
										listView.ItemsSource = objectName.Where(c => c.Equals(args.Item as string));
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
									Title = "Заполните значение...",
									TextColor = Color.FromHex("#F0F1F0"),
									TitleColor = Color.FromHex("#F0F1F0"),
									HorizontalOptions = LayoutOptions.FillAndExpand,
									Margin = new Thickness(0, 0, 0, 0),
									FontSize = 16
								};
								if (picker.Title.Length == 0)
								{
									picker.Title = "Выберите значение...";
								}
								picker.ItemsSource = resolutionValues;

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
											typeStack.Children.Add(button);
											DectionaryFields.Add(button.Id, Field);
											break;
										}
									case "issuelinks":
										{
											Picker picker = new Picker
											{
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
												Title = "Заполните значение...",
												TextColor = Color.FromHex("#F0F1F0"),
												TitleColor = Color.FromHex("#F0F1F0"),
												HorizontalOptions = LayoutOptions.FillAndExpand,
												Margin = new Thickness(0, 0, 0, 20),
												FontSize = 16
											};
											picker.Title = "Выберите значение...";
											picker.ItemsSource = resolutionValues;

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
			//Создаем переменную для построения json-запроса для совершения перехода
			string jsonRequestCreate = "{ ";
			//Переменная для заполнения полей
			string fields = "";
			//Переменная для отлова незаполненного обязательного поля
			bool checkRequeredFields = true;
			//Проходимся по всем элементам на форме
			for (int i = 0; i < generalStackLayout.Children.Count && checkRequeredFields; ++i)
			{
				if (DectionaryFields.ContainsKey(generalStackLayout.Children[i].Id))
				{
					switch (DectionaryFields[generalStackLayout.Children[i].Id].schema.type)
					{
						case "any":
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
								ObservableCollection<InsightRoot> insights = new ObservableCollection<InsightRoot>();
								string urlRequestData = "";
								int startSearch = DectionaryFields[generalStackLayout.Children[i].Id].editHtml.IndexOf("data-fieldconfig=") + 18;
								//Так же обрезаем на 19 символов для выбора только номера схемы в числовом формате
								int endSearch = DectionaryFields[generalStackLayout.Children[i].Id].editHtml.IndexOf(" ", startSearch);
								int numberConfigSchema = -1;
								if (startSearch > -1 && endSearch > -1)
								{
									numberConfigSchema = Convert.ToInt32(DectionaryFields[generalStackLayout.Children[i].Id].editHtml.Substring(startSearch, endSearch - startSearch - 1));
								}
								if (numberConfigSchema > -1)
								{
									urlRequestData = $"/rest/insight/1.0/customfield/default/{numberConfigSchema}/objects";
								}

								JSONRequest jsonRequestInsightSearch = new JSONRequest
								{
									urlRequest = urlRequestData,
									currentProject = projects[projectPic.SelectedIndex].id,
									currentReporter = CrossSettings.Current.GetValueOrDefault("login", string.Empty),
									query = ((SearchBar)generalStackLayout.Children[i]).Text,
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
									fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].NameField + "\":[{\"key\":\"" + insights[0].ObjectKey + "\"}]";
								}
								break;
							}
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
									fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].NameField + "\":{\"name\":\"" + user[0].name + "\"}";
								}
								break;
							}
						case "priority":
						case "option":
						case "option-with-child":
						case "resolution":
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
									fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].NameField + 
										"\":{\"value\":\"" + ((Picker)generalStackLayout.Children[i]).Items[((Picker)generalStackLayout.Children[i]).SelectedIndex];


									if (DectionaryFields[generalStackLayout.Children[i].Id].allowedValues != null &&
										DectionaryFields[generalStackLayout.Children[i].Id].allowedValues[((Picker)generalStackLayout.Children[i]).SelectedIndex].children != null &&
										((Picker)generalStackLayout.Children[i + 1]).SelectedIndex > -1 &&
										((Picker)generalStackLayout.Children[i + 1]).Items[((Picker)generalStackLayout.Children[i + 1]).SelectedIndex]!=null)
									{
										//Добавляем подкатегорию
										fields += "\", \"child" +
												  "\":{\"value\":\"" + ((Picker)generalStackLayout.Children[i + 1]).Items[((Picker)generalStackLayout.Children[i + 1]).SelectedIndex]+ "\"}";
									}
									fields += "}";
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
									fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].NameField + "\":\"" + ((Entry)generalStackLayout.Children[i]).Text + "\"";

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
												fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].NameField + "\":{\"name\":\"" + ((Picker)generalStackLayout.Children[i]).Items[selectedIndex] + "\"}";
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
						case "date":
						case "datetime":
							{
								if (fields.Length > 0)
								{
									fields += ", ";
								}
								fields += "\"" + DectionaryFields[generalStackLayout.Children[i].Id].NameField + "\":{\"name\":\"" + ((DatePicker)generalStackLayout.Children[i]).Date.ToString() + "\"}";
								break;
							}
					}
				}
			}

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
					
					Idf = request.GetResponses<AllowedValue>(jsonRequestCreate).key;
					Issue kEy = new Issue() {key = Idf };
					Navigation.PushAsync(new RTMobile.issues.viewIssue.TabPageIssue(kEy));
					Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
					MessagingCenter.Send<Page>(this, "RefreshIssueList");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

				/*Errors errors = request.GetResponses<Errors>(jsonRequestCreate);
				if (errors == null || (errors.comment == null && errors.assignee == null))
				{
					Application.Current.MainPage = new AllIssues();
					request = new Request(jsonRequest);
				}*/
			}
			else
			{
				errorMessage.IsVisible = true;
				errorMessage.Text = "Заполните обязательные поля";
			}
		}
	}
}
