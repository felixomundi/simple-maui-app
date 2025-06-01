using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using System.Net.Http.Json;
using users.Services;
using System.Net;
using users.Pages;

namespace users.ViewModels
{
    public class ChangePasswordViewModel : INotifyPropertyChanged
    {
        private string _currentPassword;
        private string _newPassword;
        private string _confirmPassword;
        private bool _isBusy;
        private readonly IApiService _api;

        public string CurrentPassword
        {
            get => _currentPassword;
            set => SetProperty(ref _currentPassword, value);
        }

        public string NewPassword
        {
            get => _newPassword;
            set => SetProperty(ref _newPassword, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        private bool _isCurrentPasswordHidden = true;
        public bool IsCurrentPasswordHidden
        {
            get => _isCurrentPasswordHidden;
            set => SetProperty(ref _isCurrentPasswordHidden, value);
        }

        private bool _isNewPasswordHidden = true;
        public bool IsNewPasswordHidden
        {
            get => _isNewPasswordHidden;
            set => SetProperty(ref _isNewPasswordHidden, value);
        }

        private bool _isConfirmPasswordHidden = true;
        public bool IsConfirmPasswordHidden
        {
            get => _isConfirmPasswordHidden;
            set => SetProperty(ref _isConfirmPasswordHidden, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                SetProperty(ref _isBusy, value);
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
        public string CurrentPasswordEyeIcon => IsCurrentPasswordHidden ? "view.png" : "hide.png";
        public string NewPasswordEyeIcon => IsNewPasswordHidden ? "view.png" : "hide.png";
        public string ConfirmPasswordEyeIcon => IsConfirmPasswordHidden ? "view.png" : "hide.png";
        public bool IsNotBusy => !IsBusy;

        public ICommand ChangePasswordCommand { get; }
        public ICommand ToggleCurrentPasswordVisibilityCommand { get; }
        public ICommand ToggleNewPasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }

        public ChangePasswordViewModel(IApiService apiService)
        {
            _api = apiService;

            ChangePasswordCommand = new Command(async () => await ChangePasswordAsync(), () => IsNotBusy);
            ToggleCurrentPasswordVisibilityCommand = new Command(ToggleCurrentPasswordVisibility);
            ToggleNewPasswordVisibilityCommand = new Command(ToggleNewPasswordVisibility);
            ToggleConfirmPasswordVisibilityCommand = new Command(ToggleConfirmPasswordVisibility);
        }

        private async Task ChangePasswordAsync()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Current Password is required", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New Password is required", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Confirm Password is required", "OK");
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "New password and confirmation do not match.", "OK");
                return;
            }

            IsBusy = true;
            ((Command)ChangePasswordCommand).ChangeCanExecute();

            try
            {
                var token = await SecureStorage.Default.GetAsync("access_token");
                if (string.IsNullOrEmpty(token))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "You are not logged in.", "OK");
                    return;
                }

                var payload = new
                {
                    current_password = CurrentPassword,
                    new_password = NewPassword
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _api.Client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _api.Client.PatchAsync("change-password", content);
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    SecureStorage.Default.Remove("access_token");
                    var currentPage = Application.Current.Windows[0].Page;
                    await currentPage.DisplayAlert("Session Expired", "You have been logged out. Please log in again.", "OK");

                    // Redirect to login page
                    // await currentPage.Navigation.PushAsync(new LoginPage(_api));
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }
                else if (response.IsSuccessStatusCode)
                {
                    // await Application.Current.MainPage.DisplayAlert("Success", "Password changed successfully.", "OK");
                    var Body = await response.Content.ReadAsStringAsync();
                    var errorMsg = JsonSerializer.Deserialize<Dictionary<string, string>>(Body);
                    if (errorMsg != null && errorMsg.TryGetValue("message", out var message))
                        await Application.Current.MainPage.DisplayAlert("Success", message, "OK");

                    // Clear password fields
                    CurrentPassword = "";
                    NewPassword = "";
                    ConfirmPassword = "";
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var errorMsg = JsonSerializer.Deserialize<Dictionary<string, string>>(errorBody);
                        if (errorMsg != null && errorMsg.TryGetValue("message", out var message))
                            await Application.Current.MainPage.DisplayAlert("Error", message, "OK");
                        else
                            await Application.Current.MainPage.DisplayAlert("Error", "Failed to change password.", "OK");
                    }
                    catch
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to change password.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;

                ((Command)ChangePasswordCommand).ChangeCanExecute();
            }
        }
        private void ToggleCurrentPasswordVisibility()
        {
            IsCurrentPasswordHidden = !IsCurrentPasswordHidden;
            OnPropertyChanged(nameof(CurrentPasswordEyeIcon));
        }

        private void ToggleNewPasswordVisibility()
        {
            IsNewPasswordHidden = !IsNewPasswordHidden;
            OnPropertyChanged(nameof(NewPasswordEyeIcon));
        }

        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
            OnPropertyChanged(nameof(ConfirmPasswordEyeIcon));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
