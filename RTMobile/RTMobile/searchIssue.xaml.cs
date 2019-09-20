using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class searchIssue : ContentPage
    {
        public searchIssue()
        {
            InitializeComponent();
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {

        }
    }
}