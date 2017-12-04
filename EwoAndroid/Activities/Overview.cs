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
using Android.Support.Design.Widget;
using Newtonsoft.Json;
using Android.Provider;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Graphics;
using Android.Content.PM;
using FFImageLoading.Views;
using System.IO;
using FFImageLoading;
using FFImageLoading.Transformations;
using System.Net;
using EwoAndroid;
namespace EwoAndroid.Activities
{
    [Activity(Label = "Overview", Icon = "@drawable/icon", Theme = "@style/AcquaintTheme")]
    public class Overview : AppCompatActivity
    {
        EWO ewoObj;
        Dictionary<string, View> namedObjects;

        ISharedPreferences shp;
        ISharedPreferencesEditor edtr;
        List<EWO> drafts;
        Button submitButton;
        Button saveButton;
        Button backButton;
        TableRow ButtonRow;

        public static class App
        {
            public static Java.IO.File _file;
            public static Java.IO.File _dir;
            public static Bitmap bitmap;
        }

        public static Android.Net.Uri uri;
        public static bool selected = false;
        public static readonly int PickImageId = 1000;
        public ImageViewAsync _imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ewoObj = JsonConvert.DeserializeObject<EWO>(Intent.GetStringExtra("ewoObject"));

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Overview);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
            TableLayout tl = FindViewById<TableLayout>(Resource.Id.tableLayoutInfo);
            ButtonRow = FindViewById<TableRow>(Resource.Id.tableRow12);
            tl.RemoveView(ButtonRow);
            namedObjects = new Dictionary<string, View>();
            namedObjects.Add("EwoNo"    , FindViewById(Resource.Id.textViewEwo));
            namedObjects.Add("Date"     , FindViewById(Resource.Id.textViewDate));
            namedObjects.Add("Line"     , FindViewById(Resource.Id.textViewLine));
            namedObjects.Add("Shift"    , FindViewById(Resource.Id.textViewShift));
            namedObjects.Add("Machine"  , FindViewById(Resource.Id.textViewMachine));
            foreach (var v in namedObjects.Values)
            {
                v.Click += EditInfo;
            }
            namedObjects.Add("Picture", FindViewById(Resource.Id.ewoPhotoImageView));
            namedObjects.Last().Value.Click += PictureEdit;
            _imageView = FindViewById<ImageViewAsync>(Resource.Id.ewoPhotoImageView);
            namedObjects.Add("Faliure"  , FindViewById(Resource.Id.TextViewFailureDecription));
            namedObjects.Last().Value.Click += FaliureDescription; ;
            try
            {
                _imageView.SetImageResource(Resource.Drawable.img_placeholder);
            }
            catch { }
            if (!string.IsNullOrEmpty(ewoObj.pictureLocalPath))
            {
                
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                
                // Display in ImageView. We will resize the bitmap to fit the display
                // Loading the full sized image will consume to much memory 
                // and cause the application to crash.

                int height = 800;
                int width = 600;
                App.bitmap = ewoObj.pictureLocalPath.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    _imageView.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }
                GC.Collect();
            }
            else if (!string.IsNullOrEmpty(ewoObj.pictureSmallUrl))
            {
                SetImage();
            }


            tl.AddView(textRow("What"));
            namedObjects.Last().Value.Click += WandHedit;
            tl.AddView(textRow("When"));
            namedObjects.Last().Value.Click += WandHedit;
            tl.AddView(textRow("Where"));
            namedObjects.Last().Value.Click += WandHedit;
            tl.AddView(textRow("Who"));
            namedObjects.Last().Value.Click += WandHedit;
            tl.AddView(textRow("Which"));
            namedObjects.Last().Value.Click += WandHedit;
            tl.AddView(textRow("How"));
            namedObjects.Last().Value.Click += WandHedit;

            tl.AddView(textRow("RootCause"));
            namedObjects.Last().Value.Click += RootCauseEdit; ;
            tl.AddView(ButtonRow);
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
            if (string.IsNullOrWhiteSpace(ewoObj.faliureDescription))
            {
                ewoObj.faliureDescription = "";
            }
            if (string.IsNullOrWhiteSpace(ewoObj.RootCause))
            {
                ewoObj.RootCause = "";
            }
            if (String.IsNullOrWhiteSpace(ewoObj.What))
            {
                ewoObj.What ="";
            }
            if (String.IsNullOrWhiteSpace(ewoObj.Who))
            {
                ewoObj.Who = "";
            }
            if (String.IsNullOrWhiteSpace(ewoObj.Which))
            {
                ewoObj.Which = "";
            }
            if (String.IsNullOrWhiteSpace(ewoObj.When))
            {
                ewoObj.When = "";
            }
            if (String.IsNullOrWhiteSpace(ewoObj.Where))
            {
                ewoObj.Where = "";
            }
            if (String.IsNullOrWhiteSpace(ewoObj.How))
            {
                ewoObj.How = "";
            }


            submitButton = FindViewById<Button>(Resource.Id.SubmitButtonOverView);
            saveButton   = FindViewById<Button>(Resource.Id.SaveButtonOverView);
            backButton   = FindViewById<Button>(Resource.Id.BackButtonOverView);
            backButton.Click += BackButton_Click;
            submitButton.Click += SubmitButton_Click;
            saveButton.Click += SaveButton_Click;
            saveButton.Text = "Save";
            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "Overview";
            setupUi();
        }

        private void setupUi()
        {
            (namedObjects["What"  ] as TextView).Text = "What: "+ewoObj.What;
            (namedObjects["When"   ] as TextView).Text = "When: " + ewoObj.When;
            (namedObjects["Where"   ] as TextView).Text = "Where: "+ewoObj.Where;
            (namedObjects["Who"] as TextView).Text = "Who: " + ewoObj.Who;
            (namedObjects["Which"] as TextView).Text = "Which: " + ewoObj.Which;
            (namedObjects["How"] as TextView).Text = "How: " + ewoObj.How;
            (namedObjects["RootCause"] as TextView).Text = "Root Cause: " + ewoObj.RootCause;
            (namedObjects["EwoNo"] as TextView).Text = "Ewo No: "+ewoObj.EwoNo;
            (namedObjects["Date"] as TextView).Text = "Date: "+ewoObj.Date.ToShortDateString();
            (namedObjects["Line"] as TextView).Text = "Line: " + ewoObj.line;
            (namedObjects["Shift"] as TextView).Text = "Shift: " + ewoObj.Shift;
            (namedObjects["Machine"] as TextView).Text = "Machine: " + ewoObj.Machine;
            (namedObjects["Faliure"] as TextView).Text = "Faliure Description:\n" + ewoObj.faliureDescription;
        }

        private void WandHedit(object sender, EventArgs e)
        {
            var WandHEditActivity = new Intent(this, typeof(WandH));
            string j = JsonConvert.SerializeObject(ewoObj);
            j.ToString();
            WandHEditActivity.PutExtra("ewoObject", j);
            WandHEditActivity.PutExtra("edit", true);
            this.StartActivity(WandHEditActivity);
        }

        private void RootCauseEdit(object sender, EventArgs e)
        {
            var RootCauseEditActivity = new Intent(this, typeof(RootCause));
            string j = JsonConvert.SerializeObject(ewoObj);
            j.ToString();
            RootCauseEditActivity.PutExtra("ewoObject", j);
            RootCauseEditActivity.PutExtra("edit", true);
            this.StartActivity(RootCauseEditActivity);
        }

        private void FaliureDescription(object sender, EventArgs e)
        {

            var FaliureDescriptionActivity = new Intent(this, typeof(FaliureDescription));
            string j = JsonConvert.SerializeObject(ewoObj);
            j.ToString();
            FaliureDescriptionActivity.PutExtra("ewoObject", j);
            FaliureDescriptionActivity.PutExtra("edit", true);
            this.StartActivity(FaliureDescriptionActivity);
        }

        private void PictureEdit(object sender, EventArgs e)
        {
            var PictureSelectionActivity = new Intent(this, typeof(PictureSelectActivity));
            string j = JsonConvert.SerializeObject(ewoObj);
            j.ToString();
            PictureSelectionActivity.PutExtra("ewoObject", j);
            PictureSelectionActivity.PutExtra("edit", true);
            this.StartActivity(PictureSelectionActivity);

        }

        private void EditInfo(object sender, EventArgs e)
        {

            var Infoactivity = new Intent(this, typeof(EwoInfo));
            string j = JsonConvert.SerializeObject(ewoObj);
            j.ToString();
            Infoactivity.PutExtra("ewoObject", j);
            Infoactivity.PutExtra("edit",true );
            this.StartActivity(Infoactivity);
        }

        private string generateEwoNum()
        {
            return "EWO_" + DateTime.Now.ToShortDateString().Replace("/", "-") + "_" + DateTime.Now.ToShortTimeString();
        }


        async void SetImage()
        {
            try
            {
                await ImageService
                .Instance
                .LoadUrl(ewoObj.pictureSmallUrl,new TimeSpan(1000,0,0))  // get the image from a URL
                .LoadingPlaceholder("img_placeholder.jpg")                                          // specify a placeholder image
                .Error(e => System.Diagnostics.Debug.WriteLine(e.Message))
                .IntoAsync(_imageView);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                _imageView.SetImageResource(Resource.Drawable.img_placeholder);
            }

        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            shp = Application.Context.GetSharedPreferences("ewosettings", FileCreationMode.Append);
            edtr = shp.Edit();
            if (!shp.Contains("drafts"))
            {
                drafts = new List<EWO>();
                edtr.PutString("drafts", JsonConvert.SerializeObject(drafts));
                edtr.Commit();
            }
            else
            {
                drafts = JsonConvert.DeserializeObject<List<EWO>>(shp.GetString("drafts", JsonConvert.SerializeObject(new List<EWO>())));
            }
            if (drafts.Exists(x => x.id == ewoObj.id))
            {
                drafts.Remove(drafts.Find(x => x.id == ewoObj.id));
            }
            drafts.Insert(0, ewoObj);
            edtr.PutString("drafts", JsonConvert.SerializeObject(drafts));
            edtr.Commit();
            Intent intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTask);
            intent.AddFlags(ActivityFlags.NewTask);
            StartActivity(intent);

        }

        private void uploadEwoObj(object sender)
        {
            if (submit())
            {
                shp = Application.Context.GetSharedPreferences("ewosettings", FileCreationMode.Append);
                edtr = shp.Edit();
                if (!shp.Contains("drafts"))
                {
                    drafts = new List<EWO>();
                    edtr.PutString("drafts", JsonConvert.SerializeObject(drafts));
                    edtr.Commit();
                }
                else
                {
                    drafts = JsonConvert.DeserializeObject<List<EWO>>(shp.GetString("drafts", JsonConvert.SerializeObject(new List<EWO>())));
                }
                if (drafts.Exists(x => x.id == ewoObj.id))
                {
                    drafts.Remove(drafts.Find(x => x.id == ewoObj.id));
                }
                edtr.PutString("drafts", JsonConvert.SerializeObject(drafts));
                edtr.Commit();
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTask);
                intent.AddFlags(ActivityFlags.NewTask);
                StartActivity(intent);
                //ButtonRow.Visibility = ViewStates.Visible;
            }
            else
            {
                ButtonRow.Visibility = ViewStates.Visible;
                Snackbar
               .Make(((Button)sender), "Submission Failed", Snackbar.LengthLong)
               .SetAction("Save", (x) => { FindViewById<Button>(Resource.Id.SaveButtonOverView).CallOnClick(); })
               .Show();
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ButtonRow.Visibility = ViewStates.Gone;
            if (string.IsNullOrWhiteSpace(ewoObj.pictureLocalPath))
            {
                uploadEwoObj(sender);
            }
            else
            {
                uploadAsync();
            }
            
        }
        private bool submit()
        {
            int id = 0;
            if (ewoObj.id < 0)
                id = 0;
            else
                id = ewoObj.id;
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms.Add("ewoObj", JsonConvert.SerializeObject(ewoObj));
            return (HttpPostRequest("setEwoObs/" + id, parms) == "OK");
            
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
            this.Finish();
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
            var ll = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent,  ViewGroup.LayoutParams.FillParent);
            ll.SetMargins(0, margin, 0, margin);
            //tv.LayoutParameters = ll;
            tv.SetPadding(0, margin,0, margin);
            namedObjects.Add(text, tv);
            return tv;
        }
        string serverURL = Globals.serverURL;
        public string HttpPostRequest(string url1, Dictionary<string, string> postParameters)
        {
            //string url = "http://192.168.5.102:9000/" + url1;
            string url = serverURL + ":9080/" + url1;
            string postData = "";
            foreach (string key in postParameters.Keys)
            {
                postData += WebUtility.UrlEncode(key) + "="
                    + WebUtility.UrlEncode(postParameters[key]) + "&";
            }

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.Method = "POST";
            byte[] data = System.Text.Encoding.ASCII.GetBytes(postData);
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.ContentLength = data.Length;
            try
            {
                Stream requestStream = myHttpWebRequest.GetRequestStream();
                requestStream.Write(data, 0, data.Length);
                requestStream.Close();
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(responseStream, System.Text.Encoding.Default);
                string pageContent = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                responseStream.Close();
                myHttpWebResponse.Close();
                Toast.MakeText(Application.ApplicationContext, "Data Sent", ToastLength.Short).Show();
                return pageContent;
            }
            catch
            {
                Toast.MakeText(Application.ApplicationContext, "There was and error submitting the data Please contact your manager or developer.", ToastLength.Short).Show();
                new Android.Support.V7.App.AlertDialog.Builder(this).SetPositiveButton("Okay", (sender, args) => { })
                .SetMessage("There was an error in submiting the record!")
                .SetTitle("Connectivity Error")
                .Show();
            }
            return "";
        }
        
        string SelectedFileName = "";
        private void uploadAsync()
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.UploadProgressChanged += Client_UploadProgressChanged;
                    client.UploadFileCompleted += Client_UploadFileCompleted;
                    if (!string.IsNullOrWhiteSpace(ewoObj.pictureLocalPath))
                    {
                        Toast.MakeText(Application.ApplicationContext, "Compressing Image...", ToastLength.Short).Show();
                        SelectedFileName = BitmapHelpers.ResizeImage(ewoObj.pictureLocalPath);
                        Toast.MakeText(Application.ApplicationContext, "Uploading Image", ToastLength.Short).Show();
                        client.UploadFileAsync(new System.Uri(serverURL + ":9080/upload"), SelectedFileName);
                    }
                    else
                        Client_UploadFileCompleted(null, null);
                }
            }
            catch (Exception ex)
            {
                
                ex.ToString();
                ButtonRow.Visibility = ViewStates.Visible;
                Toast.MakeText(Application.ApplicationContext, "There was an error uploading the image Please contact your manager or developer.", ToastLength.Short).Show();
            }
        }

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            //pb1.Progress = 0;
            if (e != null)
            {
                if (e.Error != null)
                {
                    ButtonRow.Visibility = ViewStates.Visible;
                    Toast.MakeText(Application.ApplicationContext, "There is an issue with the internet connection.", ToastLength.Short).Show();
                    return;
                }
                ewoObj.pictureSmallUrl = serverURL + ":9080/images/" + "300" + SelectedFileName.Split('/').Last();
                ewoObj.pictureUrl = serverURL + ":9080/images/" + SelectedFileName.Split('/').Last();

                uploadEwoObj(submitButton);
            }
            else
            {
                uploadEwoObj(submitButton);
            }
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            //RunOnUiThread(() => { pb1.Progress = e.ProgressPercentage; });
        }

    }
}