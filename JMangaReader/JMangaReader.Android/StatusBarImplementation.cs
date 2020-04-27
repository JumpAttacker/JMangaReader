using Android.App;
using Android.Views;
using JMangaReader.Droid;
using JMangaReader.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(StatusBarImplementation))]

namespace JMangaReader.Droid
{
    public class StatusBarImplementation : IStatusBar
    {
        private WindowManagerFlags _originalFlags;

        #region IStatusBar implementation

        public void HideStatusBar()
        {
            //var activity = (Activity)Forms.Context;
            //var attrs = activity.Window.Attributes;
            //_originalFlags = attrs.Flags;
            //attrs.Flags |= Android.Views.WindowManagerFlags.Fullscreen;
            //activity.Window.Attributes = attrs;
            if (MainActivity.GlobalWindow == null) return;
            _originalFlags = MainActivity.GlobalWindow.Attributes.Flags;
            MainActivity.GlobalWindow.AddFlags(WindowManagerFlags.Fullscreen);
            MainActivity.GlobalWindow.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
        }

        public void ShowStatusBar()
        {
            if (MainActivity.GlobalWindow == null) return;
            var window = MainActivity.GlobalWindow;
            var attrs = window.Attributes;
            attrs.Flags = _originalFlags;
            window.Attributes = attrs;
        }

        #endregion
    }
}