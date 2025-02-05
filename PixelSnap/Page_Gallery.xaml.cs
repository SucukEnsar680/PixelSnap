using System.Reflection;

namespace PixelSnap;

public partial class Page_Gallery : ContentPage
{
	public Page_Gallery()
	{
		InitializeComponent();
        LoadImagesFromGallery();
    }
    private void LoadImagesFromGallery()
    {
        string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string galleryPath = Path.Combine(exepath, "gallery", "images");
        if (!Directory.Exists(galleryPath))
        {
            Console.WriteLine("Gallery folder does not exist.");
            return;
        }
        string[] imagePaths = Directory.GetFiles(galleryPath, "*.*")
            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                           file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                           file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                           file.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (imagePaths.Length == 0)
        {
            Console.WriteLine("No images found in the gallery.");
            return;
        }
        foreach (var imagePath in imagePaths)
        {
            Image_Gal image = new Image_Gal(Path.GetFileName(imagePath), imagePath);
            image.DrawImage(scrollView);
        }
    }
}