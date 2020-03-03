using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class createIssue : ContentPage
    {
        public List<Project> projects { get; set; }
        public createIssue()
        {
            InitializeComponent();
            issueStartPostRequest();
            this.BindingContext = this;
        }

        async void issueStartPostRequest()
        {
            try
            {
                JSONRequest jsonRequest = new JSONRequest();
                jsonRequest.urlRequest = $"/rest/api/2/project";
                jsonRequest.methodRequest = "GET";
                Request request = new Request(jsonRequest);

                projects = request.GetResponses<List<Project>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }
        }
    }
}