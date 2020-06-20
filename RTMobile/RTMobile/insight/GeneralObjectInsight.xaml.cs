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
		public ObjectEntry selectedField { get; set; }
		public ObservableCollection<jiraData.Attribute> InsightsInfo { get; set; }
		public ObservableCollection<jiraData.Attribute> InsightsDate { get; set; }
		public GeneralObjectInsight(ObjectEntry selectedField)
		{
			InitializeComponent();
			if (selectedField != null)
			{
				InsightGeneralOptions(selectedField);
				Title = selectedField.name;
				this.selectedField = selectedField;
			}
			
			this.BindingContext = this;
		}

		void InsightGeneralOptions(ObjectEntry selectedField)
		{
			JSONRequest jsonRequest = new JSONRequest()
			{
				urlRequest = $"/rest/insight/1.0/object/{selectedField.objectKey}",
				methodRequest = "GET"
			};
			Request request = new Request(jsonRequest);
			InsightsInfo = request.GetResponses<InsightRoot>().Attributes;

			for (int i = 0; i < InsightsInfo.Count; ++i)
			{
				if (InsightsInfo[i].ObjectAttributeValues != null && InsightsInfo[i].ObjectAttributeValues.Count > 0)
				{
					//Удаляем параметры объекта у которых нет значенийр
					if (InsightsInfo[i].ObjectAttributeValues[0].DisplayValue.Length == 0)
					{
						InsightsInfo.RemoveAt(i);
						--i;
					}
					else
					{
						if (InsightsInfo[i].ObjectTypeAttribute.DefaultType != null)
						{
							//Если данные являются датой, то переносим в блок с датами
							if (InsightsInfo[i].ObjectTypeAttribute.DefaultType.Name == "DateTime")
							{
								if (InsightsDate == null)
								{
									InsightsDate = new ObservableCollection<jiraData.Attribute>();
								}
								InsightsDate.Add(InsightsInfo[i]);
								InsightsInfo.RemoveAt(i);
								--i;
							}
						}
					}
				}
				else
				{
					InsightsInfo.RemoveAt(i);
					--i;
				}
			}
			//Выставляем размер ListView размером с шрифт + отступ
			InformationObject.HeightRequest = InsightsInfo.Count * 25;
		}

		private void ToolbarItem_Clicked(object sender, EventArgs e)
		{

		}

		private void ToolbarItem_Clicked_1(object sender, EventArgs e)
		{

		}
		
		private void ToolbarItem_Clicked_QR(object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new QRgen(selectedField));
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