using CommunityToolkit.Maui.Core;

namespace PixelSnap;

public partial class Image_Zoomed : ContentPage
{
    string Imagepath { get; set; }

    public Image_Zoomed(string imagepath)
    {
        InitializeComponent();
        Imagepath = imagepath;
        DrawCurrentImage(Imagepath);
    }

    public void DrawCurrentImage(string imagepath)
    {
        ZoomedImage.Source = ImageSource.FromFile(imagepath);
    }

    public void OnBackButtonClicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    public async void OnShareButtonClicked(object sender, EventArgs e)
    {
        await ShareImageAsync(Imagepath);
    }
    public async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        try
        {
            if (File.Exists(Imagepath))
            {
                File.Delete(Imagepath);
            }
            else
            {
                await DisplayAlert("Error", "File not found.", "OK");
            }
            Page_Gallery gal = new Page_Gallery();
            gal.LoadImagesFromGallery();
            Navigation.InsertPageBefore(gal, this);
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not delete image: {ex.Message}", "OK");
        }
    }


    private async Task ShareImageAsync(string imagePath)
    {
        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Share Image",
                File = new ShareFile(imagePath)
            });
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to share: {ex.Message}", "OK");
        }
    }
}
