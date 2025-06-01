using users.ViewModels;
using users.Services;
namespace users.Pages;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage(IApiService apiService)
    {
        InitializeComponent();
        BindingContext = new ChangePasswordViewModel(apiService);
    }
}
