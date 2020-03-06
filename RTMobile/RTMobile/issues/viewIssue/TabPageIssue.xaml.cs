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
            if (issues != null)
            {
                Title = issues.key;

                //Сделать в этом файле обработчик задачи по всем полям и передавать уже заполненную переменную issues

                JSONRequest jsonRequestUser = new JSONRequest
                {
                    urlRequest = $"/rest/api/2/issue/{issues.key}?fields=*all",
                    methodRequest = "GET"
                };
                Request request = new Request(jsonRequestUser);
                Issue issue = request.GetResponses<Issue>();

                Children.Add(new General(issue) { Title = "Основное" });
                Children.Add(new Description(issue) { Title = "Описание" });
                Children.Add(new Attachments(issue) { Title = "Вложения" });
                Children.Add(new People(issue) { Title = "Люди" });
            }
        }
    }
}
