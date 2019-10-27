using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class workJournal : ContentPage
    {
        private Issue issue = new Issue();
        public List<Worklog> worklogs { get; set; }
        public workJournal()
        {
            InitializeComponent();
        }
        public workJournal(Issue issues)
        {
            InitializeComponent();
            issue = issues;

            workJournalIssue();
            this.BindingContext = this;
        }

        private async void workJournalIssue()
        {
            try
            {
                string getIssue = @"https://sd.rosohrana.ru/rest/api/2/issue/" +issue.key + "/worklog/";

                Request request = new Request(getIssue);
                RootObject workJournals = new RootObject();
                workJournals = request.GetResponses(getIssue);
                worklogs = workJournals.worklogs;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK");
            }
        }

        public async void OnItemTapped(object sender, ItemTappedEventArgs e)//обработка нажатия на элемент в ListView
        {
            //Issue selectedIssue = e.Item as Issue;
            //if (selectedIssue != null)
            //{
            //    await Navigation.PushAsync(new general(selectedIssue));
            //    //await DisplayAlert("Выбранная модель", $"{selectedIssue.key}", "OK");
            //}
            ((ListView)sender).SelectedItem = null;
        }
    }
}