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
    public partial class Commentaries : ContentPage
    {
        Issue issue = new Issue();
        public Commentaries()
        {
            InitializeComponent();
        }
        public Commentaries(Issue issues)
        {
            InitializeComponent();
            issue = issues;
        }
    }
}