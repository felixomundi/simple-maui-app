using System.Collections.ObjectModel;
using System.Windows.Input;
using users.Models;
namespace users.ViewModels
{
    public class SettingsViewModel
    {

        public ObservableCollection<SettingItem> Settings { get; } = new ObservableCollection<SettingItem>();

        public ICommand OpenSettingCommand { get; }
        private readonly Func<string, string, string, Task<bool>> _confirm;

        public SettingsViewModel(Func<string, string, string, Task<bool>> confirmDialog)
        {
            _confirm = confirmDialog;
            // Example items
            Settings.Add(new SettingItem { Name = "Profile", Icon = "profile.jpeg", TargetPage = "ProfilePage" });
            Settings.Add(new SettingItem { Name = "Change Password", Icon = "reset_password.png", TargetPage = "ChangePasswordPage" });
            Settings.Add(new SettingItem { Name = "Notifications", Icon = "notifications.png", TargetPage = "LogoutPage" });
            Settings.Add(new SettingItem { Name = "Logout", Icon = "shutdown.png", TargetPage = "LogoutPage" });

            OpenSettingCommand = new Command<SettingItem>(OpenSetting);
        }

        private async void OpenSetting(SettingItem item)
        {
            if (item == null) return;

            if (item.TargetPage == "LogoutPage")
            {
                bool confirmed = await _confirm("Logout", "Are you sure you want to log out?", "Yes");
                if (confirmed)
                {
                    SecureStorage.Default.Remove("access_token");
                    await Shell.Current.GoToAsync("//SplashScreenPage");
                }
                return;
            }

            if (!string.IsNullOrWhiteSpace(item.TargetPage))
            {
                await Shell.Current.GoToAsync(item.TargetPage);
            }
        }
    }
}
