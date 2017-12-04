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
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Security;
using System.Security.Cryptography;

namespace EwoAndroid.Activities
{
    [Activity(Label = "EWO Manager RYK", MainLauncher = true, Icon = "@drawable/icon", NoHistory = true, Theme = "@android:style/Theme.Holo.Light")]
    public class InitialSettings : Activity
    {
        string serverURL = Globals.serverURL;
        Button b1;
        Button b3;
        TextView userName;
        TextView tokenID;
        TextView userDept;
        EditText phoneText;
        EditText valText;
        EditText emailText;
        Button b2;
        ISharedPreferences shp;
        ISharedPreferencesEditor edtr;

        string key = "-----BEGIN PUBLIC KEY-----\r\n"
            + "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCybRw/pKNO/werBoqnauzAns63KgS2z2weyFJSM6RKOga08MH54+"
            + "TVKRn2MEI4uC8qmLkHtZoRuJQLhaNfYj1Dp0U7sT3f6ZuDp5iL64N1go0cCPv/q1Gpd5ODJ4ElMq280v6/0qdEjb5V"
            + "UJSxCRj9W9iHe/D+SESANYkkM+Y6NQIDAQAB"
            + "\r\n-----END PUBLIC KEY-----";
        RSACryptoServiceProvider rsaProvider;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            rsaProvider = PemKeyUtils.GetRSAProviderFromPemString(key);
            base.OnCreate(savedInstanceState);

            shp = Application.Context.GetSharedPreferences("ewosettings", FileCreationMode.Append);
            edtr = shp.Edit();
            if (!shp.Contains("Area"))
            {
                edtr.PutString("Area", "");
                edtr.Commit();
            }
            if (!shp.Contains("name"))
            {
                edtr.PutString("name", "invalid");
                edtr.Commit();
            }
            if (!shp.Contains("token"))
            {
                edtr.PutString("token", "invalid");
                edtr.Commit();
            }
            if (!shp.Contains("record_id"))
            {
                edtr.PutString("record_id", "not_found");
                edtr.Commit();
            }
            if (shp.GetString("record_id", "not_found") != "not_found")
            {
                StartActivity(typeof(MainActivity));
            }
            SetContentView(Resource.Layout.InitailSettings);
            tv1 = FindViewById<TextView>(Resource.Id.textView1);
            ed1 = FindViewById<EditText>(Resource.Id.tokenEdit);
            phoneText = FindViewById<EditText>(Resource.Id.phoneEdit);
            valText = FindViewById<EditText>(Resource.Id.validEditText);
            emailText = FindViewById<EditText>(Resource.Id.emailEdit);
            tokenID = FindViewById<TextView>(Resource.Id.tokenID);
            tv2 = FindViewById<TextView>(Resource.Id.textView2);
            tv4 = FindViewById<TextView>(Resource.Id.textView4);
            tv5 = FindViewById<TextView>(Resource.Id.textView5);
            tv6 = FindViewById<TextView>(Resource.Id.textView6);
            tv7 = FindViewById<TextView>(Resource.Id.textView7);
            tv8 = FindViewById<TextView>(Resource.Id.textView8);
            userName = FindViewById<TextView>(Resource.Id.userNameText);
            userDept = FindViewById<TextView>(Resource.Id.depText);


            b1 = FindViewById<Button>(Resource.Id.button1);
            b3 = FindViewById<Button>(Resource.Id.button3);
            b2 = FindViewById<Button>(Resource.Id.button2);
            b3.Click += requestValidation;
            b2.Click += B2_Click;
            b1.Click += B1_Click;
            switchUI(true);
            // Create your application here
        }

        private void requestValidation(object sender, EventArgs e)
        {
            Dictionary<string, string> postParameters = new Dictionary<string, string>();
            postParameters.Add("token", ed1.Text.ToLower().Trim());
            postParameters.Add("number", Convert.ToBase64String(rsaProvider.Encrypt(Encoding.UTF8.GetBytes(phoneText.Text.ToLower().Trim()), true)));
            try
            {
                string response = HttpPostRequest("getrequestValCodeAssigned", postParameters);
                if (String.IsNullOrWhiteSpace(response))
                {
                    Toast.MakeText(Application.ApplicationContext, "There is an issue with the internet connection.", ToastLength.Short).Show();
                    return;
                }
                if (response == "0")
                {
                    Toast.MakeText(Application.ApplicationContext, "Invalid verification Code.", ToastLength.Short).Show();
                    return;
                }
                if (response == "OK")
                {
                    Toast.MakeText(Application.ApplicationContext, "Validation code request sent\r\nyou should get an sms in around 60 sec\r\n", ToastLength.Short).Show();
                }

            }
            catch
            {

            }
        }

        public void switchUI(bool flag)
        {
            if (flag)
            {
                tv2.Visibility = tv4.Visibility = tv5.Visibility = tv6.Visibility = tv7.Visibility = tv8.Visibility = emailText.Visibility = valText.Visibility = phoneText.Visibility = userName.Visibility
                      = userDept.Visibility = tokenID.Visibility = b2.Visibility = b3.Visibility = ViewStates.Gone;
                tv1.Visibility = ed1.Visibility = b1.Visibility = ViewStates.Visible;
            }
            else
            {
                tv2.Visibility = tv4.Visibility = tv5.Visibility = tv6.Visibility = tv7.Visibility = tv8.Visibility = emailText.Visibility = valText.Visibility = phoneText.Visibility = userName.Visibility
                  = userDept.Visibility = tokenID.Visibility = b2.Visibility = b3.Visibility = ViewStates.Visible;

                tv1.Visibility = ed1.Visibility = b1.Visibility = ViewStates.Gone;

            }
        }
        private void B2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> postParameters = new Dictionary<string, string>();
            postParameters.Add("token", ed1.Text.ToLower().Trim());
            postParameters.Add("val", valText.Text.ToLower().Trim());
            postParameters.Add("email", Convert.ToBase64String(rsaProvider.Encrypt(Encoding.UTF8.GetBytes(emailText.Text.ToLower().Trim()), true)));
            postParameters.Add("phone", Convert.ToBase64String(rsaProvider.Encrypt(Encoding.UTF8.GetBytes(phoneText.Text.ToLower().Trim()), true)));
            try
            {
                string response = HttpPostRequest("updtUser", postParameters);
                if (String.IsNullOrWhiteSpace(response))
                {
                    Toast.MakeText(Application.ApplicationContext, "There is an issue with the internet connection.", ToastLength.Short).Show();
                    return;
                }
                if (response == "0")
                {
                    Toast.MakeText(Application.ApplicationContext, "Invalid verification Code.", ToastLength.Short).Show();
                    return;
                }
                edtr.PutString("token", ed1.Text.ToLower().Trim());
                edtr.Commit();
                edtr.PutString("record_id", recordID);
                edtr.Commit();
                edtr.PutString("name", userName.Text);
                edtr.Commit();
                if (IsPlayServicesAvailable())
                {
                    // Start the registration intent service; try to get a token:
                    //var intent = new Intent(this, typeof(RegistrationIntentService));
                    //StartService(intent);
                }
                StartActivity(typeof(MainActivity));
            }
            catch
            {

            }
        }

        TextView tv1;
        EditText ed1;
        TextView tv2;
        TextView tv4;
        TextView tv5;
        TextView tv6;
        TextView tv7;
        TextView tv8;

        private void B1_Click(object sender, EventArgs e)
        {

            //1511515
            try
            {
                updateInfo(ed1.Text.ToLower().Trim());
            }
            catch
            {
                Toast.MakeText(Application.ApplicationContext, "Invalid Token.", ToastLength.Short).Show();
                return;
            }

            if (ed1.Text.ToLower().Trim() == "visitor")
            {
                edtr.PutString("token", "visitor");
                edtr.Commit();
                edtr.PutString("record_id", recordID);
                edtr.Commit();
                edtr.PutString("name", "visitor");
                edtr.Commit();
                StartActivity(typeof(MainActivity));
            }
            else
            {
                switchUI(false);
            }

        }
        public bool IsPlayServicesAvailable()
        {
            return true;/*
            // These methods are moving to GoogleApiAvailability soon:
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                // Google Play Service check failed - display the error to the user:
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    // Give the user a chance to download the APK:
                }
                else
                {
                    Finish();
                }
                return false;
            }
            else
            {
                return true;
            }*/
        }

        string recordID;

        private void updateInfo(string tokenNumber)
        {
            string json = HttpGetRequest("getUserInfo/" + tokenNumber);
            object o = JsonConvert.DeserializeObject(json);
            Newtonsoft.Json.Linq.JArray ar = o as Newtonsoft.Json.Linq.JArray;
            tokenID.Text = tokenNumber;
            userName.Text = ar[0].ToString();
            userDept.Text = ar[1].ToString();
            recordID = ar[2].ToString();
        }

        //HttpGetRequest("getAssigned/2")
        //HttpGetRequest("getUsers")
        //HttpGetRequest("getareas")
        //HttpGetRequest("getUserStats/2")

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
                new AlertDialog.Builder(this).SetPositiveButton("Okay", (sender, args) => { })
                .SetMessage("There was an error in submiting the record!")
                .SetTitle("Connectivity Error")
                .Show();
            }
            return "";
        }

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
                Toast.MakeText(Application.ApplicationContext, "There is an issue with the internet connection.", ToastLength.Short).Show();
                Out = string.Format("HTTP_ERROR :: The second HttpWebRequest object has raised an Argument Exception as 'Connection' Property is set to 'Close' :: {0}", ex.Message);
            }
            catch (WebException ex)
            {
                Toast.MakeText(Application.ApplicationContext, "There is an issue with the internet connection.", ToastLength.Short).Show();
                Out = string.Format("HTTP_ERROR :: WebException raised! :: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.ApplicationContext, "There is an issue with the internet connection.", ToastLength.Short).Show();
                Out = string.Format("HTTP_ERROR :: Exception raised! :: {0}", ex.Message);
            }

            return Out;
        }

    }
}