
namespace users.Pages;
using users.ViewModels;
public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
      BindingContext = new SettingsViewModel(DisplayConfirmation);      
    }

    private async Task<bool> DisplayConfirmation(string title, string message, string accept)
    {
        return await DisplayAlert(title, message, accept, "Cancel");
    }
}
