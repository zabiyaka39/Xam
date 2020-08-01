using System;
using Xamarin.Forms;
using RTMobile.issues;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
    public partial class App : Application
    {   
        public App(string data)
        {
            
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage(data));
            //MainPage = new NavigationPage(new MainPage());
            //MainPage = new NavigationPage(new dataIssue.screen());
            //MainPage = new NavigationPage(new RTMobile.IssuePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
