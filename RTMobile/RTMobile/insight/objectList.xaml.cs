using Microsoft.AppCenter.Crashes;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.issues.viewIssue;
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
	public partial class objectList : ContentPage
	{
		public ObservableCollection<ObjectEntry> insightObject { get; set; }
		public objectList(Objectschema objectschema)
		{
			InitializeComponent();
			Title = objectschema.name;
			if (objectschema != null)
			{
				try
				{
					JSONRequest jsonRequest = new JSONRequest()
					{
						urlRequest = $"/rest/insight/1.0/iql/objects?objectSchemaId={objectschema.id}",
						methodRequest = "GET"
					};
					Request request = new Request(jsonRequest);
					//Получаем список избранных фильтров
					insightObject = (request.GetResponses<RootObject>()).objectEntries;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Crashes.TrackError(ex);
				}
			}
			this.BindingContext = this;

		}

		async void OpenField(object sender, ItemTappedEventArgs e)
		{
			if (e.Item != null)
			{
				ObjectEntry selectedSchema = e.Item as ObjectEntry;
				if (selectedSchema != null)
				{
					await Navigation.PushAsync(new TabPageObjectInsight(selectedSchema)).ConfigureAwait(true);
				}
			}
			((ListView)sender).SelectedItem = null;
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
	}
}