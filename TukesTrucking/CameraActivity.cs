using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using System.Threading;
using System;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android;
using Android.Content.PM;
using Android.Locations;
using Android.Gms.Common;

using Android.Util;

using System.Linq;
using Android.Support.V4.Content;
using Android.Support.V4.App;

namespace TukesTrucking
{
	[Activity (Label = "Camera2Basic",  Icon = "@drawable/icon")]
	public class CameraActivity : Activity
	{
        static readonly int REQUEST_CAMERA_PERMISSION = 3000;
        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			//    ActionBar.Hide ();
			SetContentView (Resource.Layout.activity_camera);

            //    RequestPermission(REQUEST_CAMERA_PERMISSION);
            try
            {
                if (bundle == null)
                {
                    FragmentManager.BeginTransaction().Replace(Resource.Id.container, Camera2BasicFragment.NewInstance()).Commit();
                }
            }
            catch (Exception e)
            
            { }
		}


        void RequestPermission(int requestCode)
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


