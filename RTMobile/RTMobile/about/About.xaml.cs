using System;
using System.Collections.Generic;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTMobile.about
{
    public partial class About : ContentPage
    {
        public About()
        {
            InitializeComponent();
            VersionTracking.Track();
            versionApp.Text ="Версия: " + VersionTracking.CurrentVersion; 

        }
        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Calendar());
        }

        void ImageButton_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Insight());
        }

        void ImageButton_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Filter());
        }

        void ImageButton_Clicked_3(System.Object sender, System.EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
    
}
