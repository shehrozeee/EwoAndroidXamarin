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
    [Activity(Label = "EwoInfo", Theme = "@style/AcquaintTheme")]
    public class EwoInfo : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.EwoInfo);

            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "EW Details";

            Button nextButton = FindViewById<Button>(Resource.Id.NextButtonInfo);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipButtonInfo);
            Button backButton = FindViewById<Button>(Resource.Id.BackButtonInfo);
            nextButton.Click += NextButton_Click;
            backButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(PictureSelectActivity));
        }
    }
}