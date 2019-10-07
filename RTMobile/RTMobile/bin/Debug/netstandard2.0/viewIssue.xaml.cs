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
    public partial class viewIssue : ContentPage
    {
        public viewIssue()
        {
            InitializeComponent();
            //((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Wheat;
            //((NavigationPage)Application.Current.MainPage).BarTextColor = Color.Black;
            //((NavigationPage)Application.Current.MainPage).Title = "Задача";
            var names = new List<string>
            {
                "Test1", "Test2", "Test3"
            };
            //MainCarouselView.ItemsSource = names;

        }

        private void ButtonDetailIssue_Clicked(object sender, EventArgs e)
        {
            if (detailIssueData.IsVisible == false)
            {
                detailIssueData.IsVisible = true;
                buttonDetailIssue.Source = "arrowUp.png";

            }
            else
            {
                detailIssueData.IsVisible = false;
                buttonDetailIssue.Source = "arrowDown.png";
            }
        }

        private void ButtonDescriptionIssue_Clicked(object sender, EventArgs e)
        {
            if (descriptionIssue.IsVisible == false)
            {
                descriptionIssue.IsVisible = true;
                buttonDescriptionIssue.Source = "arrowUp.png";

            }
            else
            {
                descriptionIssue.IsVisible = false;
                buttonDescriptionIssue.Source = "arrowDown.png";
            }
        }

        private void ButtonFileIssue_Clicked(object sender, EventArgs e)
        {

            if (fileIssue.IsVisible == false)
            {
                fileIssue.IsVisible = true;
                buttonFileIssue.Source = "arrowUp.png";

            }
            else
            {
                fileIssue.IsVisible = false;
                buttonFileIssue.Source = "arrowDown.png";
            }
        }

        private async void imageTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new photoView());
        }
        private void ButtonPeopleIssue_Clicked(object sender, EventArgs e)
        {
            if (peopleIssue.IsVisible == false)
            {
                peopleIssue.IsVisible = true;
                buttonPeopleIssue.Source = "arrowUp.png";

            }
            else
            {
                peopleIssue.IsVisible = false;
                buttonPeopleIssue.Source = "arrowDown.png";
            }
        }
    }
}