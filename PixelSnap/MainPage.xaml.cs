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
        public async void Gal_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraViewPage(cameraProvider));
        }
    }

}
