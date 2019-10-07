using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Net.Http;

namespace RTMobile
{
  

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IssuePage : ContentPage
    {
        public IssuePage()
        {
            InitializeComponent();

            try
            {
                IssueJSONSearch issueJSONSearch = new IssueJSONSearch
                {
                    jql = "key = TELECOM-1216",
                    maxResults = 1,
                    startAt = 0
                };
                Request request = new Request(issueJSONSearch);
                RootObject rootObject = request.GetResponses(issueJSONSearch);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            

        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new searchIssue());
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new createIssue());
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new general());            
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new profile());
        }

        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new notifications());
        }

        private async void ImageButton_Clicked_2(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new insight());
        }
    }
}