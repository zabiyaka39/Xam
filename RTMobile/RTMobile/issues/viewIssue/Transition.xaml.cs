using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
	public partial class Transition : ContentPage
	{
		public List<Fields> fieldIssue { get; set; }//поля заявки
		public Transition(int transitionId)
		{
			InitializeComponent();

			Request request = new Request(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty)
				+ $"/rest/api/2/issue/IT-5444/transitions?expand=transitions.fields&transitionId=" + transitionId);
			fieldIssue = request.GetFieldTransitions();

			for (int i = 0; i < fieldIssue.Count; ++i)
			{
				if (fieldIssue[i].hasScreen)
				{
					Label label = new Label
					{
						Text = fieldIssue[i].displayName,
						TextColor = Color.FromHex("#F0F1F0"),
						FontSize = 18
					};
					if (fieldIssue[i].required)
					{
						label.Text += "*";
					}
					generalStackLayout.Children.Add(label);
					switch (fieldIssue[i].schema.type)
					{
						case "assignee":
							{
								Picker picker = new Picker
								{
									Title = fieldIssue[i].defaultValue
								};
								picker.Items.Add(fieldIssue[i].defaultValue);
								break;
							}
					}
				}
			}
			//this.Content = layout;
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
