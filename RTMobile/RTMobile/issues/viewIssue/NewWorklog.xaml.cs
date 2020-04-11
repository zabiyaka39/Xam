using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class NewWorkjornal : ContentPage
    {
        public string issueSummary { get; set; }
        public string issueKey { get; set; }


        public NewWorkjornal(string issueKey, string issueSummary)
        {
            this.issueKey = issueKey;
            this.issueSummary = issueSummary;
            InitializeComponent();
         
            DatePick1.MaximumDate = DateTime.Today;
            DatePick2.MaximumDate = DateTime.Today;            
        }
        private async void Create_new_worklog_Clicked(object sender, EventArgs e)
        {
            string time1 = String.Format("{0} {1}", DatePick1.Date.ToString("yyyy-MM-dd"), TimePick1.Time);
            string time2 = String.Format("{0} {1}", DatePick2.Date.ToString("yyyy-MM-dd"), TimePick2.Time);
            TimeSpan Totaltime = Convert.ToDateTime(time2) - Convert.ToDateTime(time1);

            if (newComment.Text != null)
            {
                try
                {
                    JSONRequest jsonRequest = new JSONRequest
                    {
                        urlRequest = $"/rest/api/2/issue/{this.issueKey}/worklog",
                        methodRequest = "POST",
                        comment = newComment.Text,
                        started = String.Format("{0}T{1}.00+0000", DatePick1.Date.ToString("yyyy-MM-dd"), TimePick1.Time.ToString()),
                        timeSpentSeconds = Convert.ToString(Totaltime.TotalSeconds),

                    };
                    RootObject rootObject = new RootObject();
                    Request request = new Request(jsonRequest);
                    rootObject = request.GetResponses<RootObject>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
                    Crashes.TrackError(ex);
                }

                MessagingCenter.Send<NewWorkjornal>(this, "RefreshMainPage");
                Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Ошибка", "Заполните поле комментарий, комментарий не может быть пустым!", "OK").ConfigureAwait(true);
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