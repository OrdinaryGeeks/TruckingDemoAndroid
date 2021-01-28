using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace TukesTrucking
{

    [Activity]
    public class GetCameraPermissionActivity : Activity
    {
        static readonly int REQUEST_CAMERA_PERMISSION = 3000;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);


            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
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
                RequestCameraPermission(REQUEST_CAMERA_PERMISSION);
            }

            while (!(ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted))
            {
                Log.Debug("wait", "waiting on access loc");


            }

            Thread.Sleep(200);

        


            Intent intent = new Intent(this, typeof(QRCodeScanner));
            StartActivity(intent);


          

        }

        void RequestCameraPermission(int requestCode)
        {




            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera))
            {


                //    Snackbar.Make(rootLayout, Resource.String.permission_location_rationale, Snackbar.LengthIndefinite)
                //     .SetAction(Resource.String.ok,
                //      delegate
                //{
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera }, requestCode);
                //  })
                // .Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.Camera }, requestCode);
            }
        }
    }
}