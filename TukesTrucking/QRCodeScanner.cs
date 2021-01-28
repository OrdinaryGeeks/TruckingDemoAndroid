
using System;
using System.Collections.Generic;
using ZXing.Mobile;
using Android.OS;

using Android.App;
using Android.Widget;
using Android.Content.PM;
using Android.Support.V4.Content;
using Android;
using Android.Support.V4.App;
using Android.Content;

namespace TukesTrucking
{
    [Activity(Label = "Tukes Trucking", Theme = "@android:style/Theme.Holo.Light", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class QRCodeScanner : global::Android.Support.V4.App.FragmentActivity
    {
        ZXingScannerFragment scanFragment;
        int CameraRequestCode = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == Permission.Granted)
            {

                scanFragment = new ZXingScannerFragment();

                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, scanFragment)
                    .Commit();

            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera }, 1000);
            }


            SetContentView(Resource.Layout.QRCodeReader);



        }
        protected override void OnDestroy()
        {
            base.OnDestroy();

               Console.WriteLine("KILLING QRCODE");
        }
        protected override void OnResume()
        {
            base.OnResume();


            scan();
            //scan();
        }

          public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
            // PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            for (int i = 0; i < permissions.Length; i++)
            {
                if (permissions[i].Equals("android.permission.CAMERA") && grantResults[i] == Permission.Granted)
                {
                    global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);


                    scanFragment = new ZXingScannerFragment();

                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.fragment_container, scanFragment)
                        .Commit();

                } 
            }
            
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        void scan()
        {
            var opts = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<ZXing.BarcodeFormat> {
                    ZXing.BarcodeFormat.QR_CODE
                },
                CameraResolutionSelector = availableResolutions =>
                {

                    foreach (var ar in availableResolutions)
                    {
                        Console.WriteLine("Resolution: " + ar.Width + "x" + ar.Height);
                    }
                    return null;
                }
            };

            scanFragment.StartScanning(result =>
            {

                // Null result means scanning was cancelled
                if (result == null || string.IsNullOrEmpty(result.Text))
                {
                    Toast.MakeText(this, "Scanning Cancelled", ToastLength.Long).Show();
                    return;
                }

                else
                {

                    Intent intent = new Intent(this, typeof(MainActivity));

                    intent.PutExtra("Tracking", result.ToString());
                    StartActivity(intent);
                    
                    string[] trackingInfo = result.ToString().Split(" ");
                }

                // Otherwise, proceed with result
           //     RunOnUiThread(() => Toast.MakeText(this, "Scanned: " + result.Text, ToastLength.Short).Show());
            }, opts);
        }
    }
}