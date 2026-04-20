using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SpotiFlyerMaui.Platforms.Android;
using UraniumUI;

namespace SpotiFlyerMaui
{
    public static class MauiProgram
    {
        private static readonly string Tag = nameof(MauiProgram);
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>()
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Gotu-Regular.ttf", "GotuRegular");
                    fonts.AddFontAwesomeIconFonts();
                    fonts.AddMaterialIconFonts();
                })
                .ConfigureLifecycleEvents(events =>
                {
                    events.AddAndroid(android =>
                    {
                        android.OnCreate((activity, bundle) =>
                        {
                            Console.WriteLine($"{Tag} OnCreate");
                        });
                        android.OnResume(activity =>
                        {
                            Console.WriteLine($"{Tag} OnResume");
                        });
                        android.OnDestroy((activity) =>
                        {
                            Console.WriteLine($"{Tag} OnDestroy");
                            try
                            {
                                activity.UnregisterReceiver(MainActivity.MFinishReceiver);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"{Tag} MFinishReceiver already unregistered");
                            }
                        });
                    });
                });

            // dependency injection
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<IServiceDownload, DownloadService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
