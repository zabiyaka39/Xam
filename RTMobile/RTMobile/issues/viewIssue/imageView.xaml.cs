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
	public partial class imageView : CarouselPage
	{
		ObservableCollection<Attachment> attachmentsImage = new ObservableCollection<Attachment>();
		public imageView(ObservableCollection<Attachment> attachmentsImage)
		{
			this.attachmentsImage = attachmentsImage;

			InitializeComponent();
			this.BindingContext = this;
		}
	}
}