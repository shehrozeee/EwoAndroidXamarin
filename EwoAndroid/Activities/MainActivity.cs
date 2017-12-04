using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android.Views;
using FFImageLoading.Views;
using FFImageLoading;
using System;
using FFImageLoading.Transformations;
using Android.Content;
using Android.Support.V4.Widget;
using System.Threading;
using Newtonsoft.Json;
using Android.Graphics;
using System.Linq;
using Android.Util;
using System.Net;

namespace EwoAndroid.Activities
{
    


    [Activity(Label = "EwoAndroid", Icon = "@drawable/icon",Theme = "@style/AcquaintTheme")]
    public class MainActivity : AppCompatActivity
    {
        const string TAG = "MainActivity";
        AcquaintanceCollectionAdapter _Adapter;

        SwipeRefreshLayout _SwipeRefreshLayout;

        ISharedPreferences shp;
        ISharedPreferencesEditor edtr;
        List<EWO> drafts;
        public static bool showDratfs = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.acquaintanceListFloatingActionButton);
            /*Snackbar
            .Make(fab, "Text Here", Snackbar.LengthLong)
            .SetAction("Party", (x) => { })
            .Show();*/
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
            
            // instantiate adapter
            _Adapter = new AcquaintanceCollectionAdapter();

            // instantiate the layout manager
            var layoutManager = new LinearLayoutManager(this);


            // setup the action bar
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));

            // ensure that the system bar color gets drawn
            Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            // set the title of both the activity and the action bar
            Title = SupportActionBar.Title = "EW Observations";

            _SwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.acquaintanceListSwipeRefreshContainer);

            _SwipeRefreshLayout.Refresh += async (sender, e) => { await LoadAcquaintances(); };

            _SwipeRefreshLayout.Post(() => _SwipeRefreshLayout.Refreshing = true);

            // instantiate/inflate the RecyclerView
            var recyclerView = (RecyclerView)FindViewById(Resource.Id.acquaintanceRecyclerView);

            // set RecyclerView's layout manager 
            recyclerView.SetLayoutManager(layoutManager);

            // set RecyclerView's adapter
            recyclerView.SetAdapter(_Adapter);

            var addButton = (FloatingActionButton)FindViewById(Resource.Id.acquaintanceListFloatingActionButton);
            var draftsButton = (FloatingActionButton)FindViewById(Resource.Id.EwoEditFloatingActionButton);
            draftsButton.Click += async (sender, e) =>
             {
                 Toast.MakeText(this, "Switching to " + (!showDratfs ? "Drafts" : "Online") + " records", ToastLength.Long).Show();
                 showDratfs = !showDratfs;
                 await LoadAcquaintances();

             };
            addButton.Click += (sender, e) => {
                var Infoactivity = new Intent(this, typeof(EwoInfo));
                Infoactivity.PutExtra("ewoObject",JsonConvert.SerializeObject(new EWO() { Date = DateTime.Now,id = minId()}));
                StartActivity(Infoactivity);
            };


        }
        public int minId()
        {
            if (drafts != null)
            {
                if (drafts.Count != 0)
                {
                    return drafts.Min(x => x.id)-1;
                }
                return -1;
            }
            else
                return -1;
        }
        protected override async void OnResume()
        {
            //Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);
            base.OnResume();
            await LoadAcquaintances();
            
        }
        async Task LoadAcquaintances()
        {
            _SwipeRefreshLayout.Refreshing = true;
            try
            {
                // load the items
                await _Adapter.LoadAcquaintances();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting acquaintances: {ex.Message}");

                //set alert for executing the task
                var alert = new Android.App.AlertDialog.Builder(this);

                alert.SetTitle("Error getting acquaintances");

                alert.SetMessage("Ensure you have a network connection, and that a valid backend service URL is present in the app settings.");

                alert.SetNegativeButton("OK", (senderAlert, args) => {
                    // an empty delegate body, because we just want to close the dialog and not take any other action
                });

                //run the alert in UI thread to display in the screen
                RunOnUiThread(() => {
                    alert.Show();
                });
            }
            finally
            {
                _SwipeRefreshLayout.Refreshing = false;
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.AcquaintanceListMenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }
        int count = 0;
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item != null)
            {
                switch (item.ItemId)
                {
                    case Resource.Id.settingsButton:
                        count++;
                        if (count == 5)
                        {


                            edtr.Clear();
                            edtr.Commit();
                            StartActivity(typeof(InitialSettings));
                            this.Finish();
                        }
                        else
                        {
                            Toast.MakeText(this, "You are " + (5 - count) + " more taps from clearing your initial settings", ToastLength.Short).Show();
                        }
                        break;
                }
            }

            return base.OnOptionsItemSelected(item);
        }
    

}

    public class EWO
    {
        public int id { get; set; }
        public string RootCause { get; set; }
        public string EwoNo { get; set; }
        public DateTime Date { get; set; }
        public string Shift  { get; set; }
        public string line { get; set; }
        public string Machine { get; set; }
        public string pictureUrl { get; set; }
        public string pictureSmallUrl { get; set; }
        public string pictureLocalPath { get; set; }
        public string faliureDescription { get; set; }
        public string What { get; set; }
        public string When { get; set; }
        public string Where { get; set; }
        public string Who { get; set; }
        public string Which { get; set; }
        public string How { get; set; }


    }

    internal class EwoRowHolder : RecyclerView.ViewHolder
    {
        public View AcquaintanceRow { get; }

        public TextView NameTextView { get; }

        public TextView CompanyTextView { get; }

        public TextView JobTitleTextView { get; }

        public ImageViewAsync ProfilePhotoImageView { get; }

        public EwoRowHolder(View itemView) : base(itemView)
        {
            AcquaintanceRow = itemView;

            NameTextView = AcquaintanceRow.FindViewById<TextView>(Resource.Id.nameTextView);
            CompanyTextView = AcquaintanceRow.FindViewById<TextView>(Resource.Id.companyTextView);
            JobTitleTextView = AcquaintanceRow.FindViewById<TextView>(Resource.Id.jobTitleTextView);
            ProfilePhotoImageView = AcquaintanceRow.FindViewById<ImageViewAsync>(Resource.Id.profilePhotoImageView);
        }
    }

    /// <summary>
    /// Acquaintance collection adapter. Coordinates data the child views of RecyclerView.
    /// </summary>
    internal class AcquaintanceCollectionAdapter : RecyclerView.Adapter, View.IOnClickListener
    {
        //IDataSource<Acquaintance> _DataSource;

        // the list of items that this adapter uses
        public List<EWO> EWOs { get; private set; }

        public AcquaintanceCollectionAdapter()
        {
            EWOs = new List<EWO>();

            SetDataSource();
        }

        void SetDataSource()
        {
            //_DataSource = ServiceLocator.Current.GetInstance<IDataSource<Acquaintance>>();
        }


        public void ExpensiveTask()
        {
            try
            {
                string j = HttpGetRequest("getEwoObs/40");
                EWOs = JsonConvert.DeserializeObject<List<EWO>>(j);
            }
            catch(Exception ex)
            {
                ex.ToString();
            }
        }
        string serverURL = Globals.serverURL; 
        public string HttpGetRequest(string Url)
        {
            string Out = String.Empty;
            Url = serverURL + ":9080/" + Url;
            System.Net.WebRequest req = System.Net.WebRequest.Create(Url);
            try
            {
                System.Net.WebResponse resp = req.GetResponse();
                using (System.IO.Stream stream = resp.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(stream))
                    {
                        Out = sr.ReadToEnd();
                        sr.Close();
                    }
                }
            }
            catch (ArgumentException ex)
            {
                Toast.MakeText(Application.Context, "There is an issue with the internet connection.", ToastLength.Short).Show();
                Out = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
            }
            catch (WebException ex)
            {
                Toast.MakeText(Application.Context, "There is an issue with the internet connection.", ToastLength.Short).Show();
                Out = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, "There is an issue with the internet connection.", ToastLength.Short).Show();
                Out = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
            }

            return Out;
        }
        public Task ExpensiveTaskAsync()
        {
            return Task.Run(() => ExpensiveTask());
        }

        /// <summary>
        /// Loads the acquaintances.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task LoadAcquaintances()
        {
            SetDataSource();
            if (!MainActivity.showDratfs)
            {
                EWOs.Clear();
                await ExpensiveTaskAsync();
                /*//Acquaintances = (await _DataSource.GetItems()).ToList();
                EWOs.Add(new EWO() { pictureSmallUrl = "http://unilever.innidata.com:9000/images/Cover_.jpg", Date = DateTime.Now, Machine = "Mixer", line = "2", Shift = "A" });
                EWOs.Add(new EWO() { pictureSmallUrl = "http://unilever.innidata.com:9000/images/edited_20160825_113830_.jpg", Date = DateTime.Now + new TimeSpan(1, 2, 1), Machine = "Noodler", line = "3", Shift = "A" });
                EWOs.Add(new EWO() { pictureSmallUrl = "http://unilever.innidata.com:9000/images/edited_20160826_115536_.jpg", Date = DateTime.Now + new TimeSpan(1, 1, 1, 1, 1), Machine = "Mixer", line = "4", Shift = "B" });
                EWOs.Add(new EWO() { pictureSmallUrl = "http://unilever.innidata.com:9000/images/edited_20160925_200250_.jpg", Date = DateTime.Now + new TimeSpan(2, 1, 1), Machine = "Roll Mill", line = "5", Shift = "A" });
                EWOs.Add(new EWO() { pictureSmallUrl = "http://unilever.innidata.com:9000/images/FateZero-Blu-rayBox1-01_.jpg", Date = DateTime.Now + new TimeSpan(1, 1, 1), Machine = "Plodder", line = "6", Shift = "C" });
                EWOs.Add(new EWO() { pictureSmallUrl = "http://unilever.innidata.com:9000/images/FB_IMG_1474133525512_.jpg", Date = DateTime.Now + new TimeSpan(3, 1, 1), Machine = "Stamper", line = "1", Shift = "A" });
                */
            }
            else
            {
                ISharedPreferences shp;
                ISharedPreferencesEditor edtr;
                shp = Application.Context.GetSharedPreferences("ewosettings", FileCreationMode.Append);
                EWOs.Clear();
                try
                {
                    EWOs = JsonConvert.DeserializeObject<List<EWO>>(shp.GetString("drafts", JsonConvert.SerializeObject(new List<EWO>())));
                }
                catch
                {

                }
            }


            NotifyDataSetChanged();

            //Settings.ClearImageCacheIsRequested = false;
        }

        // when a RecyclerView itemView is requested, the OnCreateViewHolder() is called
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // instantiate/inflate a view
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.EwoRow, parent, false);

            // Create a ViewHolder to find and hold these view references, and 
            // register OnClick with the view holder:
            var viewHolder = new EwoRowHolder(itemView);

            return viewHolder;
        }

        // populates the properties of the child views of the itemView
        public override async void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            // instantiate a new view holder
            var viewHolder = holder as EwoRowHolder;

            // get an item by position (index)
            var ewo = EWOs[position];

            // assign values to the views' text properties
            if (viewHolder == null) return;

            viewHolder.NameTextView.Text = ewo.Date.ToShortDateString();
            viewHolder.CompanyTextView.Text = "Machine: "+ewo.Machine;
            viewHolder.JobTitleTextView.Text = "Line: "+ewo.line + "\t Ewo No: "+ewo.EwoNo;

            // set the Tag property of the AcquaintanceRow view to the position (index) of the item that is currently being bound. We'll need it later in the OnLick() implementation.
            viewHolder.AcquaintanceRow.Tag = position;

            // set OnClickListener of AcquaintanceRow
            viewHolder.AcquaintanceRow.SetOnClickListener(this);
            if (!string.IsNullOrEmpty(ewo.pictureLocalPath))
            {
                //selected = true;
                //FileSelect = false;
                // Make it available in the gallery


                // Display in ImageView. We will resize the bitmap to fit the display
                // Loading the full sized image will consume to much memory 
                // and cause the application to crash.
                try
                {
                    await ImageService
                    .Instance
                    .LoadFile(ewo.pictureLocalPath)
                    .LoadingPlaceholder("img_placeholder.jpg")                                          // specify a placeholder image
                    .Transform(new CircleTransformation())                                                      // transform the image to a circle
                    .Error(e => System.Diagnostics.Debug.WriteLine(e.Message))
                    .IntoAsync(viewHolder.ProfilePhotoImageView);
                }
                catch
                {
                    viewHolder.ProfilePhotoImageView.SetImageResource(Resource.Drawable.img_placeholder);
                }
            }            
            else if (string.IsNullOrWhiteSpace(ewo.pictureSmallUrl))
                viewHolder.ProfilePhotoImageView.SetImageResource(Resource.Drawable.img_placeholder);
            else
                // use FFImageLoading library to asynchronously:
                try
                {
                    await ImageService
                    .Instance
                    .LoadUrl(ewo.pictureSmallUrl, TimeSpan.FromHours(1))  // get the image from a URL
                    .LoadingPlaceholder("img_placeholder.jpg")                                          // specify a placeholder image
                    .Transform(new CircleTransformation())                                                      // transform the image to a circle
                    .Error(e => System.Diagnostics.Debug.WriteLine(e.Message))
                    .IntoAsync(viewHolder.ProfilePhotoImageView);
                }
                catch
                {
                    viewHolder.ProfilePhotoImageView.SetImageResource(Resource.Drawable.img_placeholder);
                }
        }

        public void OnClick(View view)
        {
            // setup an intent
            //var detailIntent = new Intent(view.Context, typeof(AcquaintanceDetailActivity));

            // get an item by position (index)
            var ewoObj = EWOs[(int)view.Tag];

            // Add some identifying item data to the intent. In this case, the id of the ewo for which we're about to display the detail screen.
            //detailIntent.PutExtra(view.Context.Resources.GetString(Resource.String.acquaintanceDetailIntentKey), acquaintance.Id);

            // get a referecne to the profileImageView
            var profileImageView = view.FindViewById(Resource.Id.profilePhotoImageView);

            // shared element transitions are only supported on Android 5.0+
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                // define transitions 
                var transitions = new List<Android.Util.Pair>() {
                    Android.Util.Pair.Create(profileImageView, view.Context.Resources.GetString(Resource.String.profilePhotoTransition)),
                };

                // create an activity options instance and bind the above-defined transitions to the current activity
                var transistionOptions = ActivityOptions.MakeSceneTransitionAnimation(view.Context as Activity, transitions.ToArray());

                // start (navigate to) the detail activity, passing in the activity transition options we just created
                var Infoactivity = new Intent(view.Context, typeof(EwoInfo));
                Infoactivity.PutExtra("ewoObject", JsonConvert.SerializeObject(ewoObj));
                view.Context.StartActivity(Infoactivity);

                //view.Context.StartActivity(Infoactivity, transistionOptions.ToBundle());

            }
            else
            {
                var Infoactivity = new Intent(view.Context, typeof(EwoInfo));
                string j = JsonConvert.SerializeObject(ewoObj);
                j.ToString();
                Infoactivity.PutExtra("ewoObject",j);

                view.Context.StartActivity(Infoactivity);
            }
        }

        // Return the number of items
        public override int ItemCount => EWOs.Count;
    }
}

