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
    public partial class Comment : ContentPage
    {
        public ObservableCollection<RTMobile.Comment> comments { get; set; }
        private List<RTMobile.Transition> transition { get; set; }//Переходы по заявке

        public string issueKeySummary { get; set; }
        public string issueSummary { get; set; }
        public string issueKey { get; set; }
        public Comment()
        {
            InitializeComponent();
            transitionIssue(issueKey);
        }      
        public Comment(string issueKey, string issueSummary)
        {
            this.issueKey = issueKey;
            this.issueSummary = issueSummary;
            this.issueKeySummary = issueKey + " - " + issueSummary;
            InitializeComponent();            
            issueStartPostRequest(issueKey);
            transitionIssue(issueKey);

            if (this.comments.Count > 0)
            {
                listComment.IsVisible = true;
                noneComment.IsVisible = false;
            }
            else
            {
                listComment.IsVisible = false;
                noneComment.IsVisible = true;
            }
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
                ToolbarItems.Add(tb);
            }
        }
        /// <summary>
        /// Выгрузка всех задач
        /// </summary>
        async void issueStartPostRequest(string issueKey, bool firstRequest = true)
        {
            try
            {
                CommentJSONSearch commentJSONSearch = new CommentJSONSearch
                {
                    maxResults = 50,
                    startAt = 0
                };

                RootObject rootObject = new RootObject();
                Request request = new Request(commentJSONSearch, issueKey);

                rootObject = request.GetResponses("");
                //проверка на наличие комментариев. При отсутствии комментариев добавляем все, при наличии добавляем только последний

                if (!firstRequest && rootObject.comments.Count > 0)
                {
                    comments.Add(rootObject.comments[rootObject.comments.Count - 1]);
                }
                else
                {
                    comments = rootObject.comments;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK");
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
            Navigation.PushAsync(new History(issueKey,issueSummary));
        }

        void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new WorkJournal(issueKey,issueSummary));
        }

        void showHistory_Clicked(System.Object sender, System.EventArgs e)
        {

        }

        void ListView_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private async void ImageButton_Clicked_4(object sender, EventArgs e)
        {
            try
            {
                RTMobile.Comment comment = new RTMobile.Comment
                {
                    body = newComment.Text
                };
                RootObject rootObject = new RootObject();
                Request request = new Request(comment, issueKey);

                rootObject = request.GetResponses();

                //Проверка на пустой список задач


                if (rootObject.id != 0)
                {
                    newComment.Text = "";
                    issueStartPostRequest(issueKey,false);
                    await DisplayAlert("Готово", "Комментарий добавлен", "OK").ConfigureAwait(true);
                }
                else
                {
                    await DisplayAlert("Ошибка", "Ошибка добавления комментария в систему", "OK").ConfigureAwait(true); 
                }

                if (this.comments.Count > 0)
                {
                    listComment.IsVisible = true;
                    noneComment.IsVisible = false;
                }
                else
                {
                    listComment.IsVisible = false;
                    noneComment.IsVisible = true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK");
            }
        }
    }
}
