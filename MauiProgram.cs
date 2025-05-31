using Microsoft.Extensions.Logging;
// using users;
// using users.Services;
// using users.ViewModels;
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

            // builder.Services.AddSingleton<UserService>();
            // builder.Services.AddSingleton<ViewModel>();
            // builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
