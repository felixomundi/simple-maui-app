
using users.Services;
namespace users
{
    public partial class MainPage : ContentPage
    {

        private readonly UserService _userService = new();
        public MainPage()
        {
            InitializeComponent();
            LoadUsers();
        }     

        private async void LoadUsers()
        {
            var users = await _userService.GetUsersAsync();
            UsersListView.ItemsSource = users;
        }
    }
}
