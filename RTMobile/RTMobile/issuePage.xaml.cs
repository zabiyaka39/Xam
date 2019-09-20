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
    public partial class IssuePage : ContentPage
    {
        public IssuePage()
        {
            InitializeComponent();          


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
    }
}