using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.insight
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AttachmentsObjectInsight : ContentPage
	{
		public AttachmentsObjectInsight(ObjectEntry selectedField)
		{
			InitializeComponent();
		}
	}
}