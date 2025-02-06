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

            // Verwende ein StackLayout statt FlexLayout
            StackLayout stackLayout = (StackLayout)galleryScrollView.Content;
            galleryScrollView.ZIndex = 0;

            if (stackLayout == null)
            {
                stackLayout = new StackLayout
                {
                    Spacing = 5, // Setze Abstand zwischen den Bildern
                    Orientation = StackOrientation.Vertical
                };
                galleryScrollView.Content = stackLayout;
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

            // Füge das Bild in das Frame ein und das Frame in das StackLayout
            imageFrame.Content = image;
            stackLayout.Children.Add(imageFrame);

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
