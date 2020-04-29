using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.insight
{
    public partial class Insight : ContentPage
    {
        public ObservableCollection<Objectschema> insightObject { get; set; }
        public Insight()
        {
            InitializeComponent();
            try
            {
                JSONRequest jsonRequest = new JSONRequest()
                {
                    urlRequest = $"/rest/insight/1.0/objectschema/list",
                    methodRequest = "GET"
                };
                Request request = new Request(jsonRequest);
                //Получаем список избранных фильтров
                insightObject = (request.GetResponses<RootObject>()).objectschemas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }
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

        private async void favoritesList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Objectschema selectedSchema = e.Item as Objectschema;
            if (selectedSchema != null)
            {
                await Navigation.PushAsync(new objectList(selectedSchema)).ConfigureAwait(true);
            }
            ((ListView)sender).SelectedItem = null;
        }
    }
}
