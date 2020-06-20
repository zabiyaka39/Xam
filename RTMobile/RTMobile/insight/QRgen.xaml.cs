using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Crashes;
using RTMobile.calendar;
using RTMobile.filter;
using RTMobile.profile;
using ZXing.Net.Mobile.Forms;
using System.Diagnostics.Tracing;

namespace RTMobile.insight
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QRgen : ContentPage
    {
        public QRgen(ObjectEntry selectedField)
        {
            try
            {
                InitializeComponent();
                Issuetype imageDownload = new Issuetype(){ iconUrl = new Uri($"https://sd.rosohrana.ru/rest/insight/1.0/qrcode/object/{selectedField.id}/code.png?size=300")};
                image.Source = imageDownload.icon;
                this.BindingContext = this;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Crashes.TrackError(ex);
            }
        }


    }
}