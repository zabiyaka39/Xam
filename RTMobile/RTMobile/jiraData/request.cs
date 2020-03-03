﻿//using Nancy.Json;
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
using Windows.Data.Json;
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
		private string json { get; set; }
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
				Authorization authorization = new Authorization();
				authorization.username = login;
				authorization.password = password;

				this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + "/rest/auth/1/session");
				this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(login + ":" + password)));

				this.httpWebRequest.ContentType = "application/json";
				this.httpWebRequest.Method = "POST";
				this.json = JsonConvert.SerializeObject(authorization);

				RootObject rootObject = new RootObject();

				rootObject = this.GetResponses<RootObject>();
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
		/// Вывод всех полей экрана
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Fields> GetFieldScreen()
		{
			ObservableCollection<Fields> fields = new ObservableCollection<Fields>();

			Dictionary<string, string> keyValuePairsField = new Dictionary<string, string>();

			WebResponse httpResponse = this.httpWebRequest.GetResponse();
			RootObject rootObject = new RootObject();
			//Отправляем запрос для получения списка полей задачи
			using (StreamReader streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				//читаем поток
				string result = streamReader.ReadToEnd();
				//Создаем JAVA серелиазатор для возможности чтения элементов по названию, а не по полю класса, т.к. нам заранее не известны названия и количество полей в задаче и их количество может меняться
				JavaScriptSerializer js = new JavaScriptSerializer();
				//десериализуем в переменную с типом dynamic
				dynamic objectTransitions = js.Deserialize<dynamic>(result);

				foreach (dynamic transitions in objectTransitions.transitions)
				{
					//Проверяем количество полей на 0
					if (transitions.fields.Count > 0)
					{
						//Проходим по всем полям
						for (int i = 0; i < transitions.fields.Count; ++i)
						{
							//Создаем список перечислений (например выпадающий список)
							List<AllowedValue> allowedValues = new List<AllowedValue>();
							//Наличие данных перечислений
							if (transitions.fields[i].allowedValues.Count > 0)
							{
								//Проходимся по всем перечислениям
								for (int j = 0; j < transitions.fields[i].allowedValues.Count; ++j)
								{
									//Добавляем данные
									allowedValues.Add(new AllowedValue
									{
										value = transitions.fields[i].allowedValues[j].name,
										id = transitions.fields[i].allowedValues[j].id,
										self = transitions.fields[i].allowedValues[j].self
									});
								}
							}
							//Создаем список операций
							List<string> operations = new List<string>();
							if (transitions.fields[i].operations.Count > 0)
							{
								//Добавляем все возможные операции по полю
								for (int j = 0; j < transitions.fields[i].operations.Count; ++j)
								{
									operations.Add(transitions.fields[i].operations[j]);
								}
							}

							//Добавляем список полей для заполнения
							fields.Add(new Fields
							{
								required = transitions.fields[i].required,
								schema = new Schema
								{
									type = transitions.fields[i].schema.type,
									system = transitions.fields[i].schema.system
								},
								name = transitions.fields[i].name,
								operations = operations,
								allowedValues = allowedValues

							});
						}
					}
				}


			}
			return fields;
		}
		/// <summary>
		/// Список полей при переходе
		/// </summary>
		/// <returns></returns>
		public List<Fields> GetFieldTransitions()
		{
			List<Fields> fields = new List<Fields>();

			Dictionary<string, string> keyValuePairsField = new Dictionary<string, string>();

			WebResponse httpResponse = this.httpWebRequest.GetResponse();
			RootObject rootObject = new RootObject();
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

			Dictionary<string, string> keyValuePairsField = new Dictionary<string, string>();

			WebResponse httpResponse = this.httpWebRequest.GetResponse();
			RootObject rootObject = new RootObject();
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
				foreach (System.Collections.Generic.KeyValuePair<string, object> field in objectCustomField["fields"])
				{
					//Определяем тип выбранного поля
					switch (objectCustomField["schema"][field.Key]["type"])
					{
						//Добавляем customField если это выпадающий список
						case "option":
							{
								//проверяем на наличие заполнения поля
								if (field.Value != null)
								{
									dynamic keyValue = field.Value;
									foreach (System.Collections.Generic.KeyValuePair<string, object> valueCustomFeildOption in keyValue)
									{
										//ищем поле value для получения значения
										if (valueCustomFeildOption.Key == "value")
										{
											Fields tmpFiled = new Fields();
											tmpFiled.name = objectCustomField["names"][field.Key];
											tmpFiled.value = valueCustomFeildOption.Value.ToString();
											fields.Add(tmpFiled);
											break;
										}
									}
								}
								break;
							}
						//Добавляем customField если это перечисление нескольких элементов (например insight)
						case "any":
							{
								if (field.Value != null)
								{
									dynamic keyValue = field.Value;
									string arrayElement = "";
									foreach (var arrayCustomField in keyValue)
									{
										arrayElement += arrayCustomField + "\n";
									}
									arrayElement = arrayElement.Trim('\n');
									Fields tmpFiled = new Fields();
									tmpFiled.name = objectCustomField["names"][field.Key];
									tmpFiled.value = arrayElement.ToString();
									fields.Add(tmpFiled);

								}
								break;
							}
						//Добавляем customField если это число или строка
						case "number":
						case "string":
							{
								if (field.Value != null)
								{
									//Убираем пустые элементы
									if (field.Value.ToString().Trim(' ').Length > 0)
									{
										if (field.Key.ToLower() != "description" && field.Key.ToLower() != "summary")
										{
											Fields tmpFiled = new Fields();
											tmpFiled.name = objectCustomField["names"][field.Key];
											tmpFiled.value = field.Value.ToString();
											fields.Add(tmpFiled);
										}
									}
								}
								break;
							}
						//Добавляем customField если это многоуровневый список (2 уровня)
						case "option-with-child":
							{
								if (field.Value != null)
								{
									dynamic keyValue = field.Value;
									Fields tmpFiled = new Fields();
									foreach (System.Collections.Generic.KeyValuePair<string, object> valueCustomFeildOption in keyValue)
									{
										//ищем поле value для получения значения
										if (valueCustomFeildOption.Key.ToLower() == "value")
										{
											tmpFiled.name = objectCustomField["names"][field.Key];
											tmpFiled.value = valueCustomFeildOption.Value.ToString();
										}
										if (valueCustomFeildOption.Key.ToLower() == "child")
										{
											dynamic child = valueCustomFeildOption.Value;
											foreach (System.Collections.Generic.KeyValuePair<string, object> valueCustomFeildOptionChild in child)
											{
												if (valueCustomFeildOptionChild.Key.ToLower() == "value")
												{
													tmpFiled.value += "-" + valueCustomFeildOptionChild.Value.ToString();
												}
											}
										}
									}


									fields.Add(tmpFiled);
								}
								break;
							}
					}

				}

			}
			return fields;
		}
	}
}
