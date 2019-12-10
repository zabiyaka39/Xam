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
		public profile(string user)
		{
			InitializeComponent();

			Title = "Профиль " +  issueStartPostRequest(user);

			this.BindingContext = this;
		}
		public profile()
        {
            InitializeComponent();

            issueStartPostRequest();

			Title = "Мой профиль";

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
            await Navigation.PopToRootAsync().ConfigureAwait(true);
        }
		string issueStartPostRequest(string user)
		{
			try
			{
				string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/user?username=" + user + @"&expand=groups,applicationRoles";
				Request request = new Request(getIssue);

				rootObject = request.GetResponsersProfile();
				
				groups = rootObject.groups.items;
				username.Text = user;
				autoClose.IsVisible = false;
				buttonExit.IsVisible = false;
				return rootObject.displayName;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				return "";
			}
			return "";
		}
		void issueStartPostRequest()
        {
            try
            {
                string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/user?username=" + CrossSettings.Current.GetValueOrDefault("tmpLogin", string.Empty) + @"&expand=groups,applicationRoles";
                Request request = new Request(getIssue);

                rootObject = request.GetResponsersProfile();

                groups = rootObject.groups.items;
                username.Text = CrossSettings.Current.GetValueOrDefault("tmpLogin", string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}