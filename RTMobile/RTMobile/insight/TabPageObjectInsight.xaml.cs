using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile.insight
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TabPageObjectInsight : TabbedPage
	{
		public TabPageObjectInsight(ObjectEntry selectedField)
		{
			InitializeComponent();

			Title = selectedField.objectKey;

			Children.Add(new GeneralObjectInsight(selectedField) { Title = "Сведения" });
			Children.Add(new IssueObjectInsight(selectedField) { Title = "Задачи" });
			Children.Add(new AttachmentsObjectInsight(selectedField) { Title = "Вложения" });
			Children.Add(new LinkObjectInsight(selectedField) { Title = "Ссылки" });

			//Добавляем выпадающее меню справа для действий по объекту
			//try
			//{
			//	if (issues != null)
			//	{
			//		JSONRequest jsonRequest = new JSONRequest
			//		{
			//			urlRequest = $"/rest/api/2/issue/{issues.key}/transitions/",
			//			methodRequest = "GET"
			//		};
			//		Request request = new Request(jsonRequest);

			//		transition = request.GetResponses<RootObject>().transitions;
			//		for (int i = 0; i < transition.Count; ++i)
			//		{
			//			ToolbarItem tb = new ToolbarItem
			//			{
			//				Text = transition[i].name,
			//				Order = ToolbarItemOrder.Secondary,
			//				Priority = i + 1
			//			};
			//			tb.Clicked += async (sender, args) =>
			//			{
			//				await Navigation.PushAsync(new Transition(int.Parse(transition[((ToolbarItem)sender).Priority - 1].id), issues.key)).ConfigureAwait(true);
			//			};
			//			ToolbarItems.Add(tb);
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
			//	Console.WriteLine(ex.Message);
			//	Crashes.TrackError(ex);
			//}
		}
	}
}