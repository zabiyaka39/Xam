using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using RTMobile.profile;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class Description : ContentPage
	{
		public Issue issue { get; set; }
		private List<RTMobile.Transition> transition { get; set; }
		public Description(Issue issue)
		{
			this.issue = issue;
			InitializeComponent();
			if (issue != null && issue.fields != null)
			{
				if (issue.fields.description != null && issue.fields.description.Length > 0)
				{
					description.Text = issue.fields.description;
				}
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

		void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new History(issue.key, issue.fields.summary));
		}

		void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new WorkJournal(issue.key, issue.fields.summary));
		}
		void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Navigation.PushAsync(new Comment(issue.key, issue.fields.summary));
		}


		void showDescriptionIssue_Clicked(System.Object sender, System.EventArgs e)
		{
			if (description.IsVisible)
			{
				showDescription.Source = "arrowDown.png";
				//descriptionFrame.HeightRequest = 70;
				description.IsVisible = false;
				descriptionFrame.VerticalOptions = LayoutOptions.Start;
			}
			else
			{
				showDescription.Source = "arrowUp.png";
				//descriptionFrame.HeightRequest = 200;
				descriptionFrame.VerticalOptions = LayoutOptions.FillAndExpand;
				description.IsVisible = true;
			}
		}

		private void ToolbarItem_Clicked_4(object sender, EventArgs e)
		{

		}
		private void ToolbarItem_Clicked_5(object sender, EventArgs e)
		{

		}
		private void ToolbarItem_Clicked_6(object sender, EventArgs e)
		{

		}

	}
}
