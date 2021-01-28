using Android.App;
using Android.OS;
using Android.Runtime;
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
using TukesTrucking.Models;
using TukesTrucking.Services;


using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Content;

namespace TukesTrucking
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", LaunchMode =LaunchMode.SingleInstance)]
    public class MainActivity : Activity, BottomNavigationView.IOnNavigationItemSelectedListener,/* IOnMapReadyCallback,*/ Android.Locations.ILocationListener
    {

        List<Marker> Markers;
        TextView textMessage;
        int updateCount = 0;

        const long ONE_MINUTE = 60 * 1000;
        const long FIVE_MINUTES = 5 * ONE_MINUTE;
        static readonly string KEY_REQUESTING_LOCATION_UPDATES = "requesting_location_updates";

        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;

        Button getLastLocationButton;
        bool isRequestingLocationUpdates;
        //TextView latitude;
        //ternal TextView latitude2;
        LocationManager locationManager;
        //  TextView longitude;
        //        internal TextView longitude2;
        //    TextView provider;

        double latitude;
        double longitude;
        //FusedLocationProviderCallback locationCallback;
        GoogleMap theMap;
        static readonly int REQUEST_PERMISSIONS_LOCATION = 1000;
        static readonly int REQUEST_CAMERA_PERMISSION = 3000;
        LocationManager locMan;
        // FusedLocationProviderClient fusedLocationProviderClient;
        View rootLayout;
        public static Cargo cargo;
        public static Driver driver;
        DataStore dataStore;
        bool freeze;
        TextView provider;

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }


        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.activity_main);

            Xamarin.Essentials.Platform.Init(this, bundle);
            //   Xamarin.Essentials.Platform.Init(this, bundle);
                freeze = false;
            //  globals = new Globals();
            string cargoInfo = "";
            string trackingInfo = "";
            int cargoID = 0;
            int driverID = 0;

            if (bundle == null)
            {
             trackingInfo= Intent.GetStringExtra("Tracking");

                string[] trackingInfoSplit;
                if (trackingInfo.Length > 0)
                {
                    trackingInfoSplit = trackingInfo.Split(" ");

                    for(int i = 0; i<trackingInfoSplit.Length; i++)
                    {

                        if (trackingInfoSplit[i] == "cargoID")
                            cargoID = int.Parse(trackingInfoSplit[i + 1]);

                        if (trackingInfoSplit[i] == "driverID")
                            driverID = int.Parse(trackingInfoSplit[i + 1]);
                    }
                   


                }
            }

            else
            {
                cargoID = bundle.GetInt("CargoID");

            }


            //  Task.Run(async () => await GetCargo());

            //cargo = new Ca  rgo();


            cargo = await Loading.globals.DataStore.GetCargoAsync(cargoID);

            driver = await Loading.globals.DataStore.GetDriverAsync(driverID);
            Intent intent = new Intent(this, typeof(DriverOnMap));

            intent.PutExtra("cargoID", cargoID);
            intent.PutExtra("cargoLat", cargo.Latitude);
            intent.PutExtra("cargoLong", cargo.Longitude);

            intent.PutExtra("driverID", driverID);
            intent.PutExtra("driverLat", driver.Latitude);
            intent.PutExtra("driverLong", driver.Longitude);
            StartActivity(intent);
            /*

Markers = new List<Marker>();
var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map2);
mapFragment.GetMapAsync(this);

rootLayout = (View)FindViewById(Resource.Id.container);
// fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
//  locationCallback = new FusedLocationProviderCallback(this);

textMessage = FindViewById<TextView>(Resource.Id.message);
provider = FindViewById<TextView>(Resource.Id.provider);
BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
navigation.SetOnNavigationItemSelectedListener(this);

//  await NavigateDownTheStreet(30.0, 30.0);

locationManager = (LocationManager)GetSystemService(LocationService);

if (bundle != null)
{
isRequestingLocationUpdates = bundle.KeySet().Contains(KEY_REQUESTING_LOCATION_UPDATES) &&
                          bundle.GetBoolean(KEY_REQUESTING_LOCATION_UPDATES);
}
else
{
isRequestingLocationUpdates = false;
}


if (locationManager.AllProviders.Contains(LocationManager.NetworkProvider)
&& locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
{

if (isRequestingLocationUpdates)
{
isRequestingLocationUpdates = false;
StopRequestingLocationUpdates();
}
else
{
if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
{




    var criteria = new Criteria { PowerRequirement = Power.High };

    var bestProvider = locationManager.GetBestProvider(criteria, true);
    Android.Locations.Location location = locationManager.GetLastKnownLocation(bestProvider);

    provider.Text = bestProvider.ToString();

  textMessage.Text = location.Latitude + " " + location.Longitude + " " + location.Altitude;
  StartRequestingLocationUpdates();
  isRequestingLocationUpdates = true;
}
else
{
    RequestLocationPermission(RC_LAST_LOCATION_PERMISSION_CHECK);
}
}
}
//   if (this.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
{
//       locMan = new LocationManager()
//       textMessage =

}
*/
        }

        public async Task<Cargo> GetCargo(int cargoID)
        {


        return await Loading.globals.DataStore.GetCargoAsync(cargoID);

        }

        protected override void OnSaveInstanceState(Bundle outState)
        {

            Console.WriteLine("OnSaveInstance Main");
            if(cargo!= null)

            outState.PutInt("CargoID", cargo.CargoID);
            // always call the base implementation!
            base.OnSaveInstanceState(outState);

        }
        public async void updateCargoLocation()
        {
            try
            {
                await dataStore.PutCargoAsync(cargo);
                textMessage.Text = (updateCount++).ToString();
            }
            catch (Exception e)
            {

                provider.Text = "Error with UCL" + " " + cargo.Latitude + " " + cargo.Longitude + " " + e.Message;
            }

        }

        public void OnProviderDisabled(string provider)
        {
            isRequestingLocationUpdates = false;
            textMessage.Text = provider + " is disabled";
        
        }

        public void OnProviderEnabled(string provider)
        {
            // Nothing to do in this example.
            Log.Debug("LocationExample", "The provider " + provider + " is enabled.");
        }

      
        public void OnLocationChanged(Android.Locations.Location location)
        {


            if (location == null)
            {


                textMessage.Text = "NULL";
               // freeze = true;


            }

            else
            {
              //  freeze = false;
                textMessage.Text = "OLc" + location.Latitude + " " + location.Longitude + " " + location.Altitude;


                cargo.Latitude = location.Latitude;
                cargo.Longitude = location.Longitude;

                updateCargoLocation();

                AddMarkersToMap();

            }
        //    Toast.MakeText(this, "updating cargo", ToastLength.Short);
            //latitude2.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            //longitude2.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
            //provider2.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
        }

        void AddMarkersToMap()
        {

            MarkerOptions tempMarker;
            LatLng templatLng = new LatLng(0.0, 0.0);

            if(Markers.Count>0)
            Markers[0].Remove();

            theMap.Clear();

                tempMarker = new MarkerOptions();

                tempMarker.SetPosition(new LatLng(cargo.Latitude,cargo.Longitude)).SetTitle("Tuke").SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
                Markers.Add(theMap.AddMarker(tempMarker));
                templatLng = new LatLng(cargo.Latitude, cargo.Longitude);


            
            /*  var vimyMarker = new MarkerOptions();
              vimyMarker.SetPosition(VimyRidgeLatLng)
                        .SetTitle("Vimy Ridge")
                        .SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueCyan));
              googleMap.AddMarker(vimyMarker);


              var passchendaeleMarker = new MarkerOptions();
              passchendaeleMarker.SetPosition(PasschendaeleLatLng)
                                 .SetTitle("PasschendaeleLatLng");
              googleMap.AddMarker(passchendaeleMarker);
            */
            // We create an instance of CameraUpdate, and move the map to it.
            var cameraUpdate = CameraUpdateFactory.NewLatLngZoom(templatLng, 15);
            theMap.MoveCamera(cameraUpdate);
        }

        void RequestLocationPermission(int requestCode)
        {
            isRequestingLocationUpdates = false;
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
        void StartRequestingLocationUpdates()
        {
            long time =  60;



            var criteria = new Criteria { PowerRequirement = Power.High };

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            Android.Locations.Location location =locationManager.GetLastKnownLocation(bestProvider);
            provider.Text = bestProvider.ToString();

            textMessage.Text = location.Latitude + " " + location.Longitude + " " + location.Altitude;
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, time, .01f, (Android.Locations.ILocationListener)this);
        }

        void StopRequestingLocationUpdates()
        {
            //  latitude2.Text = string.Empty;
            // longitude2.Text = string.Empty;

            //     requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
            textMessage.Text = "Stopping";
            locationManager.RemoveUpdates((Android.Locations.ILocationListener)this);

            StartRequestingLocationUpdates();
        }
    
    bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
                Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",
                          queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }


        /*      async Task GetLastLocationFromDevice()
              {
                  // This method assumes that the necessary run-time permission checks have succeeded.
                  // getLastLocationButton.SetText(Resource.String.getting_last_location);
                  Android.Locations.Location location = await fusedLocationProviderClient.GetLastLocationAsync();

                  if (location == null)
                  {
                      // Seldom happens, but should code that handles this scenario
                  }
                  else
                  {
                      // Do something with the location 
                      Log.Debug("Sample", "The latitude is " + location.Latitude);
                  }
                  LocationRequest locationRequest = new LocationRequest()
                                        .SetPriority(LocationRequest.PriorityHighAccuracy)
                                        .SetInterval(1000)
                                        .SetFastestInterval(1000 * 2);
                  await fusedLocationProviderClient.RequestLocationUpdatesAsync(locationRequest, locationCallback);


              }
              void InitializeUiSettingsOnMap()
              {
                  theMap.UiSettings.MyLocationButtonEnabled = true;
                  theMap.UiSettings.CompassEnabled = true;
                  theMap.UiSettings.ZoomControlsEnabled = true;
                  theMap.MyLocationEnabled = true;



              }
        */
        void GetLastLocationFromDevice()
        {
            //   getLastLocationButton.SetText(Resource.String.getting_last_location);

            var criteria = new Criteria { PowerRequirement = Power.Medium };

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var location = locationManager.GetLastKnownLocation(bestProvider);

            if (location != null)
            {

                textMessage.Text = location.Latitude + " " + location.Longitude + " " + location.Altitude;
                Toast.MakeText(this, "changing in getlastloc", ToastLength.Short);
             //latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
             //  longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
             //provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
             //getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
            else
            {
                //latitude.SetText(Resource.String.location_unavailable);
                //longitude.SetText(Resource.String.location_unavailable);
                //provider.Text = Resources.GetString(Resource.String.provider_string, bestProvider);
                //getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK || requestCode == RC_LOCATION_UPDATES_PERMISSION_CHECK)
            {
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    if (requestCode == RC_LAST_LOCATION_PERMISSION_CHECK)
                    {
                        GetLastLocationFromDevice();
                    }
                    else
                    {
                        isRequestingLocationUpdates = true;
                        StartRequestingLocationUpdates();
                    }
                }
                else
                {
                    Snackbar.Make(rootLayout,"Permission not granted", Snackbar.LengthIndefinite)
                            .SetAction("OK", delegate { FinishAndRemoveTask(); })
                            .Show();
                    return;
                }
            }
            else
            {
                Log.Debug("LocationSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
  
        public async Task NavigateDownTheStreet(double latS, double longS)
        {
            var location = new Xamarin.Essentials.Location(34.964540899999996, -90.0306415);
            var options = new MapLaunchOptions {NavigationMode=NavigationMode.Driving };
//
            try
            {
                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                // No map application available to open
            }
        }
        public void OnMapReady(GoogleMap googleMap)

        {
            theMap = googleMap;
           
           

           // googleMap.SetOnMarkerClickListener((this));

            //     animateToLocationButton.Click += AnimateToPasschendaele;
            LatLng location = new LatLng(cargo.Latitude, cargo.Longitude);

            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location).Zoom(15);
            //builder.Zoom(18);
            //  builder.Bearing(155);
            // builder.Tilt(65);

            CameraPosition cameraPosition = builder.Build();

            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);


            theMap.MoveCamera(cameraUpdate);
        }
        /* public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
         {
             Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

             base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
         }*/
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    textMessage.SetText(Resource.String.title_home);
                    return true;
                case Resource.Id.navigation_dashboard:
                    textMessage.SetText(Resource.String.title_dashboard);
                    return true;
                case Resource.Id.navigation_notifications:
                    textMessage.SetText(Resource.String.title_notifications);
                    return true;
            }
            return false;
        }

  /*      public class FusedLocationProviderCallback : LocationCallback
        {
            readonly MainActivity activity;

            public FusedLocationProviderCallback(MainActivity activity)
            {
                this.activity = activity;
            }

            public override void OnLocationAvailability(LocationAvailability locationAvailability)
            {
                Log.Debug("FusedLocationProviderSample", "IsLocationAvailable: {0}", locationAvailability.IsLocationAvailable);
            }

            void GetLastLocationFromDevice()
            {
             //   getLastLocationButton.SetText(Resource.String.getting_last_location);

                var criteria = new Criteria { PowerRequirement = Power.Medium };

                var bestProvider = locationManager.GetBestProvider(criteria, true);
                var location = locationManager.GetLastKnownLocation(bestProvider);

                if (location != null)
                {//
                    //latitude.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
                  //  longitude.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
                    //provider.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
                    //getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
                }
                else
                {
                    //latitude.SetText(Resource.String.location_unavailable);
                    //longitude.SetText(Resource.String.location_unavailable);
                    //provider.Text = Resources.GetString(Resource.String.provider_string, bestProvider);
                    //getLastLocationButton.SetText(Resource.String.get_last_location_button_text);
                }
            }
            public override void OnLocationResult(LocationResult result)
            {
                if (result.Locations.Any())
                {
                    var location = result.Locations.First();
                    Log.Debug("Sample", "The latitude is :" + location.Latitude);
                    activity.textMessage.Text = location.Latitude + " " +  location.Longitude + " " +  location.Altitude;
                }
                else
                {
                    // No locations to work with.
                }
            }
        }

*/
    }
   

}

