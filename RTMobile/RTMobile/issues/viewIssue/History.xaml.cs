using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{   
    public partial class History : ContentPage
    {
        public string issueKeySummary { get; set; }
        public string issueSummary { get; set; }
        public string issueKey { get; set; }
        private List<RTMobile.Transition> transition { get; set; }
        public ObservableCollection<RTMobile.History> histories { get; set; }
        public History(string issueKey, string issueSummary)
        {
            issueKeySummary = issueKey + " - " + issueSummary;
            this.issueKey = issueKey;
            this.issueSummary = issueSummary;
            transitionIssue(issueKey);

            InitializeComponent();

            historyIssue(issueKey);

            this.BindingContext = this;
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
                tb.Clicked += async (sender, args) =>
                {
                    await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issueKey)).ConfigureAwait(true);
                };
                ToolbarItems.Add(tb);
            }
        }
        private async void historyIssue(string issueKey, bool firstRequest = true)
        {
            try
            {
                string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/issue/" + issueKey + "?expand=changelog";

                Request request = new Request(getIssue);
                RootObject historyIssues = new RootObject();
                historyIssues = request.GetResponses(getIssue);
                //Проверяем наличие истории. Если первая то присваиваем, если обновляем, то добавляем последний элемент
                if (!firstRequest && historyIssues.changelog.histories.Count > 0)
                {
                    histories.Add(historyIssues.changelog.histories[historyIssues.changelog.histories.Count - 1]);
                }
                else
                {
                    histories = historyIssues.changelog.histories;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK").ConfigureAwait(true);
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


        void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new WorkJournal());
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
