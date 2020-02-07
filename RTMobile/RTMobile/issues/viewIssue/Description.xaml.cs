using System;
using System.Collections.Generic;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class Description : ContentPage
    {
        public Description()
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

        void showDescriptionIssue_Clicked(System.Object sender, System.EventArgs e)
        {
            if (description.IsVisible)
            {
                showDescription.Source = "arrowDown.png";
                //descriptionFrame.HeightRequest = 70;
                description.IsVisible = false;
                descriptionFrame.VerticalOptions = LayoutOptions.Start;
            }
            else
            {
                showDescription.Source = "arrowUp.png";
                //descriptionFrame.HeightRequest = 200;
                descriptionFrame.VerticalOptions = LayoutOptions.FillAndExpand;
                description.IsVisible = true;
            }
        }
        
    }
}
