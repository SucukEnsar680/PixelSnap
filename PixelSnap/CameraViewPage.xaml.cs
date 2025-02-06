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
        private bool isFlashOn = false;
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
            UpdateFlashIcon();

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
            try
            {
                string exepath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string galleryPath = Path.Combine(exepath, "gallery", "capturedimages");
                Directory.CreateDirectory(galleryPath); // Nur einmal erstellen

                string fileName = $"image_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.jpg";
                string imagePath = Path.Combine(galleryPath, fileName);
                Filepath = imagePath; // Hier einmal korrekt zuweisen

                if (File.Exists(imagePath))
                {
                    throw new Exception("File already exists");
                }

                using (var fileStream = new FileStream(Filepath, FileMode.Create, FileAccess.Write))
                {
                    await e.Media.CopyToAsync(fileStream);
                }

                // UI im Hauptthread aktualisieren
                Device.BeginInvokeOnMainThread(() =>
                {
                    MyImage.Source = ImageSource.FromFile(Filepath);
                    ConvertPage page = new ConvertPage(true);
                    page.Draw(Filepath, true);
                    Navigation.PushAsync(page);
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
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
        }

        private void Flash_Clicked(object sender, EventArgs e)
        {
            isFlashOn = !isFlashOn;
            MyCamera.CameraFlashMode = MyCamera.CameraFlashMode == CameraFlashMode.Off ? CameraFlashMode.On : CameraFlashMode.Off;
            UpdateFlashIcon();
        }

        private void UpdateFlashIcon()
        {
            FlashImage.Source = isFlashOn ? "blitzfill.png" : "blitz.png";
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
