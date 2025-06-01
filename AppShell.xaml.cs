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

            _inactivityTimer = inactivityTimer;
            _inactivityTimer.OnTimeout += HandleTimeout;
            _inactivityTimer.Start();

            SubscribeToTouchEvents();
        }

        private void HandleTimeout()
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                SecureStorage.Default.Remove("access_token");
                await Current.DisplayAlert("Logged Out", "You have been logged out due to inactivity.", "OK");
                await Current.GoToAsync("//LoginPage");
            });
        }

        private void SubscribeToTouchEvents()
        {
            Application.Current!.MainPage.Appearing += (_, __) =>
            {
                AddGestureHandlers(Application.Current!.MainPage);
            };
        }

        private void AddGestureHandlers(Page page)
        {
            var gesture = new TapGestureRecognizer
            {
                Command = new Command(() => _inactivityTimer.Reset())
            };

            if (page is ContentPage contentPage && contentPage.Content != null)
            {
                contentPage.Content.GestureRecognizers.Add(gesture);
            }
        }

    }


}
