using Microsoft.Maui.Layouts;
using SkiaSharp;
using System.Reflection;

namespace PixelSnap;

public partial class ConvertPage : ContentPage
{
    public ConvertPage()
    {
        InitializeComponent();
        Draw(scrollView);
    }

    public void Draw(ScrollView galleryScrollView)
    {
        string ImagePath = "UploadFile.png";
        double widthInLogicalUnits = DeviceDisplay.MainDisplayInfo.Width;
        FlexLayout flexLayout = (FlexLayout)galleryScrollView.Content;
        galleryScrollView.ZIndex = 0;

        if (flexLayout == null)
        {
            flexLayout = new FlexLayout
            {
                AlignItems = FlexAlignItems.Center,  
                JustifyContent = FlexJustify.Start, 
                Wrap = FlexWrap.NoWrap,             
                Direction = FlexDirection.Column     
            };
            galleryScrollView.Content = flexLayout;
        }

        Image image = new Image
        {
            Source = ImageSource.FromFile(ImagePath),
            WidthRequest = double.NaN,
            HeightRequest = 200,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Aspect = Aspect.AspectFit,
        };

        // Add the TapGestureRecognizer directly to the Image
        image.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () =>
            {
                var file = await MediaPicker.PickPhotoAsync();
                if (file != null)
                {
                    image.Source = ImageSource.FromFile(file.FullPath);
                    ImagePath = file.FullPath;
                }
            })
        });

        flexLayout.Children.Add(image);

        Button button = new Button
        {
            Text = "Convert now",
            WidthRequest = widthInLogicalUnits,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
        };

        button.Clicked += (sender, e) =>
        {
            SKBitmap bitmap =  PixelArtConverter.ReduceColors(ImagePath,16);
            string convImg = SaveImage(bitmap);
            Image image_con = new Image
            {
                Source = ImageSource.FromFile(convImg),
                WidthRequest = double.NaN,
                HeightRequest = 200,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit,
            };
            flexLayout.Children.Add(image_con);


        };

        flexLayout.Children.Add(button);
    }
    private static string SaveImage(SKBitmap bitmap)
    {
        string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string galleryPath = Path.Combine(exepath, "gallery", "images");
        Directory.CreateDirectory(galleryPath); // Ensure directory exists

        string fileName = $"image_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.jpg";
        string imagePath = Path.Combine(galleryPath, fileName);
        Console.WriteLine(imagePath);
        PixelArtConverter.SaveImage(bitmap, imagePath);
        return imagePath;
    }
}
