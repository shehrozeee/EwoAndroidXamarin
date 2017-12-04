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
    [Activity(Label = "EwoInfo", Theme = "@style/AcquaintTheme")]
    public class EwoInfo : AppCompatActivity
    {
        bool editing = false;
        private DatePicker infoDatePicker;
        private EditText infoEwoNum;
        private RadioButton infoRadioButtonA;
        private RadioButton infoRadioButtonB;
        private RadioButton infoRadioButtonC;
        private RadioButton infoRadioButton1;
        private RadioButton infoRadioButton2;
        private RadioButton infoRadioButton3;
        private RadioButton infoRadioButton4;
        private RadioButton infoRadioButton5;
        private RadioButton infoRadioButton6;
        private RadioButton infoRadioButtonm1;
        private RadioButton infoRadioButtonm2;
        private RadioButton infoRadioButtonm3;
        private RadioButton infoRadioButtonm4;
        private RadioButton infoRadioButtonm5;
        private RadioButton infoRadioButtonm6;
        private EWO ewoObj;
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

            infoRadioButtonA = FindViewById<RadioButton>(Resource.Id.radioButtonA);
            infoRadioButtonB = FindViewById<RadioButton>(Resource.Id.radioButtonB);
            infoRadioButtonC = FindViewById<RadioButton>(Resource.Id.radioButtonC);
            infoRadioButton1 = FindViewById<RadioButton>(Resource.Id.radioButtonl1);
            infoRadioButton2 = FindViewById<RadioButton>(Resource.Id.radioButtonl2);
            infoRadioButton3 = FindViewById<RadioButton>(Resource.Id.radioButtonl3);
            infoRadioButton4 = FindViewById<RadioButton>(Resource.Id.radioButtonl4);
            infoRadioButton5 = FindViewById<RadioButton>(Resource.Id.radioButtonl5);
            infoRadioButton6 = FindViewById<RadioButton>(Resource.Id.radioButtonl6);
            infoRadioButtonm1= FindViewById<RadioButton>(Resource.Id.radioButtonm1);
            infoRadioButtonm2= FindViewById<RadioButton>(Resource.Id.radioButtonm2);
            infoRadioButtonm3= FindViewById<RadioButton>(Resource.Id.radioButtonm3);
            infoRadioButtonm4= FindViewById<RadioButton>(Resource.Id.radioButtonm4);
            infoRadioButtonm5= FindViewById<RadioButton>(Resource.Id.radioButtonm5);
            infoRadioButtonm6 = FindViewById<RadioButton>(Resource.Id.radioButtonm6);

            string j = Intent.GetStringExtra("ewoObject");
            j.ToString();
            ewoObj = JsonConvert.DeserializeObject<EWO>(Intent.GetStringExtra("ewoObject"));

            editing =  Intent.GetBooleanExtra("edit", false);
            if (string.IsNullOrWhiteSpace(ewoObj.EwoNo))
            {
                ewoObj.EwoNo = generateEwoNum();
            }
            if (string.IsNullOrWhiteSpace(ewoObj.Shift))
            {
                ewoObj.Shift = "A";
            }
            if (string.IsNullOrWhiteSpace(ewoObj.line))
            {
                ewoObj.line = "1";
            }
            if (string.IsNullOrWhiteSpace(ewoObj.Machine))
            {
                ewoObj.Machine = "Mixer";
            }
            SetShiftUi(ewoObj.Shift);
            SetLineUi(ewoObj.line);
            SetMachineUi(ewoObj.Machine);

            var ewoNumEditText = FindViewById<EditText>(Resource.Id.InfoEwoNumberEditText);
            var ewoDateEdit = FindViewById<DatePicker>(Resource.Id.InfoDateEditText);
            ewoDateEdit.DateTime = ewoObj.Date;
            ewoNumEditText.Text = ewoObj.EwoNo;
            Button nextButton = FindViewById<Button>(Resource.Id.NextButtonInfo);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipButtonInfo);
            Button backButton = FindViewById<Button>(Resource.Id.BackButtonInfo);
            skipButton.Click += SkipButton_Click;
            nextButton.Click += NextButton_Click;
            backButton.Click += BackButton_Click;
            if (editing)
                skipButton.Visibility = ViewStates.Invisible;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {


            ewoObj.Shift = GetShift();
            ewoObj.line = GetLine();
            ewoObj.Machine = GetMachine();
            var ewoNumEditText = FindViewById<EditText>(Resource.Id.InfoEwoNumberEditText);
            var ewoDateEdit = FindViewById<DatePicker>(Resource.Id.InfoDateEditText);
            ewoObj.Date = ewoDateEdit.DateTime;
            ewoObj.EwoNo = ewoNumEditText.Text;
            if (!editing)
            {
                var PictureSelectionActivity = new Intent(this, typeof(PictureSelectActivity));
                PictureSelectionActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                StartActivity(PictureSelectionActivity);
            }
            else
            {
                
                var OverViewActivity= new Intent(this, typeof(Overview));
                OverViewActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                StartActivity(OverViewActivity);

            }
        }
        private void SkipButton_Click(object sender, EventArgs e)
        {
            var PictureSelectionActivity = new Intent(this, typeof(PictureSelectActivity));
            PictureSelectionActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
            StartActivity(PictureSelectionActivity);
        }

        enum RadioType {shift,line,machine,all }
        private void clearRadioButtons(RadioType group= RadioType.all)
        {
            if (group == RadioType.all)
            {
                infoRadioButtonA.Checked = false;
                infoRadioButtonB.Checked = false;
                infoRadioButtonC.Checked = false;
                infoRadioButton1.Checked = false;
                infoRadioButton2.Checked = false;
                infoRadioButton3.Checked = false;
                infoRadioButton4.Checked = false;
                infoRadioButton5.Checked = false;
                infoRadioButton6.Checked = false;
                infoRadioButtonm1.Checked = false;
                infoRadioButtonm2.Checked = false;
                infoRadioButtonm3.Checked = false;
                infoRadioButtonm4.Checked = false;
                infoRadioButtonm5.Checked = false;
                infoRadioButtonm6.Checked = false;
                return;
            }
            if (group == RadioType.shift)
            {
                infoRadioButtonA.Checked = false;
                infoRadioButtonB.Checked = false;
                infoRadioButtonC.Checked = false;
                return;
            }
            if (group == RadioType.machine)
            {
                infoRadioButtonm1.Checked = false;
                infoRadioButtonm2.Checked = false;
                infoRadioButtonm3.Checked = false;
                infoRadioButtonm4.Checked = false;
                infoRadioButtonm5.Checked = false;
                infoRadioButtonm6.Checked = false;
                return;
            }
            if (group == RadioType.line)
            {
                infoRadioButton1.Checked = false;
                infoRadioButton2.Checked = false;
                infoRadioButton3.Checked = false;
                infoRadioButton4.Checked = false;
                infoRadioButton5.Checked = false;
                infoRadioButton6.Checked = false;
                return;
            }
            

        }
        private void SetShiftUi(string shift)
        {
            //clearRadioButtons(RadioType.shift);
            switch (shift)
            {
                case "A": infoRadioButtonA.Checked = true; break;
                case "B": infoRadioButtonB.Checked = true; break;
                case "C": infoRadioButtonC.Checked = true; break;
            }
        }
        private void SetLineUi(string line)
        {
            //clearRadioButtons(RadioType.line);
            switch (line)
            {
                case "1": infoRadioButton1.Checked = true; break;
                case "2": infoRadioButton2.Checked = true; break;
                case "3": infoRadioButton3.Checked = true; break;
                case "4": infoRadioButton4.Checked = true; break;
                case "5": infoRadioButton5.Checked = true; break;
                case "6": infoRadioButton6.Checked = true; break;
            }
        }
        private void SetMachineUi(string machine)
        {
            //clearRadioButtons(RadioType.machine);
            switch (machine)
            {
                case "Mixer": infoRadioButtonm1.Checked = true; break;
                case "Noodler": infoRadioButtonm2.Checked = true; break;
                case "Rool Mill": infoRadioButtonm3.Checked = true; break;
                case "Plodder": infoRadioButtonm4.Checked = true; break;
                case "Stamper": infoRadioButtonm5.Checked = true; break;
                case "Wrapping": infoRadioButtonm6.Checked = true; break;
            }
        }
        private string GetMachine()
        {
            if (infoRadioButtonm1.Checked) return "Mixer";
            if (infoRadioButtonm2.Checked) return "Noodler";
            if (infoRadioButtonm3.Checked) return "Rool Mill";
            if (infoRadioButtonm4.Checked) return "Plodder";
            if (infoRadioButtonm5.Checked) return "Stamper";
            if (infoRadioButtonm6.Checked) return "Wrapping";
            return "";
        }
        private string GetLine()
        {
            if (infoRadioButton1.Checked) return "1";
            if (infoRadioButton2.Checked) return "2";
            if (infoRadioButton3.Checked) return "3";
            if (infoRadioButton4.Checked) return "4";
            if (infoRadioButton5.Checked) return "5";
            if (infoRadioButton6.Checked) return "6";
            return "";
        }
        private string GetShift()
        {
            if (infoRadioButtonA.Checked) return "A";
            if (infoRadioButtonB.Checked) return "B";
            if (infoRadioButtonC.Checked) return "C";
            return "";
        }
        private string generateEwoNum()
        {
            return "EWO_" + DateTime.Now.ToShortDateString().Replace("/", "-")+"_"+DateTime.Now.ToShortTimeString();
        }
    }
}