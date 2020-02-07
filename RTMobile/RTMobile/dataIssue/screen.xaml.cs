using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.dataIssue
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class screen : ContentPage
	{
		public ObservableCollection<Fields> fields { get; set; }
		public screen()
		{
			InitializeComponent();

			CrossSettings.Current.AddOrUpdateValue("tmpLogin", "sekisov");
			CrossSettings.Current.AddOrUpdateValue("tmpPassword", "28651455gsbua1A");

			Request request = new Request("https://dev-sd.rosohrana.ru/rest/api/2/issue/KDOP-61168/transitions?expand=transitions.fields&transitionId=271");
			fields = request.GetFieldScreen();

			Grid grid = new Grid();

			//StackLayout stackLayout = new StackLayout();

			for (int i = 0; i < fields.Count; i++)
			{
				string nameField = fields[i].name;
				if (fields[i].required == true)
				{
					nameField += " *";
				}
				Label label = new Label
				{
					Text = nameField,
					FontSize = 23
				};
				grid.Margin = 5;
				grid.Children.Add(label, 0, i);
				switch (fields[i].schema.type)
				{
					case "resolution":
						{
							Picker  picker = new Picker
							{
								Title = nameField
							};
							for (int j = 0; j < fields[i].allowedValues.Count; ++j)
							{
								picker.Items.Add(fields[i].allowedValues[j].name);
							}
							grid.Children.Add(picker, 1, i);
							break;
						}
					case "string":
						{

							grid.Children.Add(label, 1, i);
							break;
						}
				}
			}
			//stackLayout.Children.Add(grid);
			fieldsView.Content = grid;
			this.Content = fieldsView;
		}
	}
}