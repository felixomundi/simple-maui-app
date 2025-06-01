namespace users.Pages
{
    public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

   private async void OnLogoutClicked(object sender, EventArgs e)
    {
        SecureStorage.Default.Remove("access_token");
        await Shell.Current.GoToAsync("//SplashScreenPage");
    }
}
}