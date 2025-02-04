using Microsoft.Maui.Layouts;

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
            Imagec;
        };

        flexLayout.Children.Add(button);
        Image image_con = new Image
        {
            Source = ImageSource.FromFile(ImagePath),
            WidthRequest = double.NaN,
            HeightRequest = 200,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Aspect = Aspect.AspectFit,
        };
        flexLayout.Children.Add(image_con);
    }
}
