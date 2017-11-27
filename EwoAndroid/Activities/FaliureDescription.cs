using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;

namespace EwoAndroid.Activities
{
    [Activity(Label = "EwoInfo", Icon = "@drawable/icon", Theme = "@style/AcquaintTheme")]
    public class FaliureDescription: AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FaliureDescriptionLayout);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "Faliure Description";

            Button nextButton = FindViewById<Button>(Resource.Id.NextFaliureDescription);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipFaliureDescription);
            Button backButton = FindViewById<Button>(Resource.Id.BackFaliureDescription);
            nextButton.Click += NextButton_Click;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(this, typeof(WandH)));
        }
    }
}