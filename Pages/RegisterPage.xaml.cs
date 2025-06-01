namespace users.Pages
{
    public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterConfirmClicked(object sender, EventArgs e)
    {
        // simulate registration
        await SecureStorage.Default.SetAsync("access_token", "mock_token");
        await Shell.Current.GoToAsync("//home");
    }
}
}