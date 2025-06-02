using System.Windows.Input;
using users.Pages;
namespace users.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public ICommand CashWithdrawalCommand { get; }
        public ICommand CashDepositCommand { get; }
        public ICommand CheckBalanceCommand { get; }
        public ICommand PayAirtimeCommand { get; }

        public DashboardViewModel()
        {
            CashWithdrawalCommand = new Command(OnCashWithdrawal);
            // CashDepositCommand = new Command(OnCashDeposit);
            // CheckBalanceCommand = new Command(OnCheckBalance);
            // PayAirtimeCommand = new Command(OnPayAirtime);
        }

        private async void OnCashWithdrawal() =>
            await Shell.Current.GoToAsync(nameof(CashWithdrawalPage));

        // private async void OnCashDeposit() =>
        //     await Shell.Current.GoToAsync(nameof(CashDepositPage));

        // private async void OnCheckBalance() =>
        //     await Shell.Current.GoToAsync(nameof(CheckBalancePage));

        // private async void OnPayAirtime() =>
        //     await Shell.Current.GoToAsync(nameof(PayAirtimePage));
    }
}
