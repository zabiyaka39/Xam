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


    public partial class Commentaries : ContentPage
    {
        public List<Comment> comments { get; set; }

        Issue issue = new Issue();
        public Commentaries()
        {
            InitializeComponent();
        }
        public Commentaries(Issue issues)
        {
            InitializeComponent();

            issue = issues;

            issueStartPostRequest();
            this.BindingContext = this;
        }

        /// <summary>
        /// Выгрузка всех задач
        /// </summary>
        async void issueStartPostRequest()
        {
            try
            {
                CommentJSONSearch commentJSONSearch = new CommentJSONSearch
                {
                    maxResults = 50,
                    startAt = 0
                };
                
                RootObject rootObject = new RootObject();
                Request request = new Request(commentJSONSearch, issue.key);

                rootObject = request.GetResponses("");
                Console.WriteLine(rootObject.total + " 1111111dsadadadsa");

                //Проверка на пустой список задач
                comments = rootObject.comments;

                //Console.WriteLine(rootObject.comments[0].body);

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