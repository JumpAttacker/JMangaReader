using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Platform = Xamarin.Essentials.Platform;

namespace JMangaReader.Droid
{
    [Activity(Label = "JMangaReader", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        public static Window GlobalWindow { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            // Window.AddFlags(WindowManagerFlags.Fullscreen);
            // Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
            GlobalWindow = Window;
            Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            // CachedImageRenderer.Init(true);
            LoadApplication(new App());

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) !=
                (int) Permission.Granted)
                ActivityCompat.RequestPermissions(this, new[] {Manifest.Permission.WriteExternalStorage}, 0);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) !=
                (int) Permission.Granted)
                ActivityCompat.RequestPermissions(this, new[] {Manifest.Permission.ReadExternalStorage}, 0);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}