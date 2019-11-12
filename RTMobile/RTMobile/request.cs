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


namespace RTMobile
{
    class Request
    {
        private HttpWebRequest httpWebRequest = null;

        private string json { get; set; }
        /// <summary>
        /// Авторизация пользователя и возвращение упешности результата авторизации
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool authorization(string login, string password)
        {
            if (CrossSettings.Current.GetValueOrDefault<string>("urlServer").Length <= 0)
            {
                CrossSettings.Current.AddOrUpdateValue<string>("urlServer", "https://sd.rosohrana.ru");
            }
            Authorization authorization = new Authorization();
            authorization.username = login;
            authorization.password = password;

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault<string>("urlServer") + "/rest/auth/1/session");
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(login + ":" + password)));

            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "POST";
            this.json = JsonConvert.SerializeObject(authorization);

            RootObject rootObject = new RootObject();

            try
            {
                rootObject = this.GetResponses();
                if (rootObject.session.name != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// Пустой конструктор
        /// </summary>
        public Request()
        { }
        /// <summary>
        /// Запрос на список задач
        /// </summary>
        /// <param name="issueJSONSearch"></param>
        public Request(IssueJSONSearch issueJSONSearch)
        {
            //CrossSettings.Current.AddOrUpdateValue<string>("urlServer", "https://sd.rosohrana.ru");
            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault<string>("urlServer") + "/rest/api/2/search?");
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "POST";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault<string>("tmpLogin") + ":" + CrossSettings.Current.GetValueOrDefault<string>("tmpPassword"))));
            this.json = JsonConvert.SerializeObject(issueJSONSearch);
        }
        /// <summary>
        /// Запрос на список комментариев
        /// </summary>
        /// <param name="commentJSONSearch"></param>
        /// <param name="keyIssue"></param>
        public Request(CommentJSONSearch commentJSONSearch, string keyIssue)
        {
            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault<string>("urlServer") + "/rest/api/2/issue/" + keyIssue + "/comment");
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault<string>("tmpLogin") + ":" + CrossSettings.Current.GetValueOrDefault<string>("tmpPassword"))));
        }
        /// <summary>
        /// Запрос на получение списка комментариев
        /// </summary>
        /// <param name="comment"></param>
        /// <param name="keyIssue"></param>
        public Request(Comment comment, string keyIssue)
        {

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault<string>("urlServer") + "/rest/api/2/issue/" + keyIssue + "/comment/");

            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "POST";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault<string>("tmpLogin") + ":" + CrossSettings.Current.GetValueOrDefault<string>("tmpPassword"))));
            this.json = JsonConvert.SerializeObject(comment);
        }
        /// <summary>
        /// Запрос на получение списков проектов
        /// </summary>
        /// <param name="project"></param>
        public Request(Project project)
        {

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(CrossSettings.Current.GetValueOrDefault<string>("urlServer") + "/rest/api/2/project");

            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault<string>("tmpLogin" + ":" + CrossSettings.Current.GetValueOrDefault<string>("tmpPassword")))));
            this.json = JsonConvert.SerializeObject(project);

        }
        /// <summary>
        /// Универсальный GET запрос на получение информации
        /// </summary>
        /// <param name="getIssue"></param>
        public Request(string getIssue)
        {
            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(getIssue);
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(CrossSettings.Current.GetValueOrDefault<string>("tmpLogin") + ":" + CrossSettings.Current.GetValueOrDefault<string>("tmpPassword"))));
            this.json = "";
        }

        public RootObject GetResponses()
        {
            RootObject rootObject = new RootObject();

            if (this.json.Length > 0)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(this.json);
                }
            }
            try
            {
                var httpResponse = this.httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                    rootObject = JsonConvert.DeserializeObject<RootObject>(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("1111111111111111111111" + ex.Message);
            }

            return rootObject;
        }
        public RootObject GetResponsesF()
        {
            RootObject rootObject = new RootObject();

            var httpResponse = this.httpWebRequest.GetResponse();

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(this.json);
            }

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                rootObject = JsonConvert.DeserializeObject<RootObject>(result);

            }

            return rootObject;
        }
        public RootObject GetResponsersProfile()
        {
            RootObject rootObject = new RootObject();

            var httpResponse = this.httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                rootObject = JsonConvert.DeserializeObject<RootObject>(result);
            }

            return rootObject;
        }
        public List<Project> GetResponsesProject()
        {

            List<Project> rootObject = new List<Project>();

            var httpResponse = this.httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                rootObject = JsonConvert.DeserializeObject<List<Project>>(result);
            }

            return rootObject;
        }
        public RootObject GetResponses(string getIssue)
        {
            RootObject rootObject = new RootObject();

            var httpResponse = this.httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                rootObject = JsonConvert.DeserializeObject<RootObject>(result);
            }


            return rootObject;
        }
        /// <summary>
        /// Получаем список названий полей и значений задачи
        /// </summary>
        /// <returns></returns>
        public List<Fields> GetCustomField()
        {
            List<Fields> fields = new List<Fields>();

            Dictionary<string, string> keyValuePairsField = new Dictionary<string, string>();

            var httpResponse = this.httpWebRequest.GetResponse();
            RootObject rootObject = new RootObject();
            //Отправляем запрос для получения списка полей задачи
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //читаем поток
                var result = streamReader.ReadToEnd();
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
                                            //keyValuePairsField.Add(objectCustomField["names"][field.Key], valueCustomFeildOption.Value.ToString());
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
                                    //keyValuePairsField.Add(objectCustomField["names"][field.Key], arrayElement);
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
                                    if (field.Value.ToString().Trim(' ').Length >0 )
                                    {
                                        if (field.Key.ToLower() !="description" && field.Key.ToLower() != "summary" )
                                        {
                                            Fields tmpFiled = new Fields();
                                            tmpFiled.name = objectCustomField["names"][field.Key];
                                            tmpFiled.value = field.Value.ToString();
                                            fields.Add(tmpFiled);
                                            //keyValuePairsField.Add(objectCustomField["names"][field.Key], field.Value.ToString());
                                        }
                                    }
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
