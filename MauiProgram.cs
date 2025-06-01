using Microsoft.Extensions.Logging;
using users.Pages;
using users.Services;
using users.ViewModels;
namespace users
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddHttpClient<IApiService, ApiService>();
            builder.Services.AddSingleton<SplashScreenPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<RegisterPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<TransactionsPage>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddSingleton<InactivityTimer>();
            return builder.Build();
        }
    }
}
