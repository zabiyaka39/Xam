using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class TabPageIssue : TabbedPage
	{
		private List<RTMobile.Transition> transition { get; set; }//Переходы по заявке
		public TabPageIssue(Issue issues)
		{
			InitializeComponent();
			if (issues != null)
			{
				Title = issues.key;

				//Сделать в этом файле обработчик задачи по всем полям и передавать уже заполненную переменную issues

				JSONRequest jsonRequestUser = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{issues.key}?fields=*all",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequestUser);
				Issue issue = request.GetResponses<Issue>();

				Children.Add(new General(issue) { Title = "Основное" });
				Children.Add(new Description(issue) { Title = "Описание" });
				Children.Add(new Attachments(issue) { Title = "Вложения" });
				Children.Add(new People(issue) { Title = "Люди" });
			}

			try
			{
				if (issues != null)
				{
					JSONRequest jsonRequest = new JSONRequest
					{
						urlRequest = $"/rest/api/2/issue/{issues.key}/transitions/",
						methodRequest = "GET"
					};
					Request request = new Request(jsonRequest);

					transition = request.GetResponses<RootObject>().transitions;
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
							await Navigation.PushAsync(new Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issues.key)).ConfigureAwait(true);
						};
						ToolbarItems.Add(tb);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}

	}
}
