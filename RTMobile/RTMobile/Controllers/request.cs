//using Nancy.Json;
using Nancy.Json;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.CSharp;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Microsoft.AppCenter.Crashes;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Linq;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using static RTMobile.MainPage;
using RTMobile.Models;

namespace RTMobile
{
	class Request
	{
		private HttpWebRequest httpWebRequest = null;


		/// <summary>
		/// Пустой конструктор для авторизации
		/// </summary>
		public Request()
		{ }
		/// <summary>
		/// Строка с JSON - заропсом на серверА
		/// </summary>
		private string json { get; set; }
		/// <summary>
		/// Проверка сервера на доступность
		/// </summary>
		/// <returns></returns>
		public bool verifyServer()
		{
			if (CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) == null || CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty).Length <= 0)
			{
				CrossSettings.Current.AddOrUpdateValue("urlServer", "https://sd.rosohrana.ru");
			}
			Uri uri = new Uri(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty));
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(uri);
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			}
			//Если при попытке установить подключение произошли ошибки, то отлавливаем исключение и возвращаем false
			catch
			{
				return false;
			}
			return true;

		}
		/// <summary>
		/// Авторизация пользователя и возвращение упешности результата авторизации
		/// </summary>
		/// <param name="login"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public bool authorization(string login, string password)
		{
			if (login.Length > 0)
			{
				//CrossSettings.Current.AddOrUpdateValue("urlServer", "https://sd.rosohrana.ru");
				Authorization authorization = new Authorization
				{
					username = login,
					password = password
				};
				this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + "/rest/auth/1/session");
				this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(login + ":" + password)));
				this.httpWebRequest.ContentType = "application/json";
				this.httpWebRequest.Method = "POST";
				this.json = JsonConvert.SerializeObject(authorization);

				RootObject rootObject = this.GetResponses<RootObject>();
				if (rootObject != null && rootObject.session != null && rootObject.session.name != null)
				{
					try
					{
						JSONRequest jsonRequest = new JSONRequest()
						{
							urlRequest = $"/rest/api/2/user?username={CrossSettings.Current.GetValueOrDefault("login", string.Empty)}&expand=groups,applicationRoles",
							methodRequest = "GET"
						};
						Request request = new Request(jsonRequest);

						MeUser.User = request.GetResponses<User>();
						Event eventResp = new Event()
						{
							EventName = "Авториазция"
						};
						//Отправляем геолокацию при успешной авторизации
						//Geolocation(eventResp);
					}
					catch (Exception ex)
					{
						Crashes.TrackError(ex);
						Console.WriteLine(ex.ToString());
					}

					return true;
				}
				return false;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Создаем подключение для запроса данных
		/// </summary>
		/// <param name="jsonRequest"></param>
		public Request(JSONRequest jsonRequest)
		{
			try
			{
				this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + jsonRequest.urlRequest);
				this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " +
					Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) +
					":" +
					CrossSettings.Current.GetValueOrDefault("password", string.Empty))));
				this.httpWebRequest.KeepAlive = true;

				if (jsonRequest.FileUpload != null)
				{
					this.httpWebRequest.Headers.Add("X-Atlassian-Token", "nocheck");
					this.httpWebRequest.Method = "POST";
					this.httpWebRequest.ContentType = jsonRequest.FileUpload.Headers.ContentType.ToString();
					this.httpWebRequest.ContentLength = jsonRequest.FileUpload.Headers.ContentLength.Value;
					this.httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

					Stream requestStream = this.httpWebRequest.GetRequestStream();
					try
					{
						requestStream.Write(jsonRequest.FileUploadByte, 0, jsonRequest.FileUploadByte.Length);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						Crashes.TrackError(ex);
					}
					requestStream.Close();

					HttpWebResponse myHttpWebResponse = (HttpWebResponse)this.httpWebRequest.GetResponse();

					Stream responseStream = myHttpWebResponse.GetResponseStream();

					StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

					string pageContent = myStreamReader.ReadToEnd();

					myStreamReader.Close();
					responseStream.Close();

					myHttpWebResponse.Close();
				}
				else
				{
					this.httpWebRequest.Method = jsonRequest.methodRequest;
					this.httpWebRequest.ContentType = "application/json";
					this.json = JsonConvert.SerializeObject(jsonRequest,
														Newtonsoft.Json.Formatting.None,
														new JsonSerializerSettings
														{
															NullValueHandling = NullValueHandling.Ignore
														});
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}

		private async void Geolocation(Event eventResp)
		{
			try
			{
				if (eventResp == null)
				{
					eventResp = new Event()
					{
						EventName = "Отсутствует",
						NumberIssue = ""
					};
				}
				IGeolocator locator = CrossGeolocator.Current;
				locator.DesiredAccuracy = 50;
				TimeSpan timeSpan = new TimeSpan(10000);
				Position position = await locator.GetPositionAsync(timeSpan);
				Models.Location location = new Models.Location()
				{
					Latitude = position.Latitude,
					Longitude = position.Longitude
				};

				RequestEngineer requestEngineer = new RequestEngineer()
				{
					User = MeUser.User,
					Location = location,
					Event = eventResp
				};
				try
				{
					//Адрес сервера куда будет отправляться геолокация
					//string geoServerAdress = "172.18.43.89:52613/api/engineer";
					string geoServerAdress = "http://localhost:52613/api/engineer";

					HttpWebRequest httpWebRequestGeo = (HttpWebRequest)WebRequest.Create(geoServerAdress);

					httpWebRequestGeo.Method = "POST";
					httpWebRequestGeo.Headers.Add(HttpRequestHeader.Authorization, "Basic " +
																					Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) +
																					":" +
																					CrossSettings.Current.GetValueOrDefault("password", string.Empty))));
					httpWebRequestGeo.KeepAlive = true;
					httpWebRequestGeo.ContentType = "application/json";
					string jsonRequest = JsonConvert.SerializeObject(requestEngineer,
															Newtonsoft.Json.Formatting.None,
															new JsonSerializerSettings
															{
																NullValueHandling = NullValueHandling.Ignore
															});
					using (StreamWriter streamWriter = new StreamWriter(httpWebRequestGeo.GetRequestStream()))
					{
						streamWriter.Write(jsonRequest);
					}
					HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequestGeo.GetResponse();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Crashes.TrackError(ex);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Метод для получения данных 
		/// </summary>
		/// <returns></returns>
		public T GetResponses<T>(string json = "", Event eventResp = null)
		{
			//Проверяем прошли ли мы авторизацию, если прошли, то отправляем полноценный запрос на сервер для регистрации геолокации
			//if (MeUser.User != null)
			//{
			//	Geolocation(eventResp);
			//}
			T rootObject = default(T);
			try
			{
				if (httpWebRequest.Method == "POST" || httpWebRequest.Method == "PUT")
				{
					if (json != null && json.Length > 0)
					{
						this.json = json;
					}
					//Исключаем пустой JSON-запрос сформированный на этапе создания подключения к серверу
					if (this.json != null && this.json.Length > 2)
					{
						using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
						{
							streamWriter.Write(this.json);
						}
					}
				}
				WebResponse httpResponse = this.httpWebRequest.GetResponse();

				using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					string result = streamReader.ReadToEnd();
					rootObject = JsonConvert.DeserializeObject<T>(result);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
			return rootObject;
		}

		/// <summary>
		/// Список полей при создании задачи
		/// </summary>
		/// <returns></returns>
		public List<Fields> GetFieldScreen()
		{
			List<Fields> fields = new List<Fields>();

			WebResponse httpResponse = this.httpWebRequest.GetResponse();
			//Отправляем запрос для получения списка полей задачи
			using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				dynamic jsonConvert = JObject.Parse(streamReader.ReadToEnd());
				if (jsonConvert.projects != null)
				{
					RootObject rootObject = new RootObject();
					List<Fields> fieldsExtend = new List<Fields>();
					if (jsonConvert.projects[0].id != null && jsonConvert.projects[0].issuetypes != null && jsonConvert.projects[0].issuetypes[0] != null && jsonConvert.projects[0].issuetypes[0].id != null)
					{
						//Запрос на получение дополнительной информации по полям
						JSONRequest jsonRequestExtendedInformation = new JSONRequest()
						{
							//urlRequest = $"/rest/api/2/project",
							//Получаем только проекты которые доступны пользователю
							urlRequest = $"/secure/QuickCreateIssue!default.jspa?decorator=none&pid={jsonConvert.projects[0].id}&issuetype={jsonConvert.projects[0].issuetypes[0].id}",
							methodRequest = "GET"
						};

						Request requestExtended = new Request(jsonRequestExtendedInformation);
						WebResponse httpResponseExtended = requestExtended.httpWebRequest.GetResponse();
						using (StreamReader streamReaderExtended = new StreamReader(httpResponseExtended.GetResponseStream()))
						{
							dynamic jsonConvertExtended = JObject.Parse(streamReaderExtended.ReadToEnd());
							if (jsonConvertExtended.fields != null)
							{
								for (int i = 0; i < jsonConvertExtended.fields.Count; ++i)
								{
									string sss = jsonConvertExtended.fields[i].ToString();
									fieldsExtend.Add(JsonConvert.DeserializeObject<Fields>(jsonConvertExtended.fields[i].ToString()));
								}
							}
						}
					}

					if (jsonConvert.projects[0].issuetypes != null)
					{
						if (jsonConvert.projects[0].issuetypes[0].fields != null)
						{
							foreach (dynamic fieldsDeserializate in jsonConvert.projects[0].issuetypes[0].fields)
							{
								foreach (dynamic checkFieldsDeserializate in fieldsDeserializate)
								{
									fields.Add(JsonConvert.DeserializeObject<Fields>(checkFieldsDeserializate.ToString()));
									fields[fields.Count - 1].NameField = fieldsDeserializate.Name.ToString();
									//Если имеются системные поля то убираем их из списка на вывод
									if (fields[fields.Count - 1].schema.type == "project" || fields[fields.Count - 1].schema.type == "issuetype")
									{
										fields.RemoveAt(fields.Count - 1);
									}
									else
									{
										//если поле не удалено, то добавляем параметр editHtml полученный ранее
										int indexFind = fieldsExtend.FindIndex(fiel => fiel.Id == fields[fields.Count - 1].NameField);
										if (indexFind > -1)
										{
											fields[fields.Count - 1].editHtml = fieldsExtend[indexFind].editHtml;
										}
									}
								}
							}
						}
					}
				}
			}
			return fields;
		}

		/// <summary>
		/// Получаем список названий полей и значений задачи
		/// </summary>
		/// <returns></returns>
		/// <summary>
		/// Список полей при переходе
		/// </summary>
		/// <returns></returns>
		public List<Fields> GetFieldTransitions()
		{
			List<Fields> Fields = new List<Fields>();
			try
			{
				if (httpWebRequest.Method == "POST")
				{
					if (json != null && json.Length > 0)
					{
						this.json = json;
					}
					//Исключаем пустой JSON-запрос сформированный на этапе создания подключения к серверу
					if (this.json != null && this.json.Length > 2)
					{
						using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
						{
							streamWriter.Write(this.json);
						}
					}
				}
				WebResponse httpResponse = this.httpWebRequest.GetResponse();

				using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					dynamic jsonConvert = JObject.Parse(streamReader.ReadToEnd());
					string str = jsonConvert.ToString();
					if (jsonConvert.transitions != null && jsonConvert.transitions[0] != null && jsonConvert.transitions[0].fields != null)
					{
						foreach (dynamic fieldsDeserializate in jsonConvert.transitions[0].fields)
						{
							foreach (dynamic checkFieldsDeserializate in fieldsDeserializate)
							{
								Fields.Add(JsonConvert.DeserializeObject<Fields>(checkFieldsDeserializate.ToString()));
								Fields[Fields.Count - 1].NameField = fieldsDeserializate.Name.ToString();
							}
						}

					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return Fields;
		}

		/// <summary>
		/// Получаем список названий полей и значений задачи
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Fields> GetFieldsIssue(string json = "")
		{
			ObservableCollection<Fields> Fields = new ObservableCollection<Fields>();
			try
			{
				if (httpWebRequest.Method == "POST")
				{
					if (json != null && json.Length > 0)
					{
						this.json = json;
					}
					//Исключаем пустой JSON-запрос сформированный на этапе создания подключения к серверу
					if (this.json != null && this.json.Length > 2)
					{
						using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
						{
							streamWriter.Write(this.json);
						}
					}
				}
				WebResponse httpResponse = this.httpWebRequest.GetResponse();

				using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					dynamic jsonConvert = JObject.Parse(streamReader.ReadToEnd());
					if (jsonConvert.fields != null)
					{
						if (jsonConvert.editmeta != null && jsonConvert.editmeta.fields != null)
						{
							string str = jsonConvert.editmeta.fields.ToString();
							foreach (dynamic fields in jsonConvert.editmeta.fields)
							{
								Schema schemaFields = JsonConvert.DeserializeObject<Schema>(fields.First["schema"].ToString());
								//Получаем имя поля в системе	
								string nameField = fields.Name;
								//Проверяем является ли данное поле системным, если нет, то добавляем на экран просмотра информации
								if (schemaFields.customId > 0)
								{
									switch (schemaFields.type)
									{
										case "string":
											{
												Fields.Add(new Fields
												{
													//Заполняем значение поля
													value = jsonConvert["fields"][nameField].ToString(),
													//Заполняем имя поля в системе
													NameField = nameField,
													//Заполняем человекочитаемое имя поля для удобства отображения
													DisplayNameField = jsonConvert["names"][nameField].ToString()
												});
												//Проверяем на наличие элемента, если элемент равен null, то надо переопределить элемент
												//с именем и описанием, для удобочитаемого формата
												if (Fields[Fields.Count - 1] == null)
												{
													Fields[Fields.Count - 1] = new Fields()
													{
														DisplayNameField = jsonConvert["names"][nameField].ToString(),
														value = "Не определено",
														NameField = nameField,
														schema = schemaFields
													};
												}
												break;
											}
										case "array":
											{
												Fields.Add(JsonConvert.DeserializeObject<Fields>(jsonConvert["fields"][nameField][0].ToString()));

												//Проверяем на наличие элемента, если элемент равен null, то надо переопределить элемент
												//с именем и описанием, для удобочитаемого формата
												if (Fields[Fields.Count - 1] == null)
												{
													Fields[Fields.Count - 1] = new Fields()
													{
														value = "Не определено",
														schema = schemaFields
													};
												}
												//Изменяем значение поля, т.к. все значения должны храниться в Value, а в некоторых запросах получаем в Name или displayName (человекочитаемый формат)
												if (Fields[Fields.Count - 1].value == null)
												{
													if (Fields[Fields.Count - 1].displayName != null)
													{
														Fields[Fields.Count - 1].value = Fields[Fields.Count - 1].displayName;
													}
													else
													{
														if (Fields[Fields.Count - 1].name != null)
														{
															Fields[Fields.Count - 1].value = Fields[Fields.Count - 1].name;
														}
													}
												}
												Fields[Fields.Count - 1].DisplayNameField = jsonConvert["names"][nameField].ToString();
												Fields[Fields.Count - 1].NameField = nameField;
												break;
											}
										default:
											{
												Fields.Add(JsonConvert.DeserializeObject<Fields>(jsonConvert["fields"][nameField].ToString()));

												//Проверяем на наличие элемента, если элемент равен null, то надо переопределить элемент
												//с именем и описанием, для удобочитаемого формата
												if (Fields[Fields.Count - 1] == null)
												{
													Fields[Fields.Count - 1] = new Fields()
													{
														value = "Не определено",
														schema = schemaFields
													};
												}
												//Изменяем значение поля, т.к. все значения должны храниться в Value, а в некоторых запросах получаем в Name или displayName (человекочитаемый формат)
												if (Fields[Fields.Count - 1].value == null)
												{
													if (Fields[Fields.Count - 1].displayName != null)
													{
														Fields[Fields.Count - 1].value = Fields[Fields.Count - 1].displayName;
													}
													else
													{
														if (Fields[Fields.Count - 1].name != null)
														{
															Fields[Fields.Count - 1].value = Fields[Fields.Count - 1].name;
														}
													}
												}
												if (Fields[Fields.Count - 1].Child != null)
												{
													Fields[Fields.Count - 1].value += " - ";
												}
												Fields[Fields.Count - 1].NameField = nameField;
												Fields[Fields.Count - 1].DisplayNameField = jsonConvert["names"][nameField].ToString();
												break;
											}
									}
								}
							}
						}

					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return Fields;
		}
	}
}