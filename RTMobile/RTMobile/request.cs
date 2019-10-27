using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Text;


namespace RTMobile
{
    class Request
    {
        private HttpWebRequest httpWebRequest = null;

        private string login { get; set; }
        private string password { get; set; }
        private string json { get; set; }
        private string urlServer { get; set; }

        private void authorization()
        {
            this.login = "sekisov";
            this.password = "28651455gsbua1A";
            this.urlServer = "https://sd.rosohrana.ru";
        }

        public Request()
        { }
        /// <summary>
        /// Запрос на список задач
        /// </summary>
        /// <param name="issueJSONSearch"></param>
        public Request(IssueJSONSearch issueJSONSearch)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(this.urlServer + "/rest/api/2/search?");
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "POST";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
            this.json = JsonConvert.SerializeObject(issueJSONSearch);
        }
        /// <summary>
        /// Запрос ан список комментариев
        /// </summary>
        /// <param name="commentJSONSearch"></param>
        /// <param name="keyIssue"></param>
        public Request(CommentJSONSearch commentJSONSearch, string keyIssue)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServer + "/rest/api/2/issue/" + keyIssue + "/comment");
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
            //this.json = JsonConvert.SerializeObject(commentJSONSearch);
        }

        public Request(Comment comment, string keyIssue)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServer + "/rest/api/2/issue/" + keyIssue + "/comment/");

            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "POST";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
            this.json = JsonConvert.SerializeObject(comment);
        }

        public Request(Project project)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServer + "/rest/api/2/project");

            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
            this.json = JsonConvert.SerializeObject(project);

        }

        public Request(string getIssue)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(getIssue);
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
        }
        public RootObject GetResponses()
        {
            RootObject rootObject = new RootObject();

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(this.json);
            }
            var httpResponse = this.httpWebRequest.GetResponse();


            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                rootObject = JsonConvert.DeserializeObject<RootObject>(result);
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


    }
}
