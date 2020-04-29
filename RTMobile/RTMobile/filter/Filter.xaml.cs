using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using RTMobile.calendar;
using RTMobile.insight;
using RTMobile.profile;

using Xamarin.Forms;

namespace RTMobile.filter
{
	public partial class Filter : ContentPage
	{
		public ObservableCollection<Filters> favorites { get; set; }
		public ObservableCollection<Filters> allFilters { get; set; }
		public Filter()
		{
			InitializeComponent();

			try
			{
				JSONRequest jsonRequest = new JSONRequest()
				{
					urlRequest = $"/rest/api/2/filter/favourite",
					methodRequest = "GET"
				};
				Request request = new Request(jsonRequest);
				//Получаем список избранных фильтров
				favorites = request.GetResponses<ObservableCollection<Filters>>();
				favorites.Insert(0, new Filters { name = "Отсутствует" });
				allFilters = favorites;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Crashes.TrackError(ex);
			}
			allFilters.Add(new Filters
			{
				name = "Мои открытые задачи",
				favourite = false
			});
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
		private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
			if (grouping.IsVisible == false)
			{
				buttonShowGroup.Text = "▲";
				grouping.IsVisible = true;
			}
			else
			{
				buttonShowGroup.Text = "▼";
				grouping.IsVisible = false;
			}
		}

		private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
		{
			if (sorted.IsVisible == false)
			{
				buttonShowSorted.Text = "▲";
				sorted.IsVisible = true;
			}
			else
			{
				buttonShowSorted.Text = "▼";
				sorted.IsVisible = false;
			}
		}

		private void ToolbarItem_Clicked(object sender, EventArgs e)
		{

		}
	}
}
