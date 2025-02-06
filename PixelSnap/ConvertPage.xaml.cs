using Microsoft.Maui.Layouts;
using SkiaSharp;
using System.Reflection;

namespace PixelSnap;

public partial class ConvertPage : ContentPage
{
    private Image image_con;
    private int colorCount = 16;
    public ConvertPage(bool Withcam = false)
    {
        InitializeComponent();
        if (Withcam != true)
        {
            Draw();
        }
    }

    public void Draw(string ImagePath = "UploadFile.png", bool WithCam = false)
    {
        double widthInLogicalUnits = DeviceDisplay.MainDisplayInfo.Width;
        FlexLayout flexLayout = (FlexLayout)scrollView.Content;
        scrollView.ZIndex = 0;

        if (flexLayout == null)
        {
            flexLayout = new FlexLayout
            {
                AlignItems = FlexAlignItems.Center,
                JustifyContent = FlexJustify.Start,
                Wrap = FlexWrap.NoWrap,
                Direction = FlexDirection.Column
            };
            scrollView.Content = flexLayout;
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

        // Bildauswahl beim Antippen
        if (!WithCam)
        {
            image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var file = await MediaPicker.PickPhotoAsync();
                    if (file != null)
                    {
                        ImagePath = file.FullPath;
                        image.Source = ImageSource.FromFile(file.FullPath);
                    }
                })
            });
        }
        else { image.Source = ImagePath; }
        flexLayout.Children.Add(image);

        Button button = new Button
        {
            Text = "Convert now",
            
            WidthRequest = 400,
            HeightRequest = 50,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromHex("#C58AFF"),
        };

        button.Clicked += async (sender, e) =>
        {
            if (ImagePath == "UploadFile.png")
            {
                await DisplayAlert("Error", "Please select an image first", "OK");
                return;
            }
            button.Text = "Konvertiere...";
            button.IsEnabled = false; // Button deaktivieren während der Konvertierung
            if (txtInput.Text != null && int.TryParse(txtInput.Text, out int inputValue) && inputValue >= 2 && inputValue <= 256)
            {
                colorCount = inputValue;
            }
            else
            {
                await DisplayAlert("Info", "Accepted Values: 2-256; Image will contain 16 (standard) colors", "OK");
            }
            await Task.Run(() =>
            {
                SKBitmap bitmap = PixelArtConverter.ReduceColors(ImagePath, colorCount);
                string convImg = SaveImage(bitmap);

                // UI-Update im Hauptthread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (image_con != null)
                    {
                        flexLayout.Children.Remove(image_con);
                    }

                    // Neues konvertiertes Bild hinzufügen
                    image_con = new Image
                    {
                        Source = ImageSource.FromFile(convImg),
                        WidthRequest = double.NaN,
                        HeightRequest = 200,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        Aspect = Aspect.AspectFit,
                    };
                    flexLayout.Children.Add(image_con);

                    button.Text = "Convert now"; // Button-Text zurücksetzen
                    button.IsEnabled = true; // Button wieder aktivieren
                    if (WithCam)
                    {
                    }

                });
            });
        };
        flexLayout.Children.Add(button);
        string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string galleryPath = Path.Combine(exepath, "gallery", "caputredimages");
        if (Directory.Exists(galleryPath))
        {
            try
            {
                Directory.Delete(galleryPath, true); // Rekursives Löschen
                Console.WriteLine($"Gelöscht: {galleryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Löschen des Ordners: {ex.Message}");
            }
        }
    }

    private static string SaveImage(SKBitmap bitmap)
    {
        string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string galleryPath = Path.Combine(exepath, "gallery", "images");
        Directory.CreateDirectory(galleryPath); // Sicherstellen, dass der Ordner existiert

        string fileName = $"image_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.jpg";
        string imagePath = Path.Combine(galleryPath, fileName);
        Console.WriteLine(imagePath);
        PixelArtConverter.SaveImage(bitmap, imagePath);
        return imagePath;
    }

    public async void Open_Gallery(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

}
