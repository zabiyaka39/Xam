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
				if (rootObject.session != null && rootObject.session.name != null)
				{
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
				// если в запррос передается файл, формируется http реквест
				if (jsonRequest.fileUploadJira != null)
				{
					string boundary = "-------------------" + DateTime.Now.Ticks.ToString("x");
					byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

					this.httpWebRequest.Headers.Add("X-Atlassian-Token", "nocheck");
					this.httpWebRequest.Method = "POST";
					this.httpWebRequest.KeepAlive = true;
					this.httpWebRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
					this.httpWebRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
					
					
					// сериализация http запроса и файла и запись в поток сетевого взаимодействия 
					
					Stream rs = this.httpWebRequest.GetRequestStream();
					
					rs.Write(boundarybytes, 0, boundarybytes.Length);
					string header = "Content-Disposition: form-data; name =\"file\"; filename=\"{0}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
					string headerTemplate = string.Format(header, jsonRequest.fileUploadJiraName);
					byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(headerTemplate);
					rs.Write(headerbytes, 0, headerbytes.Length);

					FileStream fileStream = new FileStream(jsonRequest.fileUploadJira, FileMode.Open, FileAccess.Read);
					byte[] buffer = new byte[4096];
					int bytesRead = 0;
					while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
					{
						rs.Write(buffer, 0, bytesRead);
					}
					fileStream.Close();
					
					
					byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
					rs.Write(trailer, 0, trailer.Length);
					rs.Close();

					using (var oResponse = (HttpWebResponse)this.httpWebRequest.GetResponse())
					{
						using (var reader = new StreamReader(oResponse.GetResponseStream()))
						{
							var responseData = reader.ReadToEnd();
						}
					}
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
		/// <summary>
		/// Метод для получения данных 
		/// </summary>
		/// <returns></returns>
		public T GetResponses<T>(string json = "")
		{
			T rootObject = default(T);
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

		private MediaFile _mediaFile;

		public async void uploadFile()
		{
			//Инициализируем проверку доступности разрешения работы с файловой системой
			await CrossMedia.Current.Initialize();
			//Выполняем поиск по системе необходимого файла
			if (CrossMedia.Current.IsPickPhotoSupported)
			{
				_mediaFile = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);
			}

			if (_mediaFile != null)
			{
				MultipartFormDataContent content = new MultipartFormDataContent();
				content.Add(new StreamContent(_mediaFile.GetStream()), "\"multipart/form-data\"", $"\"{_mediaFile.Path}\"");


				HttpWebRequest httpWebRequest1 = (HttpWebRequest)WebRequest.Create("https://sd.rosohrana.ru/rest/api/2/issue/IT-5744/attachments");
				httpWebRequest1.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("sekisov:28651455gsbua1A")));
				////this.httpWebRequest.Headers.Add("X-Atlassian-Token", "no-check");
				////this.httpWebRequest.Timeout = -1;
				using (StreamWriter streamWriter = new StreamWriter(httpWebRequest1.GetRequestStream()))
				{
					//streamWriter.Write("file", content);
				}
				WebResponse httpResponse = httpWebRequest1.GetResponse();
				//using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
				//{
				//	string result = streamReader.ReadToEnd();
				//}




				var client = new RestClient("https://sd.rosohrana.ru/rest/api/2/issue/IT-5744/attachments");
				client.Timeout = -1;
				var request = new RestRequest(Method.POST);
				request.AddHeader("X-Atlassian-Token", "no-check");
				request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("sekisov:28651455gsbua1A")));
				request.AddFile("file", _mediaFile.Path);
				IRestResponse response = client.Execute(request);
				Console.WriteLine(response.Content);
			}



		}







		/// <summary>
		/// Список полей при переходе
		/// </summary>
		/// <returns></returns>
		/// 

		public List<Fields> GetFieldScreenCreate()
		{
			List<Fields> fields = new List<Fields>();

			WebResponse httpResponse = this.httpWebRequest.GetResponse();
			//Отправляем запрос для получения списка полей задачи
			using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				//читаем поток
				string result = streamReader.ReadToEnd();
				//Создаем JAVA серелиазатор для возможности чтения элементов по названию, а не по полю класса, т.к. нам заранее не известны названия и количество полей в задаче и их количество может меняться
				JavaScriptSerializer js = new JavaScriptSerializer();
				//десериализуем в переменную с типом dynamic
				Nancy.Json.Simple.JsonObject objectCustomField = js.Deserialize<dynamic>(result);

				//проходимся по всем полученным customField и получаем значения

				try
				{
					foreach (System.Collections.Generic.KeyValuePair<string, object> field in objectCustomField)
					{
						if (field.Key == "projects")
						{
							if (((dynamic)(field.Value)).Count > 0)
							{
								//Проходимся по всем переходам
								for (int i = 0; i < ((dynamic)(field.Value)).Count; ++i)
								{
									foreach (KeyValuePair<string, object> fieldTransaction in ((dynamic)(field.Value))[i])
									{
										if (fieldTransaction.Key == "issuetypes")
										{
											if (((dynamic)(fieldTransaction.Value)).Count > 0)
											{
												for (int k = 0; k < ((dynamic)(fieldTransaction.Value)).Count; ++k)
												{
													foreach (KeyValuePair<string, object> fieldScreenCreate in ((dynamic)(fieldTransaction.Value))[k])
													{
														switch (fieldScreenCreate.Key)
														{
															case "id":
																{
																	//Получаем id перехода
																	break;
																}
															case "name":
																{
																	//Получаем название перехода
																	break;
																}
															case "to":
																{
																	//Получаем значения перехода
																	break;
																}
															case "fields":
																{
																	//Получаем поля перехода для заполнения
																	for (int j = 0; j < ((dynamic)(fieldScreenCreate.Value)).Count; ++j)
																	{
																		Fields fieldTmp = new Fields();

																		List<string> keysFields = new List<string>();
																		foreach (string nameField in ((dynamic)(fieldScreenCreate.Value)).Keys)
																		{
																			keysFields.Add(nameField);
																		}
																		foreach (KeyValuePair<string, object> fieldTransactionInformation in ((dynamic)(fieldScreenCreate.Value))[j])
																		{
																			fieldTmp.name = keysFields[j];
																			switch (fieldTransactionInformation.Key)
																			{
																				case "required":
																					{
																						fieldTmp.required = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "hasScreen":
																					{
																						fieldTmp.hasScreen = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "isGlobal":
																					{
																						fieldTmp.isGlobal = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "isInitial":
																					{
																						fieldTmp.isInitial = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "isAvailable":
																					{
																						fieldTmp.isAvailable = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "isConditional":
																					{
																						fieldTmp.isConditional = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "hasDefaultValue":
																					{
																						fieldTmp.hasDefaultValue = (bool)fieldTransactionInformation.Value;
																						break;
																					}
																				case "key":
																					{
																						fieldTmp.key = (string)fieldTransactionInformation.Value;
																						break;
																					}
																				case "schema":
																					{
																						Schema schema = new Schema();
																						int numberKey = 0;
																						foreach (string shemaNameField in ((dynamic)(fieldTransactionInformation.Value)).Keys)
																						{
																							switch (shemaNameField)
																							{
																								case "type":
																									{
																										schema.type = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																										break;
																									}
																								case "system":
																									{
																										schema.system = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																										break;
																									}
																								case "items":
																									{
																										schema.items = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																										break;
																									}
																								case "custom":
																									{
																										schema.custom = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																										break;
																									}
																								case "customId":
																									{
																										schema.customId = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																										break;
																									}
																							}
																							fieldTmp.schema = schema;

																							numberKey++;
																						}
																						break;
																					}
																				case "name":
																					{
																						fieldTmp.displayName = (string)fieldTransactionInformation.Value;
																						break;
																					}
																				case "defaultValue":
																					{
																						fieldTmp.defaultValue = (string)fieldTransactionInformation.Value;
																						break;
																					}
																				case "autoCompleteUrl":
																					{
																						fieldTmp.autoCompleteUrl = (string)fieldTransactionInformation.Value;
																						break;
																					}
																				case "allowedValues":
																					{
																						int numberKey = 0;
																						List<AllowedValue> allowedValuesIssue = new List<AllowedValue>();
																						for (int l = 0; l < ((dynamic)(fieldTransactionInformation.Value)).Count; ++l)
																						{
																							AllowedValue allowedValues = new AllowedValue();
																							foreach (KeyValuePair<string, object> allowedValueNameField in ((dynamic)(fieldTransactionInformation.Value))[l])
																							{
																								switch (allowedValueNameField.Key)
																								{
																									case "self":
																										{
																											allowedValues.self = (string)allowedValueNameField.Value;
																											break;
																										}
																									case "name":
																										{
																											allowedValues.value = (string)allowedValueNameField.Value;
																											break;
																										}
																									case "id":
																										{
																											allowedValues.id = (string)allowedValueNameField.Value;
																											break;
																										}
																									case "Value":
																										{
																											allowedValues.value = (string)allowedValueNameField.Value;
																											break;
																										}
																									case "avatarId":
																										{
																											allowedValues.avatarId = (long)allowedValueNameField.Value;
																											break;
																										}
																									case "subtask":
																										{
																											allowedValues.subtask = (bool)allowedValueNameField.Value;
																											break;
																										}
																									case "iconUrl":
																										{
																											allowedValues.iconUrl = (string)allowedValueNameField.Value;
																											break;
																										}
																									case "description":
																										{
																											allowedValues.description = (string)allowedValueNameField.Value;
																											break;
																										}
																									case "children":
																										{
																											//Добавляем к многомерному выпадающему списку параметры
																											List<Child> children = new List<Child>();
																											for (int h = 0; h < ((dynamic)(allowedValueNameField.Value)).Count; ++h)
																											{
																												children.Add(new Child());
																												foreach (KeyValuePair<string, object> fieldChildren in ((dynamic)(allowedValueNameField.Value))[h])
																												{
																													switch (fieldChildren.Key)
																													{
																														case "self":
																															{
																																children[h].self = (string)fieldChildren.Value;
																																break;
																															}
																														case "Value":
																															{
																																children[h].value = (string)fieldChildren.Value;
																																break;
																															}
																														case "id":
																															{
																																children[h].id = (string)fieldChildren.Value;
																																break;
																															}
																													}
																												}

																											}
																											allowedValues.children = children;
																											break;
																										}
																								}
																							}
																							allowedValuesIssue.Add(allowedValues);
																						}
																						fieldTmp.allowedValues = allowedValuesIssue;
																						numberKey++;
																						break;
																					}
																			}
																		}
																		if (fieldTmp.schema.type != "project" && fieldTmp.schema.type != "issuetype")
																		{
																			fields.Add(fieldTmp);
																		}
																	}
																	break;
																}
														}
													}
												}
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
					Crashes.TrackError(ex);
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
			List<Fields> fields = new List<Fields>();

			WebResponse httpResponse = this.httpWebRequest.GetResponse();
			//Отправляем запрос для получения списка полей задачи
			using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				//читаем поток
				string result = streamReader.ReadToEnd();
				//Создаем JAVA серелиазатор для возможности чтения элементов по названию, а не по полю класса, т.к. нам заранее не известны названия и количество полей в задаче и их количество может меняться
				JavaScriptSerializer js = new JavaScriptSerializer();
				//десериализуем в переменную с типом dynamic
				Nancy.Json.Simple.JsonObject objectCustomField = js.Deserialize<dynamic>(result);

				//string str = objectCustomField["transitions"][0][3];
				//проходимся по всем полученным customField и получаем значения
				try
				{
					foreach (System.Collections.Generic.KeyValuePair<string, object> field in objectCustomField)
					{
						if (field.Key == "transitions")
						{
							if (((dynamic)(field.Value)).Count > 0)
							{
								//Проходимся по всем переходам
								for (int i = 0; i < ((dynamic)(field.Value)).Count; ++i)
								{
									foreach (KeyValuePair<string, object> fieldTransaction in ((dynamic)(field.Value))[i])
									{
										switch (fieldTransaction.Key)
										{
											case "id":
												{
													//Получаем id перехода
													break;
												}
											case "name":
												{
													//Получаем название перехода
													break;
												}
											case "to":
												{
													//Получаем значения перехода
													break;
												}
											case "fields":
												{
													//Получаем поля перехода для заполнения
													for (int j = 0; j < ((dynamic)(fieldTransaction.Value)).Count; ++j)
													{
														Fields fieldTmp = new Fields();

														List<string> keysFields = new List<string>();
														foreach (string nameField in ((dynamic)(fieldTransaction.Value)).Keys)
														{
															keysFields.Add(nameField);
														}
														foreach (KeyValuePair<string, object> fieldTransactionInformation in ((dynamic)(fieldTransaction.Value))[j])
														{
															fieldTmp.name = keysFields[j];
															switch (fieldTransactionInformation.Key)
															{
																case "required":
																	{
																		fieldTmp.required = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "hasScreen":
																	{
																		fieldTmp.hasScreen = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "isGlobal":
																	{
																		fieldTmp.isGlobal = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "isInitial":
																	{
																		fieldTmp.isInitial = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "isAvailable":
																	{
																		fieldTmp.isAvailable = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "isConditional":
																	{
																		fieldTmp.isConditional = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "hasDefaultValue":
																	{
																		fieldTmp.hasDefaultValue = (bool)fieldTransactionInformation.Value;
																		break;
																	}
																case "key":
																	{
																		fieldTmp.key = (string)fieldTransactionInformation.Value;
																		break;
																	}
																case "schema":
																	{
																		Schema schema = new Schema();
																		int numberKey = 0;
																		foreach (string shemaNameField in ((dynamic)(fieldTransactionInformation.Value)).Keys)
																		{
																			switch (shemaNameField)
																			{
																				case "type":
																					{
																						schema.type = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																						break;
																					}
																				case "system":
																					{
																						schema.system = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																						break;
																					}
																				case "items":
																					{
																						schema.items = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																						break;
																					}
																				case "custom":
																					{
																						schema.custom = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																						break;
																					}
																				case "customId":
																					{
																						schema.customId = ((dynamic)(fieldTransactionInformation.Value))[numberKey];
																						break;
																					}
																			}
																			fieldTmp.schema = schema;

																			numberKey++;
																		}
																		break;
																	}
																case "name":
																	{
																		fieldTmp.displayName = (string)fieldTransactionInformation.Value;
																		break;
																	}
																case "defaultValue":
																	{
																		fieldTmp.defaultValue = (string)fieldTransactionInformation.Value;
																		break;
																	}
																case "autoCompleteUrl":
																	{
																		fieldTmp.autoCompleteUrl = (string)fieldTransactionInformation.Value;
																		break;
																	}
																case "allowedValues":
																	{
																		int numberKey = 0;
																		List<AllowedValue> allowedValuesIssue = new List<AllowedValue>();
																		for (int k = 0; k < ((dynamic)(fieldTransactionInformation.Value)).Count; ++k)
																		{
																			AllowedValue allowedValues = new AllowedValue();
																			foreach (KeyValuePair<string, object> allowedValueNameField in ((dynamic)(fieldTransactionInformation.Value))[k])
																			{
																				switch (allowedValueNameField.Key)
																				{
																					case "self":
																						{
																							allowedValues.self = (string)allowedValueNameField.Value;
																							break;
																						}
																					case "name":
																						{
																							allowedValues.value = (string)allowedValueNameField.Value;
																							break;
																						}
																					case "id":
																						{
																							allowedValues.id = (string)allowedValueNameField.Value;
																							break;
																						}
																					case "Value":
																						{
																							allowedValues.value = (string)allowedValueNameField.Value;
																							break;
																						}
																					case "avatarId":
																						{
																							allowedValues.avatarId = (long)allowedValueNameField.Value;
																							break;
																						}
																					case "subtask":
																						{
																							allowedValues.subtask = (bool)allowedValueNameField.Value;
																							break;
																						}
																					case "iconUrl":
																						{
																							allowedValues.iconUrl = (string)allowedValueNameField.Value;
																							break;
																						}
																					case "description":
																						{
																							allowedValues.description = (string)allowedValueNameField.Value;
																							break;
																						}
																				}
																			}
																			allowedValuesIssue.Add(allowedValues);
																		}
																		fieldTmp.allowedValues = allowedValuesIssue;
																		numberKey++;
																		break;
																	}
															}
														}
														fields.Add(fieldTmp);
													}
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
					Crashes.TrackError(ex);
				}
			}
			return fields;
		}

		/// <summary>
		/// Получаем список названий полей и значений задачи
		/// </summary>
		/// <returns></returns>
		public List<Fields> GetFieldsIssue(string json = "")
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
					JObject jsonConvert = JObject.Parse(streamReader.ReadToEnd());
					if (jsonConvert["fields"] != null)
					{
						if (jsonConvert["editmeta"] != null && jsonConvert["editmeta"]["fields"] != null)
						{
							foreach (var fields in jsonConvert["editmeta"]["fields"])
							{
								Schema schemaFields = JsonConvert.DeserializeObject<Schema>(fields.First["schema"].ToString());
								//Получаем имя поля в системе	
								string nameField = fields.First.Path.Substring(fields.First.Path.LastIndexOf(".") + 1);
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
