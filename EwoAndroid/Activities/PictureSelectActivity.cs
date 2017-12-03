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
using Android.Provider;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Graphics;
using Android.Content.PM;
using FFImageLoading.Views;
using System.IO;
using FFImageLoading;
using FFImageLoading.Transformations;

namespace EwoAndroid.Activities
{
    public static class BitmapHelpers
    {
        public static string ResizeImage(string sourceFilePath)
        {


            string newPath = sourceFilePath.Replace(".jpg", "_.jpg");
            newPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), System.IO.Path.GetFileName(newPath));
            using (Android.Graphics.Bitmap bmp = Android.Graphics.BitmapFactory.DecodeFile(sourceFilePath))
            {

                using (var fs = new FileStream(newPath, FileMode.OpenOrCreate))
                {
                    bmp.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 20, fs);
                }
            }
            GC.Collect();
            return newPath;
        }
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }
    }

    [Activity(Label = "EwoInfo", Icon = "@drawable/icon", Theme = "@style/AcquaintTheme")]
    public class PictureSelectActivity : AppCompatActivity
    {

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

        private EWO ewoObj;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.PictureSelection);
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "Picture Selection";
            _imageView = FindViewById<ImageViewAsync>(Resource.Id.imageView1);
            ewoObj = JsonConvert.DeserializeObject<EWO>(Intent.GetStringExtra("ewoObject"));

            if (!string.IsNullOrEmpty(ewoObj.pictureLocalPath))
            {
                selected = true;
                FileSelect = false;
                // Make it available in the gallery

                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(App._file);
                SelectedFileName = App._file.Name;
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                // Display in ImageView. We will resize the bitmap to fit the display
                // Loading the full sized image will consume to much memory 
                // and cause the application to crash.

                int height = 800;
                int width = 600;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
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
            Button openGalleryButton = FindViewById<Button>(Resource.Id.PictureSelectCamearaButton);
            Button openCameraButton = FindViewById<Button>(Resource.Id.PictureSelectGalleryButton);
            Button openLedgerButton = FindViewById<Button>(Resource.Id.PictureSelectLegerButton);


            Button nextButton = FindViewById<Button>(Resource.Id.NextButtonPictureSelect);
            Button skipButton = FindViewById<Button>(Resource.Id.SkipButtonPictureSelect);
            Button backButton = FindViewById<Button>(Resource.Id.BackButtonPictureSelect);
            skipButton.Click += SkipButton_Click;
            backButton.Click += BackButton_Click;
            nextButton.Click += NextButton_Click;
            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                openCameraButton.Click += TakeAPicture;
                openGalleryButton.Click += ButtonOnClick;
            }
        }
        async void SetImage()
        {
            try
            {
                await ImageService
                .Instance
                .LoadUrl(ewoObj.pictureSmallUrl, TimeSpan.FromHours(1000))  // get the image from a URL
                .LoadingPlaceholder("img_placeholder.jpg")                                          // specify a placeholder image
                .Error(e => System.Diagnostics.Debug.WriteLine(e.Message))
                .IntoAsync(_imageView);
            }
            catch
            {
                _imageView.SetImageResource(Resource.Drawable.img_placeholder);
            }

        }
        private void SkipButton_Click(object sender, EventArgs e)
        {

            var FaliureDescriptionActivity = new Intent(this, typeof(FaliureDescription));
            FaliureDescriptionActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
            StartActivity(FaliureDescriptionActivity);

        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            
            var FaliureDescriptionActivity = new Intent(this, typeof(FaliureDescription));
            FaliureDescriptionActivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
            StartActivity(FaliureDescriptionActivity);
        }

        private string GetPathToImage(Android.Net.Uri uri)
        {
            string doc_id = "";
            using (var c1 = ContentResolver.Query(uri, null, null, null, null))
            {
                c1.MoveToFirst();
                String document_id = c1.GetString(0);
                doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
            }

            string path = null;

            // The projection contains the columns we want to return in our query.
            string selection = MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null))
            {
                if (cursor == null) return path;
                var columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                path = cursor.GetString(columnIndex);
            }
            return path;
        }

        bool FileSelect = false;
        private void CreateDirectoryForPictures()
        {
            App._dir = new Java.IO.File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "UniliverEO");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }
        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("UEOB_{0}.jpg", DateTime.Now.ToString("ddMMyyyyHHmmss")));

            SelectedFileName = App._file.Name;
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            Intent = new Intent();
            Intent.SetType("image/*");
            Intent.SetAction(Intent.ActionGetContent);
            StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId + 1);
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Canceled)
                return;
            if ((requestCode == PickImageId + 1) && (resultCode == Result.Ok) && (data != null))
            {
                uri = data.Data;
                _imageView.SetImageURI(uri);
                selected = true;
                FileSelect = true;
            }
            else
            {
                selected = true;
                FileSelect = false;
                // Make it available in the gallery

                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(App._file);
                SelectedFileName = App._file.Name;
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                // Display in ImageView. We will resize the bitmap to fit the display
                // Loading the full sized image will consume to much memory 
                // and cause the application to crash.

                int height = 800;
                int width = 600;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
                if (App.bitmap != null)
                {
                    _imageView.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }
                GC.Collect();
            }
            // Dispose of the Java side bitmap.
        }
        string SelectedFileName = "";

    }
}