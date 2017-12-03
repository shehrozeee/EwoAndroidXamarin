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
    [Activity(Label = "Overview", Icon = "@drawable/icon", Theme = "@style/AcquaintTheme")]
    public class Overview : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Overview);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            TableLayout tl = FindViewById<TableLayout>(Resource.Id.tableLayoutInfo);
            TableRow buttonRow = FindViewById<TableRow>(Resource.Id.tableRow12);
            tl.RemoveView(buttonRow);
            tl.AddView(textRow("What"));
            tl.AddView(textRow("When"));
            tl.AddView(textRow("Where"));
            tl.AddView(textRow("Who"));
            tl.AddView(textRow("Which"));
            tl.AddView(textRow("How"));
            tl.AddView(buttonRow);

            Button submitButton = FindViewById<Button>(Resource.Id.SubmitButtonOverView);
            Button saveButton = FindViewById<Button>(Resource.Id.SaveButtonOverView);
            Button backButton = FindViewById<Button>(Resource.Id.BackButtonOverView);
            backButton.Click += BackButton_Click;
            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "Overview";
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private TableRow textRow(string text = "")
        {
            TextView tv = newTextView(text);
            TableRow tr = new TableRow(this);
            tr.AddView(tv);
            return tr;
        }

        private TextView newTextView(string text = "")
        {
            int dpValue = 3; // margin in dips
            float d = this.Resources.DisplayMetrics.Density;
            int margin = (int)(dpValue * d); // margin in pixels
            var tv = new TextView(this) { Text = text };
            var ll = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
            ll.SetMargins(0, margin, 0, margin);
            //tv.LayoutParameters = ll;
            tv.SetPadding(0, margin,0, margin);
            return tv;
        }
        private void NextButton_Click(object sender, EventArgs e)
        {
            //StartActivity(new Intent(this, typeof()));
        }
    }
}