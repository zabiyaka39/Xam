using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.jiraData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public ObservableCollection<InsightObject> Insights { get; set; }
		public GeneralObjectInsight(ObjectEntry selectedField)
		{
			InitializeComponent();
			if (selectedField != null)
			{
				InsightGeneralOptions(selectedField);
				Title = selectedField.name;
			}
			this.BindingContext = this;
		}

		void InsightGeneralOptions(ObjectEntry selectedField)
		{
			JSONRequest jsonRequest = new JSONRequest()
			{
				urlRequest = $"/rest/insight/1.0/object/{selectedField.id}/attributes",
				methodRequest = "GET"
			};
			Request request = new Request(jsonRequest);
			Insights = request.GetResponses<ObservableCollection<InsightObject>>();
			for (int i = 0; i < Insights.Count; ++i)
			{
				//Удаляем параметры объекта у которых нет значенийр
				if (Insights[i].ObjectAttributeValues[0].DisplayValue.Length == 0)
				{
					Insights.RemoveAt(i);
					--i;
				}
			}
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