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
    public class WandH : AppCompatActivity
    {
        EWO ewoObj;
        EditText WhoEditText;
        EditText WhenEditText;
        EditText WhatEditText;
        EditText WhereEditText;
        EditText WhichEditText;
        EditText HowEditText;
        bool editing = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.FiveWandH);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "5W+1H";
            ewoObj = JsonConvert.DeserializeObject<EWO>(Intent.GetStringExtra("ewoObject"));
            WhoEditText = FindViewById<EditText>(Resource.Id.WhoEditText);
            WhenEditText = FindViewById<EditText>(Resource.Id.WhenEditText);
            WhatEditText = FindViewById<EditText>(Resource.Id.WhatEditText);
            WhereEditText = FindViewById<EditText>(Resource.Id.WhereEditText);
            WhichEditText = FindViewById<EditText>(Resource.Id.WhichEditText);
            HowEditText = FindViewById<EditText>(Resource.Id.HowEditText);
            editing = Intent.GetBooleanExtra("edit", false);
            if (!String.IsNullOrWhiteSpace(ewoObj.What))
            {
                WhatEditText.Text = ewoObj.What;
            }
            if (!String.IsNullOrWhiteSpace(ewoObj.Who))
            {
                WhoEditText.Text = ewoObj.Who;
            }
            if (!String.IsNullOrWhiteSpace(ewoObj.Which))
            {
                WhichEditText.Text = ewoObj.Which;
            }
            if (!String.IsNullOrWhiteSpace(ewoObj.When))
            {
                WhenEditText.Text = ewoObj.When;
            }
            if (!String.IsNullOrWhiteSpace(ewoObj.Where))
            {
                WhereEditText.Text = ewoObj.Where;
            }
            if (!String.IsNullOrWhiteSpace(ewoObj.How))
            {
                HowEditText.Text = ewoObj.How;
            }

            Button nextButton = FindViewById<Button>(Resource.Id.Next5WHDescription);
            Button skipButton = FindViewById<Button>(Resource.Id.Skip5WHDescription);
            Button backButton = FindViewById<Button>(Resource.Id.Back5WHDescription);
            backButton.Click += BackButton_Click;
            nextButton.Click += NextButton_Click;
            skipButton.Click += SkipButton_Click;
            if (editing)
                skipButton.Visibility = ViewStates.Invisible;
        }

        private void SkipButton_Click(object sender, EventArgs e)
        {
            var RootCauseActivity = new Intent(this, typeof(RootCause));
            RootCauseActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
            StartActivity(RootCauseActivity);
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {

            ewoObj.Who = WhoEditText.Text;
            ewoObj.When = WhenEditText.Text;
            ewoObj.What = WhatEditText.Text;
            ewoObj.Where = WhereEditText.Text;
            ewoObj.Which = WhichEditText.Text;
            ewoObj.How = HowEditText.Text;
            if (!editing)
            {
                var RootCauseActivity = new Intent(this, typeof(RootCause));
                RootCauseActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                StartActivity(RootCauseActivity);
            }
            else
            {

                var OverViewActivity = new Intent(this, typeof(Overview));
                OverViewActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                StartActivity(OverViewActivity);

            }
        }
    }
}