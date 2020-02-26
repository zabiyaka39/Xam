﻿using System;
using System.Collections.Generic;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class WorkJournal : ContentPage
    {
        public string issueKeySummary { get; set; }
        public string issueSummary { get; set; }
        public string issueKey { get; set; }
        private List<RTMobile.Transition> transition { get; set; }
        public WorkJournal()
        {
            InitializeComponent();
        }
        public WorkJournal(string issueKey, string issueSummary)
        {
            this.issueSummary = issueSummary;
            this.issueKey = issueKey;
            InitializeComponent();
            transitionIssue(issueKey);
            this.BindingContext = this;
        }
        void ImageButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Calendar());
        }
        private void transitionIssue(string issueKey)
        {
            string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/issue/" + issueKey + "/transitions/";

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
            Navigation.PushAsync(new History(issueKey, issueSummary));
        }
        
        void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment(issueKey, issueSummary));
        }


        void showHistory_Clicked(System.Object sender, System.EventArgs e)
        {

        }

        void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}