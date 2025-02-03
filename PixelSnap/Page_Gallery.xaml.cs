namespace PixelSnap;

public partial class Page_Gallery : ContentPage
{
	public Page_Gallery()
	{
		InitializeComponent();
        Image_Gal image = new Image_Gal("image1", "C:/Users/thoma/OneDrive/Bilder/Screenshots/2023-11-24.png");
        image.DrawImage(scrollView);
        Image_Gal image1 = new Image_Gal("image1", "C:/Users/thoma/OneDrive/Bilder/Screenshots/2023-11-24.png");
        image1.DrawImage(scrollView);
        Image_Gal image2 = new Image_Gal("image1", "C:/Users/thoma/OneDrive/Bilder/Screenshots/2023-11-12.png");
        image2.DrawImage(scrollView);
        Image_Gal image3 = new Image_Gal("image1", "C:/Users/thoma/OneDrive/Bilder/Screenshots/2023-11-12.png");
        image3.DrawImage(scrollView);
    }
}