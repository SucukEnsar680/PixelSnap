namespace PixelSnap
{
    public partial class MainPage : ContentPage
    {
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
    }

}
