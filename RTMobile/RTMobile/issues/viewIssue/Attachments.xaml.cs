using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.FilePicker;
using System.Threading.Tasks;
using Plugin.FilePicker.Abstractions;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Net.Http;
using System.Net.Mime;
using System.Net.Http.Headers;

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

			InitializeComponent();
			//TransitionIssue();
			this.issue = issue;
			issueFieldsRefresh(issue.fields);
			this.BindingContext = this;
		}

		public void issueFieldsRefresh(Fields fields)
		{

			if (issue != null && fields != null)
			{
				for (int i = attachmentsImage.Count; i > 0; --i)
				{
					attachmentsImage.RemoveAt(0);
				}
				for (int i = attachmentsDocument.Count; i > 0; --i)
				{
					attachmentsDocument.RemoveAt(0);
				}
				for (int i = attachmentsOther.Count; i > 0; --i)
				{
					attachmentsOther.RemoveAt(0);
				}
				for (int i = 0; i < fields.attachment.Count; ++i)
				{
					try
					{
						switch (fields.attachment[i].mimeType)
						{
							case "image":
								{
									attachmentsImage.Add(fields.attachment[i]);
									break;
								}
							case "text":
								{
									attachmentsDocument.Add(fields.attachment[i]);
									break;
								}
							default:
								{
									attachmentsOther.Add(fields.attachment[i]);
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
		}

		//Кнопка при нажатии которой открывается файловый менеджер
		private async void upload_Button(object sender, EventArgs e)
		{
			MediaFile _mediaFile = null;

			//Инициализируем проверку доступности разрешения работы с файловой системой
			await CrossMedia.Current.Initialize();
			//Выполняем поиск по системе необходимого файла
			if (CrossMedia.Current.IsPickPhotoSupported)
			{
				_mediaFile = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);
			}

			if (_mediaFile != null)
			{
				string boundary = DateTime.Now.Ticks.ToString("x");

				MultipartFormDataContent content = new MultipartFormDataContent(boundary);

				var streamContent = new StreamContent(_mediaFile.GetStream());
				//Задаем MimeType файлу
				streamContent.Headers.ContentType = new MediaTypeHeaderValue(MimeTypes.GetMimeType(_mediaFile.Path));

				content.Add(streamContent, "\"file\"", $"\"{_mediaFile.Path}\"");

				Byte[] byteArray = await content.ReadAsByteArrayAsync().ConfigureAwait(true);
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}/attachments",
					methodRequest = "POST",
					FileUpload = content,
					FileUploadByte = byteArray

				};
				Request request = new Request(jsonRequest);

				// обновление отображения вложений
				JSONRequest jsonRequest2 = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/issue/{issue.key}",
					methodRequest = "GET"

				};
				Request request2 = new Request(jsonRequest2);
				Fields fields = request2.GetResponses<Issue>().fields;
				issueFieldsRefresh(fields);
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
			Navigation.PushAsync(new imageView(attachmentsImage, carouselImages.Position));
		}

		private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{
			Uri uri = new Uri(attachmentsDocument[carouselDocuments.Position].content);
			await Launcher.TryOpenAsync(uri).ConfigureAwait(true);
		}
		private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
		{
			Uri uri = new Uri(attachmentsOther[carouselOthers.Position].content);
			await Launcher.TryOpenAsync(uri).ConfigureAwait(true);
		}
	}
}
