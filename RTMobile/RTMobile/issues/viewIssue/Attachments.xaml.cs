using System;
using System.Collections.Generic;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class Attachments : ContentPage
    {
        public Issue issue { get; set; }
        private List<RTMobile.Transition> transition { get; set; }//Переходы по заявке
        public Attachments()
        {
            InitializeComponent();
            //transitionIssue();
        }
        private void transitionIssue()
        {
            string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/issue/" + issue.key + "/transitions/";

            Request request = new Request(getIssue);
            transition = request.GetResponses(getIssue).transitions;
            for (int i = 0; i < transition.Count; ++i)
            {
                ToolbarItem tb = new ToolbarItem
                {
                    Text = transition[i].name,
                    Order = ToolbarItemOrder.Secondary,
                    Priority = i + 1
                };
                ToolbarItems.Add(tb);
            }
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
            Navigation.PushAsync(new History(issue.key, issue.fields.summary));
        }

        void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new WorkJournal(issue.key, issue.fields.summary));
        }
        void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment(issue.key, issue.fields.summary));
        }

        void ToolbarItem_Clicked_3(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment(issue.key, issue.fields.summary));
        }
    }
}
