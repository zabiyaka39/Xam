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

			  ToolbarItem tbQR= new ToolbarItem
			{
				Text = "QR-код",
				Order = ToolbarItemOrder.Secondary,
				Priority = 1
			};
			tbQR.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(new QRgen(selectedField)).ConfigureAwait(true);
			};
			ToolbarItems.Add(tbQR);

			ToolbarItem tbPrimary = new ToolbarItem
			{
				Text = "Комментарии",
				Order = ToolbarItemOrder.Primary,
				IconImageSource = "commentToolBar.png",
				Priority = 0
			};
			tbPrimary.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(new CommentInsight(selectedField.id.ToString(), selectedField.objectKey)).ConfigureAwait(true);
			};
			ToolbarItems.Add(tbPrimary);
			ToolbarItem tb = new ToolbarItem
			{
				Text = "Комментарии",
				Order = ToolbarItemOrder.Secondary,
				IconImageSource = "comment.png",
				Priority = 0
			};
			tb.Clicked += async (sender, args) =>
			{
				await Navigation.PushAsync(new CommentInsight(selectedField.id.ToString(), selectedField.label)).ConfigureAwait(true);
			};
			ToolbarItems.Add(tb);
		}
	}
}