using System;
using System.Collections.Generic;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;

using Xamarin.Forms;

namespace RTMobile.calendar
{
    public partial class Calendar : ContentPage
    {
        public Calendar()
        {
            InitializeComponent();
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
