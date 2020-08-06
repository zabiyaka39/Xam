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
using Windows.System.Power.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;


namespace RTMobile.insight
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class objectList : ContentPage
	{
		public ObservableCollection<ObjectEntry> insightObject { get; set; }
		private delegate object Poster();
		public ObservableCollection<ObjectEntry> InsightProjectStuff { get; set; }
		private Dictionary<int, Poster> Shemaobj;


		public objectList(Objectschema objectschema)
		{
			InitializeComponent();
			Title = objectschema.name;
			if (objectschema != null)
			{            
				Shemaobj = new Dictionary<int, Poster>
				{
					{6, KPA_object}
				};			
	
				try
				{
					//СОздаем запрос на получение списка объектов по id в количестве 1000 элементов
					JSONRequest jsonRequest = new JSONRequest()
					{
						urlRequest = $"/rest/insight/1.0/iql/objects?objectSchemaId={objectschema.id}&resultPerPage=1000",
						methodRequest = "GET"
					};
					Request request = new Request(jsonRequest);
					//Получаем список избранных фильтров
					insightObject = (request.GetResponses<RootObject>()).objectEntries;
					if (Shemaobj.ContainsKey(objectschema.id))
					{
						Shemaobj[objectschema.id]();
					}
					else
                    {
						InsightProjectStuff = new ObservableCollection<ObjectEntry>();
						insightObject.ForEach<ObjectEntry>((item) =>
						{
							InsightProjectStuff.Add(item);
						});
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					Crashes.TrackError(ex);
				}
			}
			this.BindingContext = this;

		}

		async Task KPA_object()
        {
			InsightProjectStuff = new ObservableCollection<ObjectEntry>();
			Task KPA = Task.Run(() =>
			{
				foreach (ObjectEntry obj in insightObject)
				{
					if ((string)obj.objectType.name == "Рубеж")
						InsightProjectStuff.Add(obj);
				}
			});
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