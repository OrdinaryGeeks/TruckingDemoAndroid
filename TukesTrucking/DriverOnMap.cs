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
using Android.Gms.Common;


using Android.Support.V4.App;
using TukesTrucking.Models;
using TukesTrucking.Services;


using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using Android.Gms.Maps;
using Android.Locations;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace TukesTrucking
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", LaunchMode = LaunchMode.SingleInstance)]
    public class DriverOnMap : Activity, BottomNavigationView.IOnNavigationItemSelectedListener, IOnMapReadyCallback, Android.Locations.ILocationListener
    {


        float DriverHue;
        float CargoHue;
        bool cargoPickedUp;

        const long ONE_MINUTE = 60 * 1000;
        Button getLastLocationButton;
        bool isRequestingLocationUpdates;
        GoogleMap theMap;
        double latitude;
        double longitude;
        public static Cargo cargo;
        int cargoID;
        public static Driver driver;

        LocationManager locationManager;
        static readonly string KEY_REQUESTING_LOCATION_UPDATES = "requesting_location_updates";

        static readonly int RC_LAST_LOCATION_PERMISSION_CHECK = 1000;
        static readonly int RC_LOCATION_UPDATES_PERMISSION_CHECK = 1100;

        List<Marker> Markers;
        protected async override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);

            DriverHue = BitmapDescriptorFactory.HueCyan;
            CargoHue = BitmapDescriptorFactory.HueRed;
            cargoPickedUp = false;

            SetContentView(Resource.Layout.MapLayout);

            cargo = new Cargo();

            cargo.Latitude  = Intent.GetDoubleExtra("Lat", 0.0);
            cargo.Longitude = Intent.GetDoubleExtra("Long", 0.0f);
            cargo.CargoID = Intent.GetIntExtra("cargoID", 0);



            Markers = new List<Marker>();
            var mapFragment = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.map2);
            mapFragment.GetMapAsync(this);

            //rootLayout = (View)FindViewById(Resource.Id.container);
            // fusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(this);
            //  locationCallback = new FusedLocationProviderCallback(this);

          //  textMessage = FindViewById<TextView>(Resource.Id.message);
           // provider = FindViewById<TextView>(Resource.Id.provider);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);
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


        }
        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            if (status == Availability.OutOfService)
            {
                StopRequestingLocationUpdates();
                isRequestingLocationUpdates = false;
            }
        }
        public async void updateDriverLocation()
        {
            try
            {

                await Loading.globals.DataStore.PutDriverAsync(MainActivity.driver);
                // textMessage.Text = (updateCount++).ToString();
            }
            catch (Exception e)
            {

                //     provider.Text = "Error with UCL" + " " + cargo.Latitude + " " + cargo.Longitude + " " + e.Message;
            }

        }
        public async void updateCargoLocation()
        {
            try
            {
                await Loading.globals.DataStore.PutCargoAsync(cargo);
               // textMessage.Text = (updateCount++).ToString();
            }
            catch (Exception e)
            {

           //     provider.Text = "Error with UCL" + " " + cargo.Latitude + " " + cargo.Longitude + " " + e.Message;
            }

        }
        public void OnProviderDisabled(string provider)
        {
            isRequestingLocationUpdates = false;
        //    textMessage.Text = provider + " is disabled";

        }

        public void OnProviderEnabled(string provider)
        {
            // Nothing to do in this example.
          //  Log.Debug("LocationExample", "The provider " + provider + " is enabled.");
        }

        public bool checkDistanceBetweenDriverAndCargoForPickup()
        {

            double distance = distanceInKmBetweenEarthCoordinates(MainActivity.driver.Latitude, MainActivity.driver.Longitude, MainActivity.cargo.Latitude, MainActivity.cargo.Longitude);
            if (distance < 50)
            {
                DriverHue = BitmapDescriptorFactory.HueGreen;
                cargoPickedUp = true;
                return true;
            }
            else
                return false;
        }
        public void OnLocationChanged(Android.Locations.Location location)
        {


            if (location == null)
            {


              //  textMessage.Text = "NULL";
                // freeze = true;


            }

            else
            {
                //  freeze = false;
                //    textMessage.Text = "OLc" + location.Latitude + " " + location.Longitude + " " + location.Altitude;


                //MainActivity.cargo.Latitude = location.Latitude;
                //cargo.Longitude = location.Longitude;

                //  updateCargoLocation();

                MainActivity.driver.Latitude = location.Latitude;
                MainActivity.driver.Longitude = location.Longitude;

                checkDistanceBetweenDriverAndCargoForPickup();
                if (cargoPickedUp)
                {
                  //  updateCargoLocation();

                    MainActivity.cargo.Latitude = location.Latitude;
                    MainActivity.cargo.Longitude = location.Longitude;
                    updateCargoLocation();
                }

                    updateDriverLocation();

                AddMarkersToMap();

            }
            //    Toast.MakeText(this, "updating cargo", ToastLength.Short);
            //latitude2.Text = Resources.GetString(Resource.String.latitude_string, location.Latitude);
            //longitude2.Text = Resources.GetString(Resource.String.longitude_string, location.Longitude);
            //provider2.Text = Resources.GetString(Resource.String.provider_string, location.Provider);
        }

        public double degreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public double distanceInKmBetweenEarthCoordinates(double lat1, double lon1, double lat2, double lon2)
        {
            var earthRadiusM = 6371000;

            var dLat = degreesToRadians(lat2 - lat1);
            var dLon = degreesToRadians(lon2 - lon1);

            lat1 = degreesToRadians(lat1);
            lat2 = degreesToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusM * c;
        }
        void AddMarkersToMap()
        {

            MarkerOptions tempMarker;
            LatLng templatLng = new LatLng(0.0, 0.0);

            if (Markers.Count > 0)
                Markers[0].Remove();

            theMap.Clear();

            if (!cargoPickedUp)
            {
                tempMarker = new MarkerOptions();


                tempMarker.SetPosition(new LatLng(MainActivity.cargo.Latitude, MainActivity.cargo.Longitude)).SetTitle("Tuke").SetIcon(BitmapDescriptorFactory.DefaultMarker(CargoHue));
                Markers.Add(theMap.AddMarker(tempMarker));
            }


            tempMarker = new MarkerOptions();
            
                tempMarker.SetPosition(new LatLng(MainActivity.driver.Latitude, MainActivity.driver.Longitude)).SetTitle(MainActivity.driver.DriverName).SetIcon(BitmapDescriptorFactory.DefaultMarker(DriverHue));
                Markers.Add(theMap.AddMarker(tempMarker));
            

            templatLng = new LatLng(MainActivity.driver.Latitude, MainActivity.driver.Longitude);



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
            long time = 60;



            var criteria = new Criteria { PowerRequirement = Power.High };

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            Android.Locations.Location location = locationManager.GetLastKnownLocation(bestProvider);
//            provider.Text = bestProvider.ToString();

  //          textMessage.Text = location.Latitude + " " + location.Longitude + " " + location.Altitude;
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, time, .01f, (Android.Locations.ILocationListener)this);
        }

        void StopRequestingLocationUpdates()
        {
            //  latitude2.Text = string.Empty;
            // longitude2.Text = string.Empty;

            //     requestLocationUpdatesButton.SetText(Resource.String.request_location_button_text);
         //   textMessage.Text = "Stopping";
            locationManager.RemoveUpdates((Android.Locations.ILocationListener)this);

            StartRequestingLocationUpdates();
        }

        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
          //      Log.Info("MainActivity", "Google Play Services is installed on this device.");
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                // Check if there is a way the user can resolve the issue
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
            //    Log.Error("MainActivity", "There is a problem with Google Play Services on this device: {0} - {1}",queryResult, errorString);

                // Alternately, display the error to the user.
            }

            return false;
        }

        void GetLastLocationFromDevice()
        {
            //   getLastLocationButton.SetText(Resource.String.getting_last_location);

            var criteria = new Criteria { PowerRequirement = Power.Medium };

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var location = locationManager.GetLastKnownLocation(bestProvider);

            if (location != null)
            {

           //     textMessage.Text = location.Latitude + " " + location.Longitude + " " + location.Altitude;
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
                  //  Snackbar.Make(rootLayout, "Permission not granted", Snackbar.LengthIndefinite)
                     //       .SetAction("OK", delegate { FinishAndRemoveTask(); })
                       //     .Show();
                    return;
                }
            }
            else
            {
               // Log.Debug("LocationSample", "Don't know how to handle requestCode " + requestCode);
            }

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task NavigateDownTheStreet(double latS, double longS)
        {
            var location = new Xamarin.Essentials.Location(34.964540899999996, -90.0306415);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
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
                   // textMessage.SetText(Resource.String.title_home);
                    return true;
                case Resource.Id.navigation_dashboard:
                  // textMessage.SetText(Resource.String.title_dashboard);
                    return true;
                case Resource.Id.navigation_notifications:
                   // textMessage.SetText(Resource.String.title_notifications);
                    return true;
            }
            return false;
        }




































    }
}