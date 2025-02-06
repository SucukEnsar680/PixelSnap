using CommunityToolkit.Maui.Camera;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using System.Linq.Expressions;
using System.Reflection;

namespace PixelSnap
{
    public partial class CameraViewPage : ContentPage
    {
        readonly string imagePath;
        Image MyImage = new Image();
        private double minValue;
        private double maxValue;
        private ICameraProvider cameraProvider;
        private string Filepath;
        public CameraViewPage(ICameraProvider cameraProvider)
        {
            InitializeComponent();
            this.cameraProvider = cameraProvider;
            MySlider.ValueChanged += Slider_ValueChanged;
            minValue = 1; // Minimum zoom value of the selected camera
            maxValue = 5; // Maximum zoom value of the selected camera
            MySlider.Minimum = minValue;
            MySlider.Maximum = maxValue;

        }

        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            try
            {
                await cameraProvider.RefreshAvailableCameras(CancellationToken.None);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            MyCamera.SelectedCamera = cameraProvider.AvailableCameras
                .Where(c => c.Position == CameraPosition.Front).FirstOrDefault();
        }
        private async void MyCamera_MediaCaptured(object sender, MediaCapturedEventArgs e)
        {
            string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string galleryPath = Path.Combine(exepath, "gallery", "caputredimages");
            Directory.CreateDirectory(galleryPath);
            Directory.CreateDirectory(galleryPath); // Sicherstellen, dass der Ordner existiert
            string fileName = $"image_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.jpg";
            string imagePath = Path.Combine(galleryPath, fileName);
            ConvertPage page = new ConvertPage(true);
            Filepath = imagePath;
            using (var fileStream = new FileStream(Filepath, FileMode.Create, FileAccess.Write))
            {
                await e.Media.CopyToAsync(fileStream);
            }
            MyImage.Source = ImageSource.FromFile(Filepath);
        }

        private async void TakePicture_Clicked(object sender, EventArgs e)
        {
            try
            {
                await MyCamera.CaptureImage(CancellationToken.None);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string galleryPath = Path.Combine(exepath, "gallery", "caputredimages");
            Directory.CreateDirectory(galleryPath);
            Directory.CreateDirectory(galleryPath); // Sicherstellen, dass der Ordner existiert
            string fileName = $"image_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.jpg";
            string imagePath = Path.Combine(galleryPath, fileName);
            ConvertPage page = new ConvertPage(true);
            Filepath = imagePath;
            page.Draw(imagePath, true);
            await Navigation.PushAsync(page);
        }

        private void Flash_Clicked(object sender, EventArgs e)
        {
            MyCamera.CameraFlashMode = MyCamera.CameraFlashMode == CameraFlashMode.Off ? CameraFlashMode.On : CameraFlashMode.Off;
        }
        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            MyCamera.ZoomFactor = (float)MySlider.Value;
        }
        private void SwitchCamera_Clicked(object sender, EventArgs e)
        {
            MyCamera.SelectedCamera = MyCamera.SelectedCamera == cameraProvider.AvailableCameras[0] ? cameraProvider.AvailableCameras[1] : cameraProvider.AvailableCameras[0];
        }

    }
}
