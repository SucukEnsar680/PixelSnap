using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelSnap
{
    public class Image_Gal
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }

        public Image_Gal(string imagename, string imagepath)
        {
            ImageName = imagename;
            ImagePath = imagepath;
        }

        public void DrawImage(ScrollView galleryScrollView)
        {
            double widthInLogicalUnits = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;
            double height = (widthInLogicalUnits) / 4 - 8;

            // Verwende FlexLayout anstelle von StackLayout
            var flexLayout = galleryScrollView.Content as FlexLayout;
            galleryScrollView.ZIndex = 0;

            if (flexLayout == null)
            {
                flexLayout = new FlexLayout
                {
                    AlignItems = FlexAlignItems.Center,
                    JustifyContent = FlexJustify.Start,
                    Wrap = FlexWrap.Wrap,
                    Direction = FlexDirection.Row
                };
                galleryScrollView.Content = flexLayout;
            }

            // Erstelle ein Frame für jedes Bild
            Frame imageFrame = new Frame
            {
                WidthRequest = height,
                HeightRequest = height,
                Margin = new Thickness(2, 5, 0, 0),
                Padding = new Thickness(0),
                CornerRadius = 0,
                ZIndex = 0
            };

            Image image = new Image
            {
                Source = ImageSource.FromFile(ImagePath),
                WidthRequest = height,
                HeightRequest = height,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFill,
            };

            // Füge das Bild in das Frame ein und das Frame in das FlexLayout
            imageFrame.Content = image;
            flexLayout.Children.Add(imageFrame);

            // Füge eine Tap-Gesture hinzu, um das Bild in groß anzuzeigen
            imageFrame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var navigation = Application.Current?.MainPage?.Navigation;
                    if (navigation != null)
                    {
                        await navigation.PushAsync(new Image_Zoomed(ImagePath), false);
                    }
                })
            });
        }
    }
}
