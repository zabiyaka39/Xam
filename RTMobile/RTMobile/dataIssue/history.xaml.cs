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
    public partial class history : ContentPage
    {
        Issue issue = new Issue();
        public history()
        {
            InitializeComponent();
        }
        public history(Issue issues)
        {
            InitializeComponent();
            issue = issues;
        }
    }
}