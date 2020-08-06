using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;

namespace RTMobile.jiraData
{
    public partial class StatusBar
    {
        public StatusBar()
        {
            InitializeComponent();
            OnBackgroundClicked();
        }
        protected override bool OnBackgroundClicked()
        {
            return false;
        }
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return true;
        }
    }
}