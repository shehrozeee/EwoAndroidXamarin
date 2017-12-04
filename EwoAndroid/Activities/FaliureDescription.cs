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
using Newtonsoft.Json;

namespace EwoAndroid.Activities
{
    [Activity(Label = "EwoInfo", Icon = "@drawable/icon", Theme = "@style/AcquaintTheme")]
    public class FaliureDescription: AppCompatActivity
    {
        EditText faliureDecriptioText;
        EWO ewoObj;
        bool editing = false;
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
            faliureDecriptioText = FindViewById<EditText>(Resource.Id.editText1);
            ewoObj = JsonConvert.DeserializeObject<EWO>(Intent.GetStringExtra("ewoObject"));
            editing = Intent.GetBooleanExtra("edit", false);
            if (string.IsNullOrWhiteSpace(ewoObj.faliureDescription))
            {
                faliureDecriptioText.Text = "";
            }
            else
                faliureDecriptioText.Text = ewoObj.faliureDescription;
            Button nextButton = FindViewById<Button>(Resource.Id.NextFaliureDescription);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipFaliureDescription);
            Button backButton = FindViewById<Button>(Resource.Id.BackFaliureDescription);
            backButton.Click += BackButton_Click;
            nextButton.Click += NextButton_Click;
            skipButton.Click += SkipButton_Click;
            if (editing)
                skipButton.Visibility = ViewStates.Invisible;
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            var WandHActivity = new Intent(this, typeof(WandH));
            WandHActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
            StartActivity(WandHActivity);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {

            ewoObj.faliureDescription = faliureDecriptioText.Text;
            if (!editing)
            {
                var WandHActivity = new Intent(this, typeof(WandH));
                WandHActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                StartActivity(WandHActivity);
            }
            else
            {
                
                var OverViewActivity= new Intent(this, typeof(Overview));
                OverViewActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                StartActivity(OverViewActivity);

            }
        }
    }
}