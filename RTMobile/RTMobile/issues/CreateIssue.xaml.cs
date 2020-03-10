using System;
using System.Collections.Generic;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Forms;

namespace RTMobile.issues
{
	public partial class CreateIssue : ContentPage
	{
		List<Project> projects { get; set; }
		List<Issuetype> typeIssue { get; set; }
		public CreateIssue()
		{

			InitializeComponent();
			JSONRequest jsonRequest = new JSONRequest()
			{
				urlRequest = $"/rest/api/2/project",
				methodRequest = "GET"
			};

			Request request = new Request(jsonRequest);

			projects = request.GetResponses<List<Project>>();
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

		private void Picker_SelectedIndexChanged(object sender, EventArgs e)
		{
			JSONRequest jsonRequest = new JSONRequest()
			{
				urlRequest = $"/rest/api/2/issue/createmeta?projectKeys=TELECOM&expand=projects.issuetypes.fields",
				methodRequest = "GET"
			};

			Request request = new Request(jsonRequest);
			typeIssue = request.GetResponses<RootObject>().projects[0].issuetypes;
			List<string> typeIssueName = new List<string>();
			for (int i = 0; i < typeIssue.Count; ++i)
			{
				typeIssueName.Add(typeIssue[i].name);
			}
			typeIssuePic.ItemsSource = typeIssue;

			//Сделать выгрузку полей запросом, т.к. на данный момент выгружаются только типы задач
		}
	}
}
