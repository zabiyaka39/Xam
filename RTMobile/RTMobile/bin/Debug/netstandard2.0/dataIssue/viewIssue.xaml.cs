using Plugin.Settings;
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
		public List<Fields> fieldIssue { get; set; }//поля заявки
		public List<Transition> transition { get; set; } //Переходы по заявке
		private RootObject watchers = new RootObject();
		public Issue issue { get; set; }
		public viewIssue()
		{
			InitializeComponent();
			//((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Wheat;
			//((NavigationPage)Application.Current.MainPage).BarTextColor = Color.Black;
			//((NavigationPage)Application.Current.MainPage).Title = "Задача";
			this.BindingContext = this;
		}

		public viewIssue(Issue issues)
		{
			InitializeComponent();
			//((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.Wheat;
			//((NavigationPage)Application.Current.MainPage).BarTextColor = Color.Black;
			//((NavigationPage)Application.Current.MainPage).Title = "Задача";          

			issue = issues;
			Request request = new Request($"https://sd.rosohrana.ru/rest/api/2/issue/{issue.key}?expand=names,schema");
			fieldIssue = request.GetCustomField();

			listDetailIssue.HeightRequest = fieldIssue.Count * 37;

			warchersIssue();

			this.BindingContext = this;
		}

		private async void warchersIssue()
		{
			try
			{
				string getIssue = CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + @"/rest/api/2/issue/" + issue.key + "/watchers/";

				Request request = new Request(getIssue);
				watchers = request.GetResponses(getIssue);
				countWatchers.Text = watchers.watchCount.ToString();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await DisplayAlert("Error issues", ex.ToString(), "OK");
			}
			try
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
					ToolbarItems.Add(tb);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				await DisplayAlert("Error issues", ex.ToString(), "OK");
			}
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
		private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			List<string> watchersList = new List<string>();
			for (int i = 0; i < watchers.watchers.Count; ++i)
			{
				watchersList.Add(watchers.watchers[i].displayName);
			}
			string action = await DisplayActionSheet("Наблюдатели задачи", "Закрыть", null, watchersList.ToArray());
			Console.WriteLine("Action: " + action);
		}

		private void ImageButton_Clicked(object sender, EventArgs e)
		{
		}

		private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{

		}

		private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new profile(issue.fields.assignee.name)).ConfigureAwait(true);
		}

		private async void TapGestureRecognizer_Tapped_3(object sender, EventArgs e)
		{
			await Navigation.PushAsync(new profile(issue.fields.reporter.name)).ConfigureAwait(true);
		}
	}
}