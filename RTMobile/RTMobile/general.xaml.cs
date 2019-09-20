using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RTMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class general : Xamarin.Forms.TabbedPage
    {
        [Obsolete]
        public general()
        {
            InitializeComponent();
            Title = "Задача";

            //_ = On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            //_ = On<Android>().SetToolbarPlacement(ToolbarItems[0].Text);
        }
    }
}