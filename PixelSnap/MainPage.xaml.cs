using CommunityToolkit.Maui.Core;
using System.Runtime.CompilerServices;

namespace PixelSnap
{
    public partial class MainPage : ContentPage
    {
        ICameraProvider cameraProvider;
        public MainPage(ICameraProvider cameraProvider)
        {
            InitializeComponent();
            this.cameraProvider = cameraProvider;
        }
        public MainPage()
        {
            InitializeComponent();
        }
        public async void Open_Gallery(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Page_Gallery());
        }
        public async void Open_Converter(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConvertPage());
        }
        public async void Open_Camera(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraViewPage(cameraProvider));
        }
    }

}
