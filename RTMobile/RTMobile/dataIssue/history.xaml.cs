using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class history : ContentPage
    {
        private Issue issue = new Issue();
        public ObservableCollection<History> histories { get; set; }

        public history()
        {
            InitializeComponent();
        }
        public history(Issue issues)
        {
            InitializeComponent();
            issue = issues;

            historyIssue();
            this.BindingContext = this;
        }

        private async void historyIssue(bool firstRequest = true)
        {
            try
            {
                string getIssue = CrossSettings.Current.GetValueOrDefault<string>("urlServer") + @"/rest/api/2/issue/" + issue.key + "?expand=changelog";

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