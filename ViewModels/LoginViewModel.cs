// using System.ComponentModel;
// using System.Runtime.CompilerServices;
// using System.Text.Json;
// using System.Text.RegularExpressions;
// using System.Windows.Input;
// using users.Models;
// using users.Services;
// using System.Net.Http.Json;

// namespace users.ViewModels
// {
//     public class LoginViewModel : INotifyPropertyChanged
//     {
//         private readonly IApiService _api;

//         public LoginViewModel(IApiService api)
//         {
//             _api = api;
//             LoginCommand = new Command(async () => await LoginAsync(), CanExecuteLogin);
//             NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
//             CancelCommand = new Command(async () => await Shell.Current.GoToAsync("//SplashScreenPage"));
//             TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);
//             ForgotPasswordCommand = new Command(async () => await Shell.Current.DisplayAlert("Reset Password", "Forgot password flow not yet implemented.", "OK"));
//         }

//         private string _email;
//         public string Email
//         {
//             get => _email;
//             set { _email = value; OnPropertyChanged(); ((Command)LoginCommand).ChangeCanExecute(); }
//         }

//         private string _password;
//         public string Password
//         {
//             get => _password;
//             set { _password = value; OnPropertyChanged(); ((Command)LoginCommand).ChangeCanExecute(); }
//         }

//         private bool _isBusy;
//         public bool IsBusy
//         {
//             get => _isBusy;
//             set { _isBusy = value; OnPropertyChanged(); ((Command)LoginCommand).ChangeCanExecute(); }
//         }
//         private bool _isPasswordVisible;
//         public bool IsPasswordVisible
//         {
//             get => _isPasswordVisible;
//             set { _isPasswordVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(PasswordVisibilityIcon)); }
//         }

//         public string PasswordVisibilityIcon => IsPasswordVisible ? "view.png" : "hide.png";

//         public ICommand LoginCommand { get; }
//         public ICommand TogglePasswordVisibilityCommand { get; }
//         public ICommand NavigateToRegisterCommand { get; }
//         public ICommand CancelCommand { get; }
//         public ICommand ForgotPasswordCommand { get; }

//         private async Task LoginAsync()
//         {
//              try
//             {
//             if (string.IsNullOrWhiteSpace(Email))
//             {
//                 await Shell.Current.DisplayAlert("Error", "Please enter email address.", "OK");
//                 return;
//             }
//             if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
//             {
//                 await Shell.Current.DisplayAlert("Error", "Please enter a valid email address.", "OK");
//                 return;
//             }
//             if (string.IsNullOrWhiteSpace(Password))
//             {
//                 await Shell.Current.DisplayAlert("Error", "Please enter password.", "OK");
//                 return;
//             }

//             IsBusy = true;

//                 var loginPayload = new { email = Email, password = Password };
//                 var content = new StringContent(JsonSerializer.Serialize(loginPayload), System.Text.Encoding.UTF8, "application/json");
//                 var response = await _api.Client.PostAsync("login", content);
//                 var body = await response.Content.ReadAsStringAsync();

//                 if (response.IsSuccessStatusCode)
//                 {
//                     var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
//                     await SecureStorage.Default.SetAsync("access_token", result.Token);
//                     await SecureStorage.Default.SetAsync("user_name", result.Name);
//                     await SecureStorage.Default.SetAsync("user_email", result.Email);
//                     await SecureStorage.Default.SetAsync("user_phone", result.Phone);
//                     await Shell.Current.GoToAsync("//home");
//                 }
//                 else
//                 {
//                     var error = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
//                     if (error != null && error.TryGetValue("message", out var message))
//                     {
//                         await Shell.Current.DisplayAlert("Login Failed", message ?? "An error occurred.", "OK");
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
//             }
//             finally
//             {
//                 IsBusy = false;
//             }
//         }
//         private void TogglePasswordVisibility()
//         {
//             IsPasswordVisible = !IsPasswordVisible;
//         }

//         private bool CanExecuteLogin() => !IsBusy && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);

//         public event PropertyChangedEventHandler PropertyChanged;
//         private void OnPropertyChanged([CallerMemberName] string name = "") =>
//             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//     }
// }


using System.ComponentModel;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using users.Models;
using users.Services;

namespace users.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IApiService _apiService;

        public LoginViewModel(IApiService apiService)
        {
            _apiService = apiService;
            // Default password is hidden
            PasswordEntryIsPassword = true;
            LoginCommand = new Command(async () => await ExecuteLogin());
            TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);
            NavigateToRegisterCommand = new Command(async () => await Shell.Current.GoToAsync("//RegisterPage"));
            CancelCommand = new Command(async () => await Shell.Current.GoToAsync("//SplashScreenPage"));
            ForgotPasswordCommand = new Command(async () => await DisplayForgotPasswordAlert());
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _passwordEntryIsPassword;
        public bool PasswordEntryIsPassword
        {
            get => _passwordEntryIsPassword;
            set
            {
                if (SetProperty(ref _passwordEntryIsPassword, value))
                {
                    OnPropertyChanged(nameof(TogglePasswordIcon));
                }
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public bool IsNotBusy => !IsBusy;
        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        private void TogglePasswordVisibility()
        {
            PasswordEntryIsPassword = !PasswordEntryIsPassword;
        }

        private async Task ExecuteLogin()
        {
            if (IsBusy) return;

            try
            {
                if (string.IsNullOrWhiteSpace(Email))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter email address.", "OK");
                    return;
                }
                if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter a valid email address.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(Password))
                {
                    await Shell.Current.DisplayAlert("Error", "Please enter password.", "OK");
                    return;
                }
                IsBusy = true;

                var loginPayload = new { email = Email.Trim(), password = Password };
                var response = await _apiService.Client.PostAsJsonAsync("login", loginPayload);

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
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent);

                    if (error != null && error.TryGetValue("message", out var message))
                        await Shell.Current.DisplayAlert("Login Failed", message ?? "An error occurred.", "OK");
                    else
                        await Shell.Current.DisplayAlert("Login Failed", "An error occurred.", "OK");
                }
            }
            catch (System.Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public string TogglePasswordIcon => PasswordEntryIsPassword ? "view.png" : "hide.png";

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private async Task DisplayForgotPasswordAlert()
        {
            await Shell.Current.DisplayAlert("Reset Password", "Forgot password flow not yet implemented.", "OK");
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
        {
            if (object.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
