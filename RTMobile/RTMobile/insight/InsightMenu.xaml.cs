﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Crashes;

namespace RTMobile.insight
{

    public partial class InsightMenu : ContentPage
    {
        public List<JiraIssue> listissue { get; set; }
        public ObservableCollection<Issue> listConnectedIssue { get; set; }
        ObjectEntry selectedField { get; set; }
        public InsightMenu(ObjectEntry selectedField)
        {
            InitializeComponent();
            takejiraIssueList(selectedField);
            Title = selectedField.name;
            this.BindingContext = this;

        }

        void takejiraIssueList(ObjectEntry selectedField)
        {
           
                JSONRequest jsonRequest = new JSONRequest()
                {
                    urlRequest = $"/rest/insight/1.0/object/{selectedField.id}/jiraissues",
                    methodRequest = "GET"
                };
                Request request = new Request(jsonRequest);
                listissue = request.GetResponses<RootObject>().jiraIssues;
                takejiraIssue();
        }

        void takejiraIssue()
        {
            if(listissue !=null)
            {
                try
                {
                    listConnectedIssue = new ObservableCollection<Issue>();
                    for (int i = 0; i < listissue.Count; ++i)
                    {
                        JSONRequest jsonRequest = new JSONRequest()
                        {
                            urlRequest = $"/rest/api/2/issue/{listissue[i].jiraIssueKey}",
                            methodRequest = "GET"
                        };
                        Request request = new Request(jsonRequest);
                        listConnectedIssue.Add(request.GetResponses<Issue>());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Crashes.TrackError(ex);
                }

            }
  
        }

        async private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
           
             Issue selectedIssue = e.Item as Issue;
             if (selectedIssue != null)
             {
                await Navigation.PushAsync(new RTMobile.issues.viewIssue.TabPageIssue(selectedIssue)).ConfigureAwait(true);
             }
            ((ListView)sender).SelectedItem = null;
        }
    }

    
}