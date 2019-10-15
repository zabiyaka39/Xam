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
        Issue issue = new Issue();
        [Obsolete]
        public general()
        {
            InitializeComponent();
            Title = "Задача";

            //_ = On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            //_ = On<Android>().SetToolbarPlacement(ToolbarItems[0].Text);
        }

        public general(Issue issues)
        {
            InitializeComponent();
            Title = "Задача " + issues.key;
            issue = issues;
            Children.Add(new viewIssue(issue) { Title = "Основное"});
            Children.Add(new Commentaries(issue) { Title = "Комментарии"});
            Children.Add(new workJournal(issue) { Title = "Рабочий журнал"});
            Children.Add(new history(issue) { Title = "История"});
            //_ = On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            //_ = On<Android>().SetToolbarPlacement(ToolbarItems[0].Text);
        }
    }
}