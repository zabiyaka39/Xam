using RTMobile.calendar;
using RTMobile.filter;
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
	public partial class GeneralObjectInsight : ContentPage
	{
		public GeneralObjectInsight(ObjectEntry selectedField)
		{
			InitializeComponent();
		}

		private void ToolbarItem_Clicked(object sender, EventArgs e)
		{

		}

		private void ToolbarItem_Clicked_1(object sender, EventArgs e)
		{

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

		private void showDetailIssue_Clicked(object sender, EventArgs e)
		{

		}

		private void showDate_Clicked(object sender, EventArgs e)
		{

		}
	}
}