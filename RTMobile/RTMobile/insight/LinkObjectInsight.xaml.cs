using Microsoft.AppCenter.Crashes;
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
	public partial class LinkObjectInsight : ContentPage
	{
		public ObservableCollection<ObjectEntry> Selectedobject {get;set;}
		public LinkObjectInsight(ObjectEntry selectedField)
		{
			InitializeComponent();
			takejiraIssueList(selectedField);
			this.BindingContext = this;
		
		}


		/// <summary>
		/// Запрос на получение списка задач связанных с данным объектом
		/// </summary>
		/// <param name="selectedField"></param>
		void takejiraIssueList(ObjectEntry selectedField)
		{
			JSONRequest jsonRequest = new JSONRequest()
			{
				urlRequest = $"/rest/insight/1.0/iql/objects?includeExtendedInfo=true&iql=object HAVING outR(Key = {selectedField.objectKey})",
				methodRequest = "GET"
			};
			Request request = new Request(jsonRequest);
			Selectedobject = request.GetResponses<RootObject>().objectEntries;

		}
		

		async private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			Issue selectedIssue = e.Item as Issue;
			if (selectedIssue != null)
			{
				await Navigation.PushAsync(new RTMobile.issues.viewIssue.TabPageIssue(selectedIssue)).ConfigureAwait(true);
			}
			((ListView)sender).SelectedItem = null;
		}
	}
}

