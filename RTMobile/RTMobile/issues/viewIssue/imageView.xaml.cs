using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.issues.viewIssue
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class imageView : ContentPage
	{
		public ObservableCollection<Attachment> attachmentsImage { get; set; }


		public imageView(ObservableCollection<Attachment> attachmentsImage, int checkImage = 0)
		{
			this.attachmentsImage = new ObservableCollection<Attachment>(attachmentsImage);
			InitializeComponent();

			page.ItemsSource = this.attachmentsImage;
			//page.PositionSelected += Carousel_PositionSelected;
			//page.ItemSelected += Carousel_ItemSelected;
		}
	}
}