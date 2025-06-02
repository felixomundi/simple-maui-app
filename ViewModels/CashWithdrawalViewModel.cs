using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;
using users.Models;
using users.Services;
namespace users.ViewModels
{
    public class CashWithdrawalViewModel : BaseViewModel
    {

        private readonly IApiService _api;

        public string AgentName { get; private set; } = string.Empty;

        public ObservableCollection<Member> AllMembers { get; } = new();
        public ObservableCollection<Member> FilteredMembers { get; } = new();
        public Member? SelectedMember { get; set; }

        public string MemberSearchText { get; set; } = string.Empty;
        public ICommand FilterMembersCommand { get; }

        public ObservableCollection<BankAccount> BankAccounts { get; } = new();
        public BankAccount? SelectedAccount { get; set; }

        public string WithdrawalAmount { get; set; } = string.Empty;

        public ICommand SubmitWithdrawalCommand { get; }
        public CashWithdrawalViewModel(IApiService api)
        {
            _api = api;
            SubmitWithdrawalCommand = new Command(async () => await SubmitWithdrawalAsync());
            FilterMembersCommand = new Command(ApplyMemberFilter);
        }
        
         public async Task LoadDataAsync()
    {
        AgentName = await SecureStorage.Default.GetAsync("user_name") ?? "Unknown";
        OnPropertyChanged(nameof(AgentName));

        var members = await _api.Client.GetFromJsonAsync<List<Member>>("members");
        if (members is not null)
        {
            AllMembers.Clear();
            foreach (var m in members)
                AllMembers.Add(m);

            ApplyMemberFilter();
        }

        var accounts = await _api.Client.GetFromJsonAsync<List<BankAccount>>("accounts");
        if (accounts is not null)
        {
            BankAccounts.Clear();
            foreach (var acct in accounts)
                BankAccounts.Add(acct);
        }
    }

    private void ApplyMemberFilter()
    {
        var filter = MemberSearchText?.ToLower() ?? "";

        var result = AllMembers
            .Where(m => m.DisplayName.ToLower().Contains(filter))
            .ToList();

        FilteredMembers.Clear();
        foreach (var member in result)
            FilteredMembers.Add(member);
    }

    private async Task SubmitWithdrawalAsync()
    {
        if (SelectedMember == null || SelectedAccount == null || string.IsNullOrWhiteSpace(WithdrawalAmount))
        {
            await Shell.Current.DisplayAlert("Error", "Please complete all fields.", "OK");
            return;
        }

        var payload = new
        {
            memberId = SelectedMember.Id,
            accountId = SelectedAccount.Id,
            amount = decimal.Parse(WithdrawalAmount)
        };

        var response = await _api.Client.PostAsJsonAsync("withdraw", payload);

        if (response.IsSuccessStatusCode)
        {
            await Shell.Current.DisplayAlert("Success", "Withdrawal successful.", "OK");
        }
        else
        {
            var msg = await response.Content.ReadAsStringAsync();
            await Shell.Current.DisplayAlert("Error", msg, "OK");
        }
    }

        
    }
}
