using Plugin.Settings;
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
    public partial class settingsRT : ContentPage
    {
        public settingsRT()
        {
            InitializeComponent();

            if (CrossSettings.Current.GetValueOrDefault<string>("urlServer").Length <= 0)
            {
                CrossSettings.Current.AddOrUpdateValue<string>("urlServer", "https://sd.rosohrana.ru");
            }

            urlServer.Text = CrossSettings.Current.GetValueOrDefault<string>("urlServer");
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            CrossSettings.Current.AddOrUpdateValue<string>("urlServer", urlServer.Text);
            if (CrossSettings.Current.GetValueOrDefault<string>("urlServer") == urlServer.Text)
            {
                await DisplayAlert("Изменение настроек", "Настройки успешно сохранены", "Закрыть").ConfigureAwait(true); 
            }
            else 
            {
                await DisplayAlert("Изменение настроек", "Ошибка сохранения данных. Повторите попытку позднее", "Закрыть").ConfigureAwait(true);
            }
        }
    }
}