using System;
using System.Collections.Generic;
using Plugin.Settings;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.insight;
using Xamarin.Forms;

namespace RTMobile.issues.viewIssue
{
    public partial class General : ContentPage
    {
        public List<Fields> fieldIssue { get; set; }//поля заявки
        public List<RTMobile.Transition> transition;//Переходы по заявке
        public Issue issue { get; set; }
        public General()
        {
            InitializeComponent();
            this.BindingContext = this;
        }
        public General(Issue issues)
        {
            InitializeComponent();
            issue = issues;
            //Проверяем на существование задачи в памяти
            if (issue != null && issue.key != null)
            {
                //Делаем запрпос на получение расширенных данных по задаче
                Request request = new Request(CrossSettings.Current.GetValueOrDefault("urlServer", string.Empty) + $"/rest/api/2/issue/{issue.key}?expand=names,schema");
                fieldIssue = request.GetCustomField();

                //Проверяем наличие полей, при отсутствии значения скрываем их
                if (issue.fields.resolution == null)
                {
                    resolutionLlb.IsVisible = false;
                    resolution.IsVisible = false;
                }
                else
                {
                    resolutionLlb.IsVisible = true;
                    resolution.IsVisible = true;
                }
               
                if (issue.fields.resolutiondate != null)
                {
                    dateresolution.IsVisible = true;
                    dateresolutionLbl.IsVisible = true;
                }
                else
                {
                    dateresolution.IsVisible = false;
                    dateresolutionLbl.IsVisible = false;
                }
                 if(issue.fields.status.name == "Закрыта")
                {
                    dateClose.IsVisible = true;
                    dateCloseLbl.IsVisible = true;
                }
                else
                {
                    dateClose.IsVisible = false;
                    dateCloseLbl.IsVisible = false;
                }

            }
            this.BindingContext = this;
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

        void showPropertyIssue_Clicked(System.Object sender, System.EventArgs e)
        {
            if (propertyIssue.IsVisible)
            {
                showPropertyIssue.Source = "arrowDown.png";
                propertyFrame.HeightRequest = 70;
                propertyIssue.IsVisible = false;
            }
            else
            {
                showPropertyIssue.Source = "arrowUp.png";
                propertyFrame.HeightRequest = 200;
                propertyIssue.IsVisible = true;
            }
        }

        void showDate_Clicked(System.Object sender, System.EventArgs e)
        {
            if (dateIssue.IsVisible)
            {
                showDate.Source = "arrowDown.png";
                dateFrame.HeightRequest = 70;
                dateIssue.IsVisible = false;
            }
            else
            {
                showDate.Source = "arrowUp.png";
                dateFrame.HeightRequest = 200;
                dateIssue.IsVisible = true;
            }
        }

        void showDetailIssue_Clicked(System.Object sender, System.EventArgs e)
        {
            if (detailIssue.IsVisible)
            {
                showDetailIssue.Source = "arrowDown.png";
                detailFrame.HeightRequest = 70;
                detailIssue.IsVisible = false;
            }
            else
            {
                showDetailIssue.Source = "arrowUp.png";
                detailFrame.HeightRequest = 200;
                detailIssue.IsVisible = true;
            }
        }

        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new History());
        }

        void ToolbarItem_Clicked_1(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new WorkJournal());
        }

        void ToolbarItem_Clicked_2(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment());
        }

        void ToolbarItem_Clicked_3(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new Comment());
        }
    }
}
