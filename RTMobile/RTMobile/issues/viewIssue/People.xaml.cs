using System;
using System.Collections.Generic;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class People : ContentPage
    {
        public People()
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

        void showWatcherPeople_Clicked(System.Object sender, System.EventArgs e)
        {
            if (watcherPeople.IsVisible)
            {
                showWatcherPeople.Source = "arrowDown.png";
                watcherFrame.HeightRequest = 70;
                watcherPeople.IsVisible = false;
            }
            else
            {
                showWatcherPeople.Source = "arrowUp.png";
                watcherFrame.HeightRequest = 250;
                watcherPeople.IsVisible = true;
            }
        }

        void showGeneralPeople_Clicked(System.Object sender, System.EventArgs e)
        {
            if (generalPeople.IsVisible)
            {
                showGeneralPeople.Source = "arrowDown.png";
                generalFrame.HeightRequest = 70;
                generalPeople.IsVisible = false;
            }
            else
            {
                showGeneralPeople.Source = "arrowUp.png";
                generalFrame.HeightRequest = 210;
                generalPeople.IsVisible = true;
            }
        }
        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new History());
        }

        void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new WorkJournal());
        }

        void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment());
        }

        void ToolbarItem_Clicked_3(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment());
        }
    }
}
