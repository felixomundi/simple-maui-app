using users.ViewModels;
namespace users.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            BindingContext = new DashboardViewModel();
        }
    }
}