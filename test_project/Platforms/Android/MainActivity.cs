using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using test_project.Droid;

namespace test_project
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestPermissions();
        }

        public async void RequestPermissions()
        {
            var status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            if (status == PermissionStatus.Granted)
            {
                var bt = await Permissions.RequestAsync<Permissions.Bluetooth>();
                if (bt == PermissionStatus.Granted)
                {

                    StartService(new Intent(this, typeof(AndroidService)));
                }
            }


        }
    }
}
