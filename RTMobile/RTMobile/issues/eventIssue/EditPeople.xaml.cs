using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.issues.eventIssue
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditPeople
	{
		public ObservableCollection<User> Users { get; set; }
		/// <summary>
		/// Указывает на тип запроса (если меняем истолнителя, то true)
		/// </summary>
		private bool assignee { get; set; }
		private string issueKey { get; set; }
		public EditPeople(string issueKey, bool assignee = true)
		{
			InitializeComponent();
			this.assignee = assignee;
			this.issueKey = issueKey;
			JSONRequest jsonrequest = new JSONRequest
			{
				urlRequest = $"/rest/api/2/user/picker?query=\"\"",
				methodRequest = "GET"
			};
			RootObject rootObject = new RootObject();
			Request requestIssue = new Request(jsonrequest);
			rootObject = requestIssue.GetResponses<RootObject>();
			if (rootObject != null && rootObject.users != null)
			{
				Users = rootObject.users;
				OnPropertyChanged(nameof(Users));
			}
			this.BindingContext = this;
		}
		public void OneItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
			{
				((ListView)sender).SelectedItem = null;
				User user = e.SelectedItem as RTMobile.User;
			}
		}

		private void SearchUserBar_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (SearchUserBar.Text.Length > 0)
				{
					JSONRequest jsonrequest = new JSONRequest
					{
						urlRequest = $"/rest/api/2/user/picker?query=" + SearchUserBar.Text.ToLower(),
						methodRequest = "GET"
					};
					RootObject rootObject = new RootObject();
					Request requestIssue = new Request(jsonrequest);
					rootObject = requestIssue.GetResponses<RootObject>();
					if (rootObject != null && rootObject.users != null)
					{
						Users = rootObject.users;
						OnPropertyChanged(nameof(Users));
					}
				}
				else
				{
					JSONRequest jsonrequest = new JSONRequest
					{
						urlRequest = $"/rest/api/2/user/picker?query=\"\"",
						methodRequest = "GET"
					};
					Request requestIssue = new Request(jsonrequest);
					RootObject rootObject = new RootObject();
					rootObject = requestIssue.GetResponses<RootObject>();
					if (rootObject != null && rootObject.users != null)
					{
						Users = rootObject.users;
						OnPropertyChanged(nameof(Users));
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private async void Button_Clicked(object sender, EventArgs e)
		{
			await Navigation.PopAsync().ConfigureAwait(true);
		}

		private void Button_Clicked_1(object sender, EventArgs e)
		{
			if (assignee)
			{
				JSONRequest jsonrequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{issueKey}/assignee",
					methodRequest = "PUT"
				};
				User user = usersList.SelectedItem as RTMobile.User;
				string json = "{\"name\":\"" + user.name + "\"}";
				Request requestIssue = new Request(jsonrequest);

				requestIssue.GetResponses<RootObject>(json);
				PopupNavigation.Instance.PopAsync(true);
			}
		}
	}
}