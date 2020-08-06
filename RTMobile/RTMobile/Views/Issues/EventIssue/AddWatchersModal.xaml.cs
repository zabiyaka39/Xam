using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;
using System.ComponentModel;
using Rg.Plugins.Popup.Services;
using Windows.ApplicationModel.VoiceCommands;

namespace RTMobile.issues.viewIssue
{
	public partial class AddWatchersModal
	{
		public Command delWatcher { get; private set; }
		/// <summary>
		/// номер задачи
		/// </summary>
		public string issueKey { get; set; }
		public ObservableCollection<RTMobile.User> usrs { get; set; }
		public ObservableCollection<User> Additional { get; set; }
		public ObservableCollection<Watchers> watchers { get; set; }

		public AddWatchersModal(string issueKey)
		{
			InitializeComponent();
			this.issueKey = issueKey;
			Additional = new ObservableCollection<RTMobile.User>();
			DoWatch("GET", null);
			delWatcher = new Command(param =>
			{
				DelFromWatchers(param);
			});
			this.BindingContext = this;
		}

		void OneTextChenged(object sender, EventArgs e)
		{
			string word = searchbar.Text;
			if (word.Length > 0)
			{
				searchResult.IsVisible = true;
				JSONRequest jsonrequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/user/picker?query=" + word.ToLower(),
					methodRequest = "GET"
				};
				Request requestIssue = new Request(jsonrequest);
				RootObject rootObject = new RootObject();
				usrs = requestIssue.GetResponses<RootObject>().users;

				OnPropertyChanged(nameof(usrs));
			}
			else
			{
				searchResult.IsVisible = false;
			}
		}

		public void DoWatch(string method, string nameadded)
		{
			if (method == "GET")
			{
				JSONRequest jsonrequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{this.issueKey}/watchers/",
					methodRequest = method
				};
				Request requestIssue = new Request(jsonrequest);
				RootObject rootObject = new RootObject();
				watchers = requestIssue.GetResponses<RootObject>().watchers;
				if (Additional.Count == 0)
					for (int i = 0; i < watchers.Count; ++i)
					{
						Additional.Add(new RTMobile.User { name = watchers[i].name, displayName = watchers[i].displayName });
					}
			}
			if (method == "POST")
			{
				JSONRequest jsonrequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{this.issueKey}/watchers",
					methodRequest = method

				};
				Request requestIssue = new Request(jsonrequest);
				requestIssue.GetResponses<RootObject>(String.Format("\"{0}\"", nameadded));
			}
		}

		public void DelFromWatchers(object nameadded)
		{
			JSONRequest jsonrequest = new JSONRequest
			{
				urlRequest = $"/rest/api/2/issue/{this.issueKey}/watchers?username={(((RTMobile.User)nameadded).name)}",
				methodRequest = "DELETE"

			};
			Request requestIssue = new Request(jsonrequest);
			requestIssue.GetResponses<RootObject>();
			if (Additional.Count != 0)
			{
				Additional.Remove((RTMobile.User)nameadded);
				watchers.Remove(new Watchers { name = ((RTMobile.User)nameadded).name, displayName = ((RTMobile.User)nameadded).displayName });
				OnPropertyChanged(nameof(Additional));
				OnPropertyChanged(nameof(watchers));
			}
		}

		public void OneItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
			{
				((ListView)sender).SelectedItem = null;
				var us = e.SelectedItem as RTMobile.User;
				if (Additional.Count != 0)
				{
					for (int j = 0; j < Additional.Count; ++j)
					{

						if (Additional[j].displayName != us.displayName)
						{
							if (j == Additional.Count - 1)
							{
								Additional.Add(us);

								OnPropertyChanged(nameof(Additional));
								break;
							}
						}
						else
						{
							DisplayAlert("Ошибка", "Уже есть в наблюдателях!", "OK").ConfigureAwait(true);
							break;
						}
					}
				}
				else
				{
					Additional.Add(us);
					OnPropertyChanged(nameof(Additional));
				};
			} 
		}

		public void SecondSelectedItem(object sender, SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null)
			{
				((ListView)sender).SelectedItem = null;
			}
		}
		void Cho(object sender, EventArgs e)
		{
			for (int j = 0; j < Additional.Count; ++j)
				if (watchers.Count == 0)
                {
					DoWatch("POST", Additional[j].name);
				}
                else
                {
					for (int i = 0; i < watchers.Count; ++i)
						if (watchers[i].name == Additional[j].name)
						{
							break;
						}
						else
						{
							if (i == watchers.Count - 1)
							{
								DoWatch("POST", Additional[j].name);
								break;
							}
						}
				}
				MessagingCenter.Send<AddWatchersModal>(this,"WatchersChange");
				PopupNavigation.Instance.PopAsync(true);
		}
		protected override bool OnBackgroundClicked()
		{
			return false;
		}

	}
}
