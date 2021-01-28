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
using TukesTrucking.Services;

using Android.Support.V4.Content;

using Android;
using Android.Content.PM;
using Android.Support.V4.App;
using System.Threading;
using Android.Util;

namespace TukesTrucking
{

    [Activity(MainLauncher = true,Label = "Tukes Trucking2", Theme = "@android:style/Theme.Holo.Light")]
    public class Loading: Activity
    {

        static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;
        static readonly int REQUEST_CAMERA_PERMISSION = 3000;

        //static readonly int REQUEST_CAMERA_PERMISSION = 6000;


        public static Globals globals;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);



            globals = new Globals();



            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {




            /*    var criteria = new Criteria { PowerRequirement = Power.High };

                var bestProvider = locationManager.GetBestProvider(criteria, true);
                Android.Locations.Location location = locationManager.GetLastKnownLocation(bestProvider);

                provider.Text = bestProvider.ToString();

                textMessage.Text = location.Latitude + " " + location.Longitude + " " + location.Altitude;
                StartRequestingLocationUpdates();
                isRequestingLocationUpdates = true;*/
            }
            else
            {
                RequestLocationPermission(REQUEST_PERMISSIONS_LOCATION);
            }

            while (!(ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted))
            {
                Log.Debug("wait","waiting on access loc");
                

            }

            Thread.Sleep(200);
               
            Intent intent = new Intent(this, typeof(GetCameraPermissionActivity));
            StartActivity(intent);

            // intent.PutExtra("Tracking", result.ToString());
            //   StartActivity(intent);

            // string[] trackingInfo = result.ToString().Split(" ");
        }


      
        void RequestLocationPermission(int requestCode)
        {
          
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                //    Snackbar.Make(rootLayout, Resource.String.permission_location_rationale, Snackbar.LengthIndefinite)
                //     .SetAction(Resource.String.ok,
                //      delegate
                //{
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
                //  })
                // .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.AccessFineLocation }, requestCode);
            }
        }
    }
}