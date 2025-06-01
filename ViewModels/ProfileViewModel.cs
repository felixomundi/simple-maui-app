using System.Windows.Input;
namespace users.ViewModels
{
    public class ProfileViewModel : BindableObject
    {
        private string _name;
        private string _email;
        private string _phone;

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Phone
        {
            get => _phone;
            set { _phone = value; OnPropertyChanged(); }
        }

        public ICommand EditCommand { get; }

        public ProfileViewModel()
        {
            LoadProfile();
            EditCommand = new Command(OnEdit);
        }

        private async void LoadProfile()
        {
            Name = await SecureStorage.Default.GetAsync("user_name") ?? "";
            Email = await SecureStorage.Default.GetAsync("user_email") ?? "";
            Phone = await SecureStorage.Default.GetAsync("user_phone") ?? "";
        }

        private async void OnEdit()
        {
            await Application.Current.MainPage.DisplayAlert("Edit", "Edit profile not yet implemented.", "OK");
        }
    }
}
