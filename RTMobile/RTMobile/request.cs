using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RTMobile
{
    class Request
    {
        private HttpWebRequest httpWebRequest = null;

        private string login = "";
        private string password = "";
        private string json = "";

        private void authorization()
        {
            this.login = "sekisov";
            this.password = "28651455gsbua1A";
        }

        public Request(IssueJSONSearch issueJSONSearch)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create("https://sd.rosohrana.ru/rest/api/2/search?");
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "POST";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
            this.json = JsonConvert.SerializeObject(issueJSONSearch);
        }

        public Request(CommentJSONSearch commentJSONSearch)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create("https://sd.rosohrana.ru/rest/api/2/issue/" + commentJSONSearch.issueIdOrKey + "/comment");
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
            this.json = JsonConvert.SerializeObject(commentJSONSearch);
        }

        public Request(string getIssue)
        {
            authorization();

            this.httpWebRequest = (HttpWebRequest)WebRequest.Create(getIssue);
            this.httpWebRequest.ContentType = "application/json";
            this.httpWebRequest.Method = "GET";
            this.httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(this.login + ":" + this.password)));
        }

        public RootObject GetResponses(IssueJSONSearch issueJSONSearch)
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
