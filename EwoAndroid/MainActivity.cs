using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;

namespace EwoAndroid
{
    [Activity(Label = "EwoAndroid", MainLauncher = true, Icon = "@drawable/icon",Theme = "@style/AcquaintTheme")]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.acquaintanceListFloatingActionButton);
            Snackbar
  .Make(fab, "Text Here", Snackbar.LengthLong)
  .SetAction("Party", (x) => { })
  .Show();
        }
    }
}

