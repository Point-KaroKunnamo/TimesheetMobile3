using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using System.Runtime.Remoting.Contexts;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;
using Android.Locations;
using Android;
using Android.Support.V4.Content;
using Plugin.Media;

namespace TimesheetMobile3.Droid
{
    public static class ImageInfo
    {
        public static Java.IO.File _file;
        public static Java.IO.File _dir;
        public static Android.Graphics.Bitmap bitmap;
    }
    [Activity(Label = "TimesheetMobile", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, ILocationListener
    {
        public static Android.Locations.LocationManager LocationManager;
        public static MainActivity AndroidMainActivity;

        // kuvan ottamisen jälkeen suoritettava metodi eli Action
        private Action pictureTaken;
        protected override async void OnCreate(Bundle bundle)
        {
            // lisätty
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);

            await CrossMedia.Current.Initialize();

            AndroidMainActivity = this;

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            // käynnistetään GPS-paikannus
            try
            {
                LocationManager = GetSystemService(
                    "location") as LocationManager;
                string Provider = LocationManager.GpsProvider;

                if (LocationManager.IsProviderEnabled(Provider))
                {
                    LocationManager.RequestLocationUpdates(
                        Provider, 2000, 1, this);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region Paikkatieto

        public void OnLocationChanged(Location location)
        {
            TimesheetMobile3.Models.GpsLocationModel.Latitude = location.Latitude;
            TimesheetMobile3.Models.GpsLocationModel.Longitude = location.Longitude;
            TimesheetMobile3.Models.GpsLocationModel.Altitude = location.Altitude;
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
        #endregion

        //#region Kuvan ottaminen
        ////Kutsutaan kun nappia painetaan
        //public void TakeAPicture(Action pictureTaken)
        //{
        //    //Intent tyyppiä käytetään käynnistämään androidissa muita sovelluksia
        //    Intent intent = new Intent(MediaStore.ActionImageCapture);
        //    //määritellään tiedosto, johon kuva tallennetaan
        //    ImageInfo._file = new Java.IO.File(ImageInfo._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
        //    //kerrotaan intentille mihin tiedostoon kuva tallennetaan
        //    intent.PutExtra(MediaStore.ExtraOutput,
        //        Android.Net.Uri.FromFile(ImageInfo._file));

        //    // tallennetaan annettu tapahtuma/action
        //    this.pictureTaken = pictureTaken;

        //    //käynnistetään määritelty intent
        //    StartActivityForResult(intent, 0);
        //}
        //#endregion
    }
}