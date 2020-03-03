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
    public partial class People : ContentPage
    {
        public Issue issue { get; set; }
        public RootObject watchers { get; set; }
        private List<RTMobile.Transition> transition { get; set; }
        public People(Issue issue)
        {
            InitializeComponent();
            this.issue = issue;
            warchersIssue();
            transitionIssue();

            this.BindingContext = this;
        }
        private async void warchersIssue()
        {
            try
            {
                string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/issue/" + issue.key + "/watchers/";

                Request request = new Request(getIssue);
                watchers = request.GetResponses(getIssue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
            }           
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
                tb.Clicked += async (sender, args) =>
                {
                    await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issue.key)).ConfigureAwait(true);
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

         
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Profile(issue.fields.creator.name)).ConfigureAwait(true);
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Profile(issue.fields.assignee.name)).ConfigureAwait(true);
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Watcher selectedIssue = e.Item as Watcher;
            if (selectedIssue != null)
            {
                await Navigation.PushAsync(new Profile(selectedIssue.name)).ConfigureAwait(true);
            }
            ((ListView)sender).SelectedItem = null;
        }
    }
}
