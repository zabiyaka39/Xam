using Nancy;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.jiraData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class Transition : ContentPage
	{
		Dictionary<Guid, Fields> DectionaryFields = new Dictionary<Guid, Fields>();
		public List<Fields> Fields { get; set; }//Поля заявки
		MediaFile _mediaFile { get; set; }
		Label nameAttachmentLabel = new Label()
		{
			Text = "Отсутствует",
			TextColor = Color.FromHex("#F0F1F0"),
			FontSize = 18
		};
		string numberIssue { get; set; }
		int transitionId { get; set; }
		public Transition(int transitionId, string numberIssue, string idIssue)
		{
			InitializeComponent();

			this.numberIssue = numberIssue;
			this.transitionId = transitionId;

			JSONRequest jsonRequest = new JSONRequest
			{
				urlRequest = $"/rest/api/2/issue/{numberIssue}/transitions?expand=transitions.fields&transitionId=" + transitionId,
				methodRequest = "GET"
			};
			Request request = new Request(jsonRequest);

			Fields = request.GetFieldTransitions(idIssue);

			if (Fields == null || Fields.Count == 0)
			{
				string jsonRequestTransitions = "{ \"transition\": \"" + transitionId.ToString() + "\"}";
				//Совершаем переход с полученными данными
				JSONRequest jsonRequestTransition = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{numberIssue}/transitions",
					methodRequest = "POST"
				};
				Request requestTransition = new Request(jsonRequestTransition);

				Errors errors = requestTransition.GetResponses<Errors>(jsonRequestTransitions);
				if (errors == null || (errors.comment == null && errors.assignee == null))
				{
					Application.Current.MainPage = new AllIssues();
				}
			}
			else
			{
				Fields.Add(new Fields
				{
					name = "Комментарий",
					NameField = "comment",
					hasDefaultValue = false,
					schema = new Schema
					{
						type = "comment",
						system = "comment"
					},
					required = true

				});

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
										if (Field.allowedValues[j].value != null)
										{
											resolutionValues.Add(Field.allowedValues[j].value);
										}
										else
										{
											resolutionValues.Add(Field.allowedValues[j].name);
										}
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

								if (Field.allowedValues != null && Field.allowedValues[0].children != null)
								{
									Picker pickerChild = new Picker
									{
										Title = "Выберите значение...",
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
						case "comment":
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


		private async void Button_Clicked(object sender, EventArgs e)
		{
			string fields = "";
			//Переменная для отлова незаполненного обязательного поля

			//Создаем переменную для построения json-запроса для совершения перехода
			string jsonRequestTransitions = "{ \"transition\":{\"id\": \"" + transitionId.ToString() + "\"}";

			//Переменная для отлова незаполненного обязательного поля
			bool checkRequeredFields = true;

			JsonFields(generalStackLayout, ref fields, ref checkRequeredFields);
			JsonFields(necessarily_fields, ref fields, ref checkRequeredFields);


			if (fields.Length > 0)
			{
				jsonRequestTransitions += ",\"fields\":{" + fields + "}";
			}

			//Закрываем запрос
			jsonRequestTransitions += "}";
			if (checkRequeredFields)
			{
				//Совершаем переход с полученными данными
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{numberIssue}/transitions",
					methodRequest = "POST"
				};
				Request request = new Request(jsonRequest);

				Errors errors = request.GetResponses<Errors>(jsonRequestTransitions);
				if (errors == null || (errors.comment == null && errors.assignee == null))
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
								urlRequest = $"/rest/api/2/issue/{numberIssue}/attachments",
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
					MessagingCenter.Send<Page>(this, "RefreshIssueUpdate");
					await Navigation.PopAsync();
				}
			}
			else
			{
				errorMessage.IsVisible = true;
				errorMessage.Text = "Заполните обязательные поля помечанные *";
			}
		}

		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e) => await Navigation.PopAsync().ConfigureAwait(true);

		private void JsonFields(StackLayout Layout, ref string fields, ref bool checkRequeredFields)
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
						case "comment":
							{
								//Отправка комментария произваодится до осуществления перехода, т.к. во время перехода отправить поле с комментарием не возможно
								if (DectionaryFields[Layout.Children[i].Id].required == true)
								{
									if (((Entry)Layout.Children[i]).Text == null)
									{
										checkRequeredFields = false;
										break;
									}
								}
								if (((Entry)Layout.Children[i]).Text != null && ((Entry)Layout.Children[i]).Text.Length > 0)
								{
									JSONRequest jsonRequest = new JSONRequest
									{
										urlRequest = $"/rest/api/2/issue/{numberIssue}/comment",
										methodRequest = "POST"
									};
									Request request = new Request(jsonRequest);
									string comment = "{\"body\":\"" + ((Entry)Layout.Children[i]).Text + "\",\"properties\":[{\"key\":\"sd.public.comment\",\"value\":{\"internal\":false}}]}";
									request.GetResponses<Errors>(comment);
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
						case "array":
							{
								//Проверяем какой массив данных необходимо принять на вход
								switch (DectionaryFields[Layout.Children[i].Id].schema.items)
								{
									case "attachment":
										{									
											
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
	}
}
