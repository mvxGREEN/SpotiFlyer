using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using UraniumUI.Material.Controls;
using static SpotiFlyerMaui.MainPage;

namespace SpotiFlyerMaui
{
    [Activity(Theme = "@style/MainTheme.NoActionBar", MainLauncher = true, Exported = true, LaunchMode = LaunchMode.SingleInstance, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionSend },
          Categories = new[] {
              Intent.CategoryDefault
          },
          DataMimeType = "*/*")]
    [MetaData(name: "com.google.android.play.billingclient.version", Value = "7.1.1")]
    public class MainActivity : MauiAppCompatActivity
    {
        private static string Tag = nameof(MainActivity);

        public static FinishReceiver MFinishReceiver = new();

        public static MainActivity ActivityCurrent { get; set; }
        public MainActivity()
        {
            ActivityCurrent = this;
        }

        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            Console.WriteLine($"{Tag}: OnCreate");

            //EdgeToEdge.Enable(this);
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            AskPermissions();

        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            Console.WriteLine($"{Tag}: OnNewIntent");

            CheckForIntent(intent);

        }

        public async Task CheckForIntent()
        {
            CheckForIntent(this.Intent);
        }

        public async Task CheckForIntent(Intent intent)
        {
            if (intent != null)
            {
                var data = intent.GetStringExtra(Intent.ExtraText);
                if (data != null)
                {
                    Console.WriteLine($"{Tag}: received data from intent: {data}");

                    MainPage mp = (MainPage)Shell.Current.CurrentPage;
                    await mp.ClearTextfield();
                    await mp.ShowEmptyUI();

                    Spotiflyer.MIsShared = true;

                    string SharedText = data.ToString();
                    TextField mTextField = (TextField)mp.FindByName("main_textfield");
                    if (mTextField != null)
                    {
                        mTextField.Text = SharedText;
                        mp.HandleInput(SharedText);
                    }
                    else
                    {
                        Console.WriteLine($"{Tag} null textfield!");
                    }
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        private void AskPermissions()
        {
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.PostNotifications) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(
                    Platform.CurrentActivity, new string[] { Android.Manifest.Permission.ReadMediaAudio, Android.Manifest.Permission.PostNotifications }, 101);
            }
            if ((int)Build.VERSION.SdkInt >= 33
                && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadMediaAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(
                    Platform.CurrentActivity, new string[] { Android.Manifest.Permission.ReadMediaAudio }, 101);
            }
            else if ((int)Build.VERSION.SdkInt < 33
                && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(
                Platform.CurrentActivity, new string[] { Android.Manifest.Permission.ReadExternalStorage, Android.Manifest.Permission.WriteExternalStorage }, 101);
            }
        }

    }
}
