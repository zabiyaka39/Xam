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
    public partial class profile : ContentPage
    {
        public RootObject rootObject { get; set; }
        public List<Item> groups { get; set; }
        public profile()
        {
            InitializeComponent();

            issueStartPostRequest();

            this.BindingContext = this;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            CrossSettings.Current.Remove("login");
            CrossSettings.Current.Remove("password");
            CrossSettings.Current.Remove("tmpLogin");
            CrossSettings.Current.Remove("tmpPassword");
            CrossSettings.Current.Remove("CookieAuthJira");
            CrossSettings.Current.Remove("saveAuthorizationData");
            await Navigation.PopToRootAsync();
        }

        async void issueStartPostRequest()
        {
            try
            {
                string getIssue = CrossSettings.Current.GetValueOrDefault<string>("urlServer") + @"/rest/api/2/user?username=" + CrossSettings.Current.GetValueOrDefault<string>("tmpLogin") + @"&expand=groups,applicationRoles";
                Request request = new Request(getIssue);

                rootObject = request.GetResponsersProfile();

                groups = rootObject.groups.items;
                username.Text = CrossSettings.Current.GetValueOrDefault<string>("tmpLogin");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}