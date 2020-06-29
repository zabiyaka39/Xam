using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Mail;
using Plugin.Settings;
using System.Net;

namespace RTMobile
{
	public partial class About : ContentPage
    {
        List<string> recipients { get; set; }
        string mail { get; set; }
        string password { get; set; }
        public About()
        {
            InitializeComponent();
            VersionTracking.Track();
            versionApp.Text = "Версия: " + VersionTracking.CurrentVersion;
            List<string> recipients = new List<string>() {/* "a.kotochigov@rosohrana.ru",*/ "sekisov@rosohrana.ru" };
            mail = CrossSettings.Current.GetValueOrDefault("login", string.Empty);
            password = CrossSettings.Current.GetValueOrDefault("password", string.Empty);
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
        void Feedback_clicked(object sender, System.EventArgs e)
        {
            if (fb.IsVisible == true)
            {
                fb.IsVisible = false;
                arroW.Source = "arrowDown.png";
                boxViewSeparator.IsVisible = false;
            }
            else
            {
                fb.IsVisible = true;
                arroW.Source = "arrowUp.png";
                boxViewSeparator.IsVisible = true;
            }
        }
        private async Task SendEmailAsync()
		{

			try
			{
				MailAddress from = new MailAddress(String.Format("{0}.@rosohrana.ru", CrossSettings.Current.GetValueOrDefault("login", string.Empty)));
				MailAddress to = new MailAddress("sekisov@rosohrana.ru");
				MailMessage message = new MailMessage(from, to);
				message.Subject = FBHead.Text;
				message.Body = FBBody.Text;
				SmtpClient client = new SmtpClient("mail.rosohrana.ru", 443);
				client.Credentials = new NetworkCredential(@"rosohrana\"+CrossSettings.Current.GetValueOrDefault("login", string.Empty), CrossSettings.Current.GetValueOrDefault("password", string.Empty));
				client.EnableSsl = true;
				await client.SendMailAsync(message);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}

		void FeedbacSender_clicked(object sender, System.EventArgs e)
        {
			SendEmailAsync();
		}
        private static int countClick = 0;
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
		{
            if (countClick < 2 || countClick > 7)
            {
                frameLogo.CornerRadius = 0;
                logoCompany.HeightRequest = 60;
                logoCompany.WidthRequest = 50;
                VersionTracking.Track();
                versionApp.Text = "Версия: " + VersionTracking.CurrentVersion;
                logoCompany.Source = "rosohranaLogo.png";
                ++countClick;
            }
            else
            {
                if (countClick < 5)
                {
                    versionApp.Text = "Секисов Владислав Александрович";
                    logoCompany.Source = "sekisov.jpg";
                }
                else
                {
                    versionApp.Text = "Коточигов Алексей Викторович";
                    logoCompany.Source = "kotochigov.jpg";
                    if (countClick >= 7)
                    {
                        countClick = 0;
                    }
                }
                ++countClick;
                frameLogo.CornerRadius = 50;
                logoCompany.HeightRequest = 150;
                logoCompany.WidthRequest = 150;                
                
            }
		}
	}   
    
}
