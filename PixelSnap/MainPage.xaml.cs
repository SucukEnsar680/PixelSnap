using CommunityToolkit.Maui.Core;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PixelSnap
{
    public partial class MainPage : ContentPage
    {
        ICameraProvider cameraProvider;
        public MainPage(ICameraProvider cameraProvider)
        {
            InitializeComponent();
            LoadImagesFromGallery();
            this.cameraProvider = cameraProvider;
        }
        public MainPage()
        {
            InitializeComponent();
            LoadImagesFromGallery();
        }
        public async void Open_Gallery(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
        public async void Open_Converter(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConvertPage(cameraProvider));
        }
        public async void Open_Camera(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraViewPage(cameraProvider));
        }
        public void LoadImagesFromGallery()
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

}
