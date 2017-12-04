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
    public class RootCause : AppCompatActivity
    {
        RadioButton r1;
        RadioButton r2;
        RadioButton r3;
        RadioButton r4;
        RadioButton r5;
        RadioButton r6;


        EWO ewoObj;
        bool editing = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.RootCauseLayout);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "Type of Root Cause";
            r1 = FindViewById<RadioButton>(Resource.Id.radioButtonr1);
            r2 = FindViewById<RadioButton>(Resource.Id.radioButtonr2);
            r3 = FindViewById<RadioButton>(Resource.Id.radioButtonr3);
            r4 = FindViewById<RadioButton>(Resource.Id.radioButtonr4);
            r5 = FindViewById<RadioButton>(Resource.Id.radioButtonr5);
            r6 = FindViewById<RadioButton>(Resource.Id.radioButtonr6);
            ewoObj = JsonConvert.DeserializeObject<EWO>(Intent.GetStringExtra("ewoObject"));
            editing = Intent.GetBooleanExtra("edit", false);
            if (string.IsNullOrWhiteSpace(ewoObj.RootCause))
            {
                SetRootCauseUi(r1.Text);
            }
            else
                SetRootCauseUi(ewoObj.RootCause);

            Button nextButton = FindViewById<Button>(Resource.Id.NextButtonRootCause);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipButtonRootCause);
            Button backButton = FindViewById<Button>(Resource.Id.BackButtonRootCause);
            skipButton.Click += SkipButton_Click;
            backButton.Click += BackButton_Click;
            nextButton.Click += NextButton_Click;

            if (editing)
                skipButton.Visibility = ViewStates.Invisible;
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            var OverViewActivity = new Intent(this, typeof(Overview));
            OverViewActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
            StartActivity(OverViewActivity);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            ewoObj.RootCause = GetRootCause();
            try
            {
                if (!editing)
                {
                    var OverViewActivity = new Intent(this, typeof(Overview));
                    OverViewActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                    StartActivity(OverViewActivity);
                }
                else
                {

                    var OverViewActivity = new Intent(this, typeof(Overview));
                    OverViewActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                    StartActivity(OverViewActivity);

                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }
        private string GetRootCause()
        {
            if (r1.Checked) return "External Factors";
            if (r2.Checked) return "Insufficient Maintenance";
            if (r3.Checked) return "Insufficient Skills";
            if (r4.Checked) return "Design Weakness";
            if (r5.Checked) return "Operating Conditions not maintained";
            if (r6.Checked) return "Basic Conditions not maintained";
            return "";
        }

        private void SetRootCauseUi(string rootCause = "")
        {
            switch (rootCause)
            {
                case "External Factors": r1.Checked = true; break;
                case "Insufficient Maintenance": r2.Checked = true; break;
                case "Insufficient Skills": r3.Checked = true; break;
                case "Design Weakness": r4.Checked = true; break;
                case "Operating Conditions not maintained": r5.Checked = true; break;
                case "Basic Conditions not maintained": r6.Checked = true; break;
            }
            /*              android:text="External Factors"
                            android:text="Insufficient Maintenance"
                            android:text="Insufficient Skills"
                            android:text="Design Weakness"
                            android:text="Operating Conditions not maintained"
                            android:text="Basic Conditions not maintained"
*/
        }
    }
}