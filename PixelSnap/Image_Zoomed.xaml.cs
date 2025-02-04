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
	public void OnMenuButtonClicked(object sender, EventArgs e)
    {
        Navigation.PopToRootAsync();
    }
}