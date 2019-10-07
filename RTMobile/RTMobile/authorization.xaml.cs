
using Serenity.Navigation;
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
    public partial class authorization : ContentPage
    {
        public authorization()
        {
            InitializeComponent();
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Black;
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.White;
            //this.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
            //Application.Current.MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var mainPage = new IssuePage();//this could be content page
            var rootPage = new NavigationPage(mainPage);
            //NavigationPage(new IssuePage());
            await Navigation.PushAsync(new IssuePage());
        }
    }
}