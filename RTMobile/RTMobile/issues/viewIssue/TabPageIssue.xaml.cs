using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class TabPageIssue : TabbedPage
    {
        public TabPageIssue(Issue issues)
        {
            InitializeComponent();

            Title = issues.key;
            Children.Add(new General(issues) { Title = "Основное" });
            Children.Add(new Description(issues) { Title = "Описание" });
            Children.Add(new Attachments() { Title = "Вложения" });
            Children.Add(new People(issues) { Title = "Люди" });
        }
    }
}
