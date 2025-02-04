namespace PixelSnap
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public async void Gal_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConvertPage());
        }
    }

}
