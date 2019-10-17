using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.ComponentModel;
using System.Net.Http;

namespace RTMobile
{


    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IssuePage : ContentPage
    {
        public List<Issue> issues { get; set; }

        public IssuePage()
        {
            InitializeComponent();


            issueStartPostRequest();
            //for (int i = 0; i < issues.Count; ++i)
            //{
            //    issues[i].fields.issuetype.iconUrl = issues[i].fields.issuetype.iconUrl;
            //    Console.WriteLine(issues[i].fields.issuetype.iconUrl);
            //}
           
            this.BindingContext = this;


        }
        /// <summary>
        /// Выгрузка всех задач
        /// </summary>
        async void issueStartPostRequest()
        {
            try
            {
                IssueJSONSearch issueJSONSearch = new IssueJSONSearch
                {
                    jql = "status not in  (Закрыта, Отклонена, Отменена, Активирована, Выполнено, 'Доставлена клиенту', Провалено) AND assignee in (currentUser())",
                    maxResults = 50,
                    startAt = 0
                };

                RootObject rootObject = new RootObject();
                Request request = new Request(issueJSONSearch);

                rootObject = request.GetResponses();
                
                //Проверка на пустой список задач
                try
                {
                    if (rootObject.issues != null)
                    {
                        issues = rootObject.issues;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.ToString(), "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await DisplayAlert("Error issues", ex.ToString(), "OK");
            }
        }
        /// <summary>
        /// Тап перехода к задаче
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void OnItemTapped(object sender, ItemTappedEventArgs e)//обработка нажатия на элемент в ListView
        {
            Issue selectedIssue = e.Item as Issue;
            if (selectedIssue != null)
            {
                await Navigation.PushAsync(new general(selectedIssue));
                //await DisplayAlert("Выбранная модель", $"{selectedIssue.key}", "OK");
            }
            ((ListView)sender).SelectedItem = null;
        }

        /// <summary>
        /// Кнопка поиска задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Clicked(object sender, EventArgs e)
        {
            issueStartPostRequest();
            await Navigation.PushAsync(new searchIssue());
        }
        /// <summary>
        /// Кнопка создания задачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new createIssue());
        }
        /// <summary>
        /// Тап перехода к задаче
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //Issue selectedIssue = sender as Issue;
            //if (selectedIssue != null)
            //{
            //    await Navigation.PushAsync(new general(selectedIssue));
            //    //await DisplayAlert("Выбранная модель", $"{selectedIssue.key}", "OK");
            //}
            //((ListView)sender).SelectedItem = null;
        }
        /// <summary>
        /// Кнопка перехода к профилю
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new profile());
        }
        /// <summary>
        /// Кнопка перехода к уведомлениям
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new notifications());
        }
        /// <summary>
        /// Кнопка перехода к разделу Insight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImageButton_Clicked_2(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new insight());
        }
    }
}