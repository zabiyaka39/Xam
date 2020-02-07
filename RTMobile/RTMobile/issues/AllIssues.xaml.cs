using RTMobile.insight;
using RTMobile.calendar;
using RTMobile.settings;
using RTMobile.about;
using Xamarin.Forms;
using RTMobile.profile;

namespace RTMobile.issues
{
	public partial class AllIssues : MasterDetailPage
	{

		public AllIssues()
		{
			InitializeComponent();
		
			Detail = new NavigationPage(new AllIssuesView());
		}

		void Button_Clicked(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Calendar());
			IsPresented = false;
		}

		void Button_Clicked_1(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Insight());
			IsPresented = false;
		}

		void Button_Clicked_2(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new CreateIssue());
			IsPresented = false;
		}

		void Button_Clicked_3(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Settings());
			IsPresented = false;
		}

		void Button_Clicked_4(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new About());
			IsPresented = false;
		}

		void Button_Clicked_5(System.Object sender, System.EventArgs e)
		{
			IsPresented = false;
		}

		void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
		{
			Detail.Navigation.PushAsync(new Profile());
			IsPresented = false;
		}

	}
}
