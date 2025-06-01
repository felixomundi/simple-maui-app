using users.Services;
using users.ViewModels;
namespace users.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(IApiService apiService)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(apiService);
        }
    }

}