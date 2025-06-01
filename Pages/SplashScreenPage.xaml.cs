namespace users.Pages
{
    public partial class SplashScreenPage : ContentPage
    {
        public SplashScreenPage()
        {
            InitializeComponent();
            // CheckAuthAndRedirect();
        }

        private async void CheckAuthAndRedirect()
        {
            await Task.Delay(1000);
            var token = await SecureStorage.Default.GetAsync("access_token");
            if (!string.IsNullOrEmpty(token))
                await Shell.Current.GoToAsync("//home");
            else
                await Shell.Current.GoToAsync("//LoginPage");
        }
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
         private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }
    }
}