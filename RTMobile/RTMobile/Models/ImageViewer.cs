using System;
using Xamarin.Forms;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Android.Media;

namespace RTMobile.Models
{
    public class ImageViewer : ViewCell
    {
        CachedImage cacheImage;
        Label imyaFamiliya;
        readonly CachedImage cachedImage = null;

        public ImageViewer()
        {
            cacheImage = new CachedImage()
            {

                CacheDuration = TimeSpan.FromDays(7),
                DownsampleToViewSize = true,
                HeightRequest =50,
                WidthRequest = 50,
                RetryCount = 0,
                RetryDelay = 250,
                BitmapOptimizations = false,
                Transformations = new System.Collections.Generic.List<FFImageLoading.Work.ITransformation>()
                {
                  new CircleTransformation(),
                }

            };
            imyaFamiliya = new Label() { TextColor = Color.White, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            StackLayout cellWalper = new StackLayout()
            {
                Padding = new Thickness(5, 5, 5, 5),
                Orientation = StackOrientation.Horizontal,
                Children = { cacheImage, imyaFamiliya }
            };

            View = cellWalper;
        }

        public static readonly BindableProperty ImagePathProperty =
            BindableProperty.Create("ImagePath", typeof(string), typeof(ImageViewer), "");
        public static readonly BindableProperty TextPathProperty =
            BindableProperty.Create("Text", typeof(string), typeof(ImageViewer), "");

        public string ImagePath
        {
            set { SetValue(ImagePathProperty, value); }
            get { return (string)GetValue(ImagePathProperty); }
        }

        public string Text
        {
            set { SetValue(TextPathProperty, value); }
            get { return (string)GetValue(TextPathProperty); }
        }
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            cacheImage.Source = null;
            if (BindingContext != null)
            {
                cacheImage.Source = ImagePath;
                imyaFamiliya.Text = Text;
            };
        }

    }
}