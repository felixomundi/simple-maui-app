using users.ViewModels;
using users.Services;
namespace users.Pages
{
    public partial class CashWithdrawalPage : ContentPage
    {
 
        public CashWithdrawalPage(IApiService apiService)
        {
            InitializeComponent();
            BindingContext = new CashWithdrawalViewModel(apiService);
        }
    }
}
