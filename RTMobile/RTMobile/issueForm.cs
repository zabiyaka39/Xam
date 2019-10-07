using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Input;
using Xamarin.Forms;

namespace RTMobile
{
    class issueForm
    {
        public string keyIssue { get; set; }

        /// <summary>
        /// Фрейм задач для кастомизации и показа теней
        /// </summary>
        Frame frameIssue = new Frame()
        {
            CornerRadius = 10,
            HasShadow = false,
            BackgroundColor = Color.White,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.CenterAndExpand
        };

        /// <summary>
        /// Основной контейнер хранящий в себе разделы задачи
        /// </summary>
        StackLayout stackLayoutGeneral = new StackLayout()
        {
            VerticalOptions = LayoutOptions.StartAndExpand
        };

        /// <summary>
        /// Вернхняя часть с основной информацией по задаче
        /// </summary>
        StackLayout stackLayoutHeader = new StackLayout()
        {
            Margin = new Thickness(0, 20, 0, 0),
            HorizontalOptions = LayoutOptions.FillAndExpand,
            Orientation = StackOrientation.Horizontal
        };

        /// <summary>
        /// Верхняя левая часть блока задачи
        /// </summary>
        StackLayout stackLayoutHeaderLeft = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal,
            HorizontalOptions = LayoutOptions.StartAndExpand
        };

        /// <summary>
        /// Хранение изображение Исполнителя по заявке
        /// </summary>
        Frame frameAvatarIssue = new Frame()
        {
            CornerRadius = 50,
            HeightRequest = 2,
            WidthRequest = 2,
            IsClippedToBounds = true,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 10, 0)
        };

        /// <summary>
        /// Аватар исполнителя задачи
        /// </summary>
        Image imageAvatarUserAnother = new Image()
        {
            Source = "sekisovAvatar.jpg",
            Aspect = Aspect.AspectFill,
            Margin = new Thickness(-20, -20, -20, -20),
            HeightRequest = 10,
            WidthRequest = 10
        };

        /// <summary>
        /// Контейнер храящий Номер и тип задачи
        /// </summary>
        StackLayout stackLayoutHeaderLeftIssueGeneralInformation = new StackLayout()
        {
            Orientation = StackOrientation.Vertical,
            HorizontalOptions = LayoutOptions.FillAndExpand

            //Добавить событие по нажатию!!!!!!!!!!!!!!!!!!
        };

        /// <summary>
        /// Номер задачи
        /// </summary>
        Label labelNumberIssue = new Label()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            TextColor = Color.FromHex("#6666ff"),
            Text = "Добавить сюда текст из задачи"//!!!!!!!!!!!!!!!!!!
        };

        /// <summary>
        /// Контейнер хранящий Название типа задачи и изображение типа задачи
        /// </summary>
        StackLayout stackLayoutHeaderLeftIssueGeneralInformationType = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal
        };

        /// <summary>
        /// Изображение типа задачи
        /// </summary>
        Image imageTypeIssue = new Image()
        {
            Source = "statusIssue"
            //исправить на изображение статуса задачи!!!!!!!!!!!!!!!!!!
        };

        /// <summary>
        /// название типа задачи
        /// </summary>
        Label typeIsse = new Label()
        {
            Text = "Исправить тип задачи" //!!!!!!!!!!!!!!!!!!
        };

        /// <summary>
        /// Верхняя правая часть блок задачи
        /// </summary>
        StackLayout stackLayoutHeaderRight = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal
            //Добавить событие по нажатию!!!!!!!!!!!!!!!!!!
        };

        /// <summary>
        /// Фрейм статуса задачи для кастомизации и показа теней
        /// </summary>
        Frame statusFrame = new Frame()
        {
            HorizontalOptions = LayoutOptions.End
        };

        /// <summary>
        /// онтейнер хранящий Название статуса задачи и иконки
        /// </summary>
        StackLayout stackLayoutStatusIssue = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal
        };

        /// <summary>
        /// Иконка статуса задачи
        /// </summary>
        Image imageStatusIssue = new Image()
        {
            Source = "statusImageIssue.png"
        };

        /// <summary>
        /// Название статуса задачи
        /// </summary>
        Label labelStatusImage = new Label()
        {
            Text = "Исправить текст статуса" //!!!!!!!!!!!!!!!!!!
        };

        /// <summary>
        /// Кнопка с дополнительными действиями
        /// </summary>
        ImageButton moreButton = new ImageButton()
        {
            Margin = new Thickness(5, 0, 0, 0),
            Source = "moreVertical.png"
        };

        /// <summary>
        /// Центральный контейнер хранящий в себе 
        /// </summary>
        StackLayout stackLayoutCenter = new StackLayout()
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.CenterAndExpand
            //Добавить событие по клику !!!!!!!!!!!!!!!!!!
        };





        /// <summary>
        /// Контейнер хранящий тему задачи и количество комментариев
        /// </summary>
        StackLayout stackLayoutSummaryComment = new StackLayout()
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand
        };

        /// <summary>
        /// Тема задачи
        /// </summary>
        Label summary = new Label()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            FontSize = 15,
            TextColor = Color.Black,
            VerticalOptions = LayoutOptions.CenterAndExpand,
            Text = "Добавить текст темы задачи" //!!!!!!!!!!!!!!
        };

        /// <summary>
        /// Количество комментариев
        /// </summary>
        Label countComment = new Label()
        {
            HorizontalOptions = LayoutOptions.End,
            FontSize = 10
        };
        
        /// <summary>
        /// разделитель
        /// </summary>
        BoxView separatorBox = new BoxView()
        {
            Color = Color.Black,
            HeightRequest = 1
        };

        /// <summary>
        /// Нижний контейнер хранящий в себе ссылки на основные параметры задачи
        /// </summary>
        StackLayout stackLayoutFooter = new StackLayout()
        {
            Orientation = StackOrientation.Horizontal
        };

        /// <summary>
        /// Наблюдение за задачей
        /// </summary>
        Label watcher = new Label()
        {
            HorizontalOptions = LayoutOptions.StartAndExpand,
            Text = "наблюдатели"
        };

        /// <summary>
        /// Комментарии задачи
        /// </summary>
        Label commentaries = new Label()
        {
            HorizontalOptions = LayoutOptions.CenterAndExpand,
            Text = "Комментарии"
        };

        /// <summary>
        /// Поделиться задачей 
        /// </summary>
        Label share = new Label()
        {
            HorizontalOptions = LayoutOptions.EndAndExpand,
            Text = "Поделиться"
        };

        issueForm()
        {
            frameIssue.Content = stackLayoutGeneral;

            //Верхняя часть
            {
                //Начало верхней левой части
                frameAvatarIssue.Content = imageAvatarUserAnother;

                stackLayoutHeaderLeftIssueGeneralInformationType.Children.Add(imageTypeIssue);
                stackLayoutHeaderLeftIssueGeneralInformationType.Children.Add(typeIsse);

                stackLayoutHeaderLeftIssueGeneralInformation.Children.Add(labelNumberIssue);
                stackLayoutHeaderLeftIssueGeneralInformation.Children.Add(stackLayoutHeaderLeftIssueGeneralInformationType);

                stackLayoutHeaderLeft.Children.Add(frameAvatarIssue);
                stackLayoutHeaderLeft.Children.Add(stackLayoutHeaderLeftIssueGeneralInformation);

                stackLayoutHeader.Children.Add(stackLayoutHeaderLeft);
                //Конец определениея верхенй левой части


                //Начало верхней правой части
                stackLayoutStatusIssue.Children.Add(imageStatusIssue);
                stackLayoutStatusIssue.Children.Add(labelStatusImage);

                statusFrame.Content = stackLayoutStatusIssue;

                stackLayoutHeaderRight.Children.Add(statusFrame);
                stackLayoutHeaderRight.Children.Add(moreButton);

                stackLayoutHeaderRight.Children.Add(stackLayoutHeaderRight);

                stackLayoutHeader.Children.Add(stackLayoutHeaderRight);
                //Конец определения верхней правой части

            }

            //Центральная часть
            {
                stackLayoutSummaryComment.Children.Add(summary);
                stackLayoutSummaryComment.Children.Add(countComment);

                stackLayoutCenter.Children.Add(stackLayoutSummaryComment);
                stackLayoutCenter.Children.Add(separatorBox);
            }

            //Нижняя часть
            {

            }

            stackLayoutGeneral.Children.Add(stackLayoutHeader);
            stackLayoutGeneral.Children.Add(stackLayoutCenter);
            stackLayoutGeneral.Children.Add(stackLayoutFooter);

        }

        issueForm(Issue issue)
        {

        }
    }

}
