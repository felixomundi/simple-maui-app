using users.Services;
namespace users
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var inactivityTimer = new InactivityTimer(TimeSpan.FromMinutes(5));
            return new Window(new AppShell(inactivityTimer));
        }
    }
}