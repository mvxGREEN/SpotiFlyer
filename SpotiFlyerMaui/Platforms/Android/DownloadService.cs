using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;

namespace SpotiFlyerMaui.Platforms.Android
{
    [Service(Name = "com.mvxgreen.spotifydownloader.DownloadService",
         Exported = true,
         ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeDataSync)]
    public class DownloadService : Service, IServiceDownload
    {
        private static string Tag = "DownloadService";
        public const int NOTIFICATION_ID = 399;
        const string channelId = "spotiflyer_channel";
        const string channelName = "SpotiFlyer Downloads";
        const string notificationTitle = "Downloading…";

        bool isForeground = false;

        public override IBinder OnBind(Intent intent) => null;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Console.WriteLine($"{Tag} OnStartCommand");

            if (intent?.Action == "STOP_SERVICE")
            {
                // CORRECT: Call the internal cleanup method
                KillService();
                return StartCommandResult.NotSticky;
            }
            else if (intent?.Action == "START_SERVICE")
            {
                Spotiflyer.OnProgressUpdate += UpdateNotification;
                RegisterNotification();

                if (Spotiflyer.MInputs != null && Spotiflyer.MInputs.Count > 0)
                {
                    Task.Run(async () => await Spotiflyer.DownloadTrack(Spotiflyer.MInputs[0]));
                }
            }

            return StartCommandResult.Sticky;
        }

        // This is the method called by the UI (MainPage)
        public void Start()
        {
            var intent = new Intent(Platform.AppContext, typeof(DownloadService));
            intent.SetAction("START_SERVICE");

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                Platform.AppContext.StartForegroundService(intent);
            }
            else
            {
                Platform.AppContext.StartService(intent);
            }
        }

        // This is the method called by the UI (MainPage)
        public void Stop()
        {
            // CORRECT: Send an intent to the running service instead of trying to stop "this" instance
            var intent = new Intent(Platform.AppContext, typeof(DownloadService));
            intent.SetAction("STOP_SERVICE");
            Platform.AppContext.StartService(intent);
        }

        // This method runs inside the actual Service context to clean up
        private void KillService()
        {
            try
            {
                NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
                manager.Cancel(NOTIFICATION_ID);

                isForeground = false;

                Spotiflyer.OnProgressUpdate -= UpdateNotification;

                // Remove notification and detach from foreground
                StopForeground(StopForegroundFlags.Remove);

                // Kill the service
                StopSelf();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Tag} Error stopping service: {ex.Message}");
            }
        }

        private void RegisterNotification()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;

            var manager = (NotificationManager)GetSystemService(NotificationService);

            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Low);
            channel.Description = "Shows download progress";
            manager.CreateNotificationChannel(channel);

            var notification = BuildNotification(0, 100);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                StartForeground(NOTIFICATION_ID, notification, ForegroundService.TypeDataSync);
            }
            else
            {
                StartForeground(NOTIFICATION_ID, notification);
            }

            isForeground = true;
        }

        private Notification BuildNotification(int progress, int max)
        {
            var pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
                ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.UpdateCurrent;

            var uiIntent = new Intent(this, typeof(MainActivity));
            uiIntent.SetFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, uiIntent, pendingIntentFlags);

            var builder = new NotificationCompat.Builder(this, channelId)
                .SetContentTitle(notificationTitle)
                .SetContentText($"{progress}%")
                .SetSmallIcon(Resource.Drawable.downloader_raw)
                .SetProgress(max, progress, progress == 0)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent)
                .SetOnlyAlertOnce(true);

            return builder.Build();
        }

        public void UpdateNotification(int progress)
        {
            if (!isForeground) return;

            var manager = (NotificationManager)GetSystemService(NotificationService);
            var notification = BuildNotification(progress, 100);
            manager.Notify(NOTIFICATION_ID, notification);
        }
    }
}