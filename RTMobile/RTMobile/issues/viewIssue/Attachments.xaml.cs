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

namespace RTMobile.issues.viewIssue
{
	public partial class Attachments : ContentPage
	{
		Issue issue = new Issue();
		public ObservableCollection<Attachment> attachmentsImage = new ObservableCollection<Attachment>();
		public ObservableCollection<Attachment> attachmentsDocument = new ObservableCollection<Attachment>();
		public ObservableCollection<Attachment> attachmentsOther = new ObservableCollection<Attachment>();
		private List<RTMobile.Transition> transition { get; set; }//Переходы по заявке
		public Attachments(Issue issue)
		{
			List<Attachment> atach = new List<Attachment>();
			InitializeComponent();
			//TransitionIssue();
			this.issue = issue;
			if (issue != null && issue.fields != null)
			{
				for (int i = 0; i < issue.fields.attachment.Count; ++i)
				{
					try
					{
						switch (issue.fields.attachment[i].mimeType)
						{
							case "image":
								{
									attachmentsImage.Add(issue.fields.attachment[i]);
									break;
								}
							case "text":
								{
									attachmentsDocument.Add(issue.fields.attachment[i]);
									break;
								}
							default:
								{
									
									atach.Add(issue.fields.attachment[i]);
									attachmentsOther.Add(issue.fields.attachment[i]);
									break;
								}
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);

					}
				}
			}

			//if (attachmentsImage != null && attachmentsImage.Count > 0)
			//{
			//	noneImage.IsVisible = true;
			//	carouselImages.IsVisible = false;
			//}
			//else
			//{
			//	carouselImages.IsVisible = true;
			//	noneImage.IsVisible = false;
			//}

			carouselImages.ItemsSource = attachmentsImage;
			carouselDocuments.ItemsSource = attachmentsDocument;
			carouselOthers.ItemsSource = attachmentsOther;

			this.BindingContext = this;
		}
		private void TransitionIssue()
		{
			try
			{
				JSONRequest jsonRequest = new JSONRequest
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/transitions/",
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
						await Navigation.PushAsync(new RTMobile.issues.viewIssue.Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issue.key)).ConfigureAwait(true);
					};
					ToolbarItems.Add(tb);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
		}
		void ImageButton_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Calendar());
		}
		void ImageButton_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Insight());
		}
		void ImageButton_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Filter());
		}
		void ImageButton_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Navigation.PopToRootAsync();
		}
		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new History(issue.key, issue.summary));
		}
		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new WorkJournal(issue.key, issue.summary));
		}
		void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issue.key, issue.fields.summary));
		}
		void ToolbarItem_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issue.key, issue.fields.summary));
		}

		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			Navigation.PushAsync(new imageView(attachmentsImage));
		}
	}
}
