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
                Project project = new Project
                {

                    includeArchived = false
                };
                string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/project";
                Request request = new Request(getIssue);

                projects = request.GetResponses<List<Project>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
            }
        }
    }
}