using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;


namespace test_project.Droid
{
    [Service(ForegroundServiceType = global::Android.Content.PM.ForegroundService.TypeConnectedDevice, Name = "com.example.DisposeMediaElementBug.AndroidService")]
    public class AndroidService : Service
    {
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 33;
        private const string ForegroundChannelId = "21";
        public bool IsServiceRunning { get; set; }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override async void OnDestroy()
        {

            Process.KillProcess(Process.MyPid());

        }
        PendingIntent GetActionIntent(string action)
        {
            var i = new Intent(this, typeof(AndroidService));
            i.SetAction(action);
            return PendingIntent.GetService(this, 0, i, PendingIntentFlags.Immutable);
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            if(intent?.Action == "KILL")
            {
                StopSelf();
            }
            else
            {
                var notification =
               CreateNotification(
               "Test app",
               "Service running",
               "Stop",
                GetActionIntent("KILL")
               );


                // Start the service in the foreground
                // Android Q
                if (OperatingSystem.IsAndroidVersionAtLeast(29))
                {
                    StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification, ForegroundService.TypeConnectedDevice);
                }
                // Android O
                else if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
                }
                else
                {
                    StartService(intent);
                }




            }
                return StartCommandResult.Sticky;
           
    }


        private Notification CreateNotification(string title, string content, string textAction = null, PendingIntent actionIntent = null)
        {
            var context = Platform.AppContext;

         
            var intent = new Intent(context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.SingleTop);

            PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.Immutable);
          

            var notificationBuilder =
                new NotificationCompat.Builder(context, ForegroundChannelId)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(title)
                .SetStyle(new NotificationCompat.BigTextStyle())
                .SetSmallIcon(Resource.Drawable.abc_ab_share_pack_mtrl_alpha)
                .SetContentText(content)
                .SetOngoing(true)
                ;

            if (actionIntent != null)
            {

                notificationBuilder.AddAction(_Microsoft.Android.Resource.Designer.ResourceConstant.Drawable.abc_ab_share_pack_mtrl_alpha, "Stop", actionIntent);
            }


            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notificationChannel = new NotificationChannel(ForegroundChannelId, "Running", NotificationImportance.Low);
                notificationChannel.SetShowBadge(true);

                var notificationManager = Platform.AppContext.GetSystemService(NotificationService) as NotificationManager;
                if (notificationManager != null)
                {
                    notificationBuilder.SetChannelId(ForegroundChannelId);
                    notificationManager.CreateNotificationChannel(notificationChannel);
                }
               
            }

            return notificationBuilder.Build();
        }
    }
}
