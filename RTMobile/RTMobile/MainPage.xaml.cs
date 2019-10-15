using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace RTMobile
{
    [DesignTimeVisible(false)]
    public partial class mainPage : ContentPage
    {
        public mainPage()
        {
            InitializeComponent();

            //ToolbarItem toolbar = new ToolbarItem
            //{
            //    Text = "Настройки",
            //    Order = ToolbarItemOrder.Primary,
            //    Priority = 0,
            //    Icon = new FileImageSource
            //    {
            //        File = "settings.png"
            //    }

            //};
            //ToolbarItems.Add(toolbar);



        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new authorization());
            //await Navigation.PushAsync(new IssuePage());
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new about());
        }
    }
}
