﻿using Plugin.Settings;
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
			//CrossSettings.Current.AddOrUpdateValue("saveAuthorizationData","true");
			//await Navigation.PushAsync(new authorization()).ConfigureAwait(true);
			if (CrossSettings.Current.GetValueOrDefault("login", string.Empty).Length > 0 && 
				CrossSettings.Current.GetValueOrDefault("password", string.Empty).Length > 0)/* && 
				CrossSettings.Current.GetValueOrDefault("saveAuthorizationData", string.Empty) == "true")*/
			{
				Request request = new Request();

				if (request.authorization(CrossSettings.Current.GetValueOrDefault("login", string.Empty), CrossSettings.Current.GetValueOrDefault("password", string.Empty)))
				{
					CrossSettings.Current.AddOrUpdateValue("tmpLogin", CrossSettings.Current.GetValueOrDefault("login", string.Empty));
					CrossSettings.Current.AddOrUpdateValue("tmpPassword", CrossSettings.Current.GetValueOrDefault("password", string.Empty));

					//var mainPage = new IssuePage();//this could be content page
					//var rootPage = new NavigationPage(mainPage);
					//NavigationPage(new IssuePage());мани
					await Navigation.PushAsync(new IssuePage()).ConfigureAwait(true);
				}
				else
				{
					await Navigation.PushAsync(new authorization()).ConfigureAwait(true);
				}
			}
			else
			{
				if (CrossSettings.Current.GetValueOrDefault("login", string.Empty).Length > 0)
				{
					CrossSettings.Current.Remove("login");
				}
				if (CrossSettings.Current.GetValueOrDefault("password", string.Empty).Length > 0)
				{
					CrossSettings.Current.Remove("password");
				}

				await Navigation.PushAsync(new authorization()).ConfigureAwait(true);
			}
			//await Navigation.PushAsync(new IssuePage());
		}

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new about()).ConfigureAwait(true);
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new settingsRT()).ConfigureAwait(true);
        }
    }
}
