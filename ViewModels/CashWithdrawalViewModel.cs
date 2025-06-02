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
            AgentName = await SecureStorage.Default.GetAsync("user_name") ?? "";
            OnPropertyChanged(nameof(AgentName));
            var token = await SecureStorage.Default.GetAsync("access_token");
            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.DisplayAlert("Error", "Access token is missing. Please log in again.", "OK");
                await Shell.Current.GoToAsync("//LoginPage"); 
                return;
            }

            try
            {
                // Fetch members
                var membersRequest = new HttpRequestMessage(HttpMethod.Get, "users");
                membersRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var membersResponse = await _api.Client.SendAsync(membersRequest);

                if (membersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await Shell.Current.DisplayAlert("Session Expired", "Please log in again.", "OK");
                    SecureStorage.Default.Remove("access_token");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }
                else if (membersResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    await Shell.Current.DisplayAlert("Server Error", "Unable to fetch members at the moment.", "OK");
                }
                else if (membersResponse.IsSuccessStatusCode)
                {
                    var members = await membersResponse.Content.ReadFromJsonAsync<List<Member>>();
                    if (members is not null)
                    {
                        AllMembers.Clear();
                        foreach (var m in members)
                            AllMembers.Add(m);

                        ApplyMemberFilter();
                    }
                }

                // Fetch accounts
                var accountsRequest = new HttpRequestMessage(HttpMethod.Get, "agency");
                accountsRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var accountsResponse = await _api.Client.SendAsync(accountsRequest);
                if (accountsResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await Shell.Current.DisplayAlert("Session Expired", "Please log in again.", "OK");
                    SecureStorage.Default.Remove("access_token");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return;
                }
                else if (accountsResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    await Shell.Current.DisplayAlert("Server Error", "Unable to fetch accounts at the moment.", "OK");
                }
                else if (accountsResponse.IsSuccessStatusCode)
                {
                    var accounts = await accountsResponse.Content.ReadFromJsonAsync<List<BankAccount>>();
                    if (accounts is not null)
                    {
                        BankAccounts.Clear();
                        foreach (var acct in accounts)
                            BankAccounts.Add(acct);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                await Shell.Current.DisplayAlert("Network Error", "Check your internet connection and try again.\n" + ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Unexpected Error", ex.Message, "OK");
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
