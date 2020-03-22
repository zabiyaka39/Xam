using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class Attachments : ContentPage
	{
		Issue issue = new Issue();
		private ObservableCollection<Attachment> attachmentsImage = new ObservableCollection<Attachment>();
		private ObservableCollection<Attachment> attachmentsDocument = new ObservableCollection<Attachment>();
		private ObservableCollection<Attachment> attachmentsOther = new ObservableCollection<Attachment>();
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
			//Скрываем или показываем предупреждение о отсутствии файлов или покаызваем файлы
			if (attachmentsImage != null && attachmentsImage.Count > 0)
			{
				noneImage.IsVisible = false;
				carouselImages.IsVisible = true;
				carouselImages.ItemsSource = attachmentsImage;
			}
			else
			{
				carouselImages.IsVisible = false;
				noneImage.IsVisible = true;
			}
			if (attachmentsDocument != null && attachmentsDocument.Count > 0)
			{
				noneDocuments.IsVisible = false;
				carouselDocuments.IsVisible = true;
				carouselDocuments.ItemsSource = attachmentsDocument;
			}
			else
			{
				carouselDocuments.IsVisible = false;
				noneDocuments.IsVisible = true;
			}
			if (attachmentsOther != null && attachmentsOther.Count > 0)
			{
				noneOthers.IsVisible = false;
				carouselOthers.IsVisible = true;
				carouselOthers.ItemsSource = attachmentsOther;
			}
			else
			{
				carouselOthers.IsVisible = false;
				noneOthers.IsVisible = true;
			}

			this.BindingContext = this;
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
			Navigation.PushAsync(new imageView(attachmentsImage, carouselImages.Position));
		}

		private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{
			Uri uri = new Uri (attachmentsDocument[carouselDocuments.Position].content);
			await Launcher.TryOpenAsync(uri).ConfigureAwait(true);
		}
		private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
		{
			Uri uri = new Uri(attachmentsOther[carouselOthers.Position].content);
			await Launcher.TryOpenAsync(uri).ConfigureAwait(true); 
		}
	}
}
