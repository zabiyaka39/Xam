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
		/// Строка с JSON - заропсом на сервер
		/// </summary>
		private string json { get; set; }
		/// <summary>
		/// Проверка сервера на доступность
		/// </summary>
		/// <returns></returns>
		public bool verifyServer()
		{
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

				if (CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty).Length <= 0)
				{
					CrossSettings.Current.AddOrUpdateValue("urlServer", "https://sd.rosohrana.ru");
				}
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
				this.httpWebRequest.ContentType = "application/json";
				this.httpWebRequest.Method = jsonRequest.methodRequest;
				this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " +
					Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault("login", string.Empty) +
					":" +
					CrossSettings.Current.GetValueOrDefault("password", string.Empty))));
				this.json = JsonConvert.SerializeObject(jsonRequest,
														Newtonsoft.Json.Formatting.None,
														new JsonSerializerSettings
														{
															NullValueHandling = NullValueHandling.Ignore
														});
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
					if (this.json != null && this.json.Length > 0)
					{
						if (json.Length > 0)
						{
							this.json = json;
						}
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
		/// Список полей при переходе
		/// </summary>
		/// <returns></returns>
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

				//string str = objectCustomField["transitions"][0][3];
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
																									case "value":
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
																														case "value":
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
																					case "value":
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
		public List<Fields> GetCustomField()
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
				dynamic objectCustomField = js.Deserialize<dynamic>(result);

				//проходимся по всем полученным customField и получаем значения
				foreach (System.Collections.Generic.KeyValuePair<string, object> field in objectCustomField)
				{
					if (field.Key == "fields")
					{
						if (((dynamic)(field.Value)).Count > 0)
						{
							for (int i = 0; i < ((dynamic)(field.Value)).Count; ++i)
							{
								foreach (KeyValuePair<string, object> fieldTransaction in ((dynamic)(field.Value))[i])
								{ 
								}
							}
						}
					}

					//Определяем тип выбранного поля
					//switch (field)
					//{
					//	//Добавляем customField если это выпадающий список
					//	case "option":
					//		{
					//			//проверяем на наличие заполнения поля
					//			if (field.Value != null)
					//			{
					//				dynamic keyValue = field.Value;
					//				foreach (System.Collections.Generic.KeyValuePair<string, object> valueCustomFeildOption in keyValue)
					//				{
					//					//ищем поле value для получения значения
					//					if (valueCustomFeildOption.Key == "value")
					//					{
					//						Fields tmpFiled = new Fields
					//						{
					//							name = objectCustomField["names"][field.Key],
					//							value = valueCustomFeildOption.Value.ToString()
					//						};
					//						fields.Add(tmpFiled);
					//						break;
					//					}
					//				}
					//			}
					//			break;
					//		}
					//	//Добавляем customField если это перечисление нескольких элементов (например insight)
					//	case "any":
					//		{
					//			if (field.Value != null)
					//			{
					//				dynamic keyValue = field.Value;
					//				string arrayElement = "";
					//				foreach (var arrayCustomField in keyValue)
					//				{
					//					arrayElement += arrayCustomField + "\n";
					//				}
					//				arrayElement = arrayElement.Trim('\n');
					//				Fields tmpFiled = new Fields
					//				{
					//					name = objectCustomField["names"][field.Key],
					//					value = arrayElement.ToString()
					//				};
					//				fields.Add(tmpFiled);
					//			}
					//			break;
					//		}
					//	//Добавляем customField если это число или строка
					//	case "number":
					//	case "string":
					//		{
					//			if (field.Value != null)
					//			{
					//				//Убираем пустые элементы
					//				if (field.Value.ToString().Trim(' ').Length > 0)
					//				{
					//					if (field.Key.ToLower() != "description" && field.Key.ToLower() != "summary")
					//					{
					//						Fields tmpFiled = new Fields
					//						{
					//							name = objectCustomField["names"][field.Key],
					//							value = field.Value.ToString()
					//						};
					//						fields.Add(tmpFiled);
					//					}
					//				}
					//			}
					//			break;
					//		}
					//	//Добавляем customField если это многоуровневый список (2 уровня)
					//	case "option-with-child":
					//		{
					//			if (field.Value != null)
					//			{
					//				dynamic keyValue = field.Value;
					//				Fields tmpFiled = new Fields();
					//				foreach (System.Collections.Generic.KeyValuePair<string, object> valueCustomFeildOption in keyValue)
					//				{
					//					//ищем поле value для получения значения
					//					if (valueCustomFeildOption.Key.ToLower() == "value")
					//					{
					//						tmpFiled.name = objectCustomField["names"][field.Key];
					//						tmpFiled.value = valueCustomFeildOption.Value.ToString();
					//					}
					//					if (valueCustomFeildOption.Key.ToLower() == "child")
					//					{
					//						dynamic child = valueCustomFeildOption.Value;
					//						foreach (System.Collections.Generic.KeyValuePair<string, object> valueCustomFeildOptionChild in child)
					//						{
					//							if (valueCustomFeildOptionChild.Key.ToLower() == "value")
					//							{
					//								tmpFiled.value += "-" + valueCustomFeildOptionChild.Value.ToString();
					//							}
					//						}
					//					}
					//				}
					//				fields.Add(tmpFiled);
					//			}
					//			break;
					//		}
					//}
				}
			}
			return fields;
		}
	}
}
