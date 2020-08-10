using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class TabPageIssue : TabbedPage
	{
		private List<RTMobile.Transition> transition { get; set; }//Переходы по заявке
		private Issue issue = new Issue();
		public TabPageIssue(Issue issues)
		{
			InitializeComponent();

			issue = issues;

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
							await Navigation.PushAsync(new Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issues.key, issue.id)).ConfigureAwait(true);
						};
						ToolbarItems.Add(tb);
					}
					//Добавляем поле с комментарием
					ToolbarItem tbComment = new ToolbarItem
					{
						Text = "Комментарии",
						Order = ToolbarItemOrder.Primary,
						IconImageSource = "commentToolBar.png",
						Priority = 0
					};
					tbComment.Clicked += ToolbarItem_Clicked_2;
					ToolbarItems.Add(tbComment);

					ToolbarItem tbSeporator = new ToolbarItem
					{
						Text = "   _________________   ",
						Order = ToolbarItemOrder.Secondary,
						Priority = 997
					};
					ToolbarItems.Add(tbSeporator);

					ToolbarItem tbCommentSec = new ToolbarItem
					{
						Text = "Комментарии",
						Order = ToolbarItemOrder.Secondary,
						Priority = 998
					};
					tbCommentSec.Clicked += ToolbarItem_Clicked_2;
					ToolbarItems.Add(tbCommentSec);

					ToolbarItem tbCommentWorklog = new ToolbarItem
					{
						Text = "Рабочий журнал",
						Order = ToolbarItemOrder.Secondary,
						Priority = 999
					};
					tbCommentWorklog.Clicked += ToolbarItem_Clicked_1;
					ToolbarItems.Add(tbCommentWorklog);


					ToolbarItem tbHistory = new ToolbarItem
					{
						Text = "История",
						Order = ToolbarItemOrder.Secondary,
						Priority = 1000
					};
					tbHistory.Clicked += ToolbarItem_Clicked;
					ToolbarItems.Add(tbHistory);

					ToolbarItem tbShared = new ToolbarItem
					{
						Text = "Поделиться",
						IconImageSource = "shered.png",
						Order = ToolbarItemOrder.Primary,
						Priority = 1001
					};
					tbShared.Clicked += SendIssueClicked;
					ToolbarItems.Add(tbShared);


					ToolbarItem tbSeporator1 = new ToolbarItem
					{
						Text = "   _________________   ",
						Order = ToolbarItemOrder.Secondary,
						Priority = 1001
					};
					ToolbarItems.Add(tbSeporator1);

					ToolbarItem tbSharedMore = new ToolbarItem
					{
						Text = "Поделиться",
						IconImageSource = "shered.png",
						Order = ToolbarItemOrder.Secondary,
						Priority = 1002
					};
					tbSharedMore.Clicked += SendIssueClicked;
					ToolbarItems.Add(tbSharedMore);

				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}

		//кнопка вызывает диалоговое окно, в котором можно выбрать способ отпраки ссылки на задачу
		async void SendIssueClicked(System.Object sender, System.EventArgs e)
		{
			await ShereIssue();
		}
		//метод вызова диалогового окна, в котором можно выбрать способ отпраки ссылки на задачу
		public async Task ShereIssue()
		{
			await Share.RequestAsync(new ShareTextRequest
			{
				Uri = String.Format("https://sd.rosohrana.ru/browse/{0}", issue.key),
				Text = String.Format("С вами поделились задачей:\n{0} - {1}", issue.key, issue.fields.summary),
				Title = String.Format("Вы хотите поделиться задачей: {0} - {1}", issue.key, issue.fields.summary)
			});
		}
		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new History(issue.key, issue.fields.summary, issue.id));
		}
		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new WorkJournal(issue.key, issue.fields.summary, issue.id));
		}

		void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issue.key, issue.fields.summary, issue.id));
		}

	}
}
