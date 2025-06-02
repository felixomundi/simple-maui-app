using users.Pages;
using users.Services;
namespace users
{
    public partial class AppShell : Shell
    {
        private readonly InactivityTimer _inactivityTimer;
        public AppShell(InactivityTimer inactivityTimer)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
            Routing.RegisterRoute(nameof(ChangePasswordPage), typeof(ChangePasswordPage));
            Routing.RegisterRoute(nameof(CashWithdrawalPage), typeof(CashWithdrawalPage));
            _inactivityTimer = inactivityTimer;
            _inactivityTimer.OnTimeout += HandleTimeout;
            _inactivityTimer.Start();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (CurrentPage is ContentPage page)
            {
                AddGestureHandlers(page);
            }
        }

        private void HandleTimeout()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SecureStorage.Default.Remove("access_token");
                await Current.DisplayAlert("Logged Out", "You have been logged out due to inactivity.", "OK");
                await GoToAsync("//LoginPage");
            });
        }

        private void AddGestureHandlers(ContentPage page)
        {
            var gesture = new TapGestureRecognizer
            {
                Command = new Command(() => _inactivityTimer.Reset())
            };

            if (page.Content != null)
            {
                page.Content.GestureRecognizers.Add(gesture);
            }
        }

    }


}
