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
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is CashWithdrawalViewModel vm)
            {
                await vm.LoadDataAsync(); 
            }
        }

    }
}
