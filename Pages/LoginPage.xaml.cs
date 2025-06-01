using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using users.Services;
using users.Models;
namespace users.Pages
{
    public partial class LoginPage : ContentPage
    {
        private bool _isPasswordVisible = false;
        private readonly IApiService _api;

        public LoginPage(IApiService apiService)
        {
            InitializeComponent();
            _api = apiService;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            try
            {
                string? email = EmailEntry.Text?.Trim();
                string password = PasswordEntry.Text;
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Error", "Please enter both email and password.", "OK");
                    return;
                }

                if (!IsValidEmail(email))
                {
                    await DisplayAlert("Error", "Please enter a valid email address.", "OK");
                    return;
                }
                LoginButton.IsEnabled = false;
                LoginActivityIndicator.IsVisible = true;
                LoginActivityIndicator.IsRunning = true;

                var loginPayload = new { email, password };
                var content = new StringContent(JsonSerializer.Serialize(loginPayload), System.Text.Encoding.UTF8, "application/json");
                var response = await _api.Client.PostAsync("login", content);
                var body = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    await SecureStorage.Default.SetAsync("access_token", result.Token);
                    await SecureStorage.Default.SetAsync("user_name", result.Name);
                    await SecureStorage.Default.SetAsync("user_email", result.Email);
                    await SecureStorage.Default.SetAsync("user_phone", result.Phone);
                    await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    var error = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                    if (error != null && error.TryGetValue("message", out var message))
                    {
                        await DisplayAlert("Login Failed", message ?? "An error occurred.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                LoginButton.IsEnabled = true;
                LoginActivityIndicator.IsRunning = false;
                LoginActivityIndicator.IsVisible = false;
            }

        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SplashScreenPage");
        }

        private void OnTogglePasswordClicked(object sender, EventArgs e)
        {
            _isPasswordVisible = !_isPasswordVisible;
            PasswordEntry.IsPassword = !_isPasswordVisible;
            TogglePasswordButton.Source = _isPasswordVisible ? "view.png" : "hide.png";
        }

        private async void OnForgotPasswordTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Reset Password", "Forgot password flow not yet implemented.", "OK");
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}