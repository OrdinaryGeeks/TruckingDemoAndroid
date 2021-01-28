using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Android.Net;

using Xamarin.Essentials;
using TukesTrucking.Models;
using Newtonsoft.Json;

namespace TukesTrucking.Services
{
    public class DataStore
    {

        HttpClient client;


        public DataStore()
        {
            client = new HttpClient(new AndroidClientHandler());
            client.BaseAddress = new Uri("http://www.alectosinterdimensionalblog.com/TukesTrucking/");


        }

        bool IsConnected => Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;



        public async Task<Cargo> GetCargoAsync(int cargoID)
        {

            if (IsConnected)
            {
                var json = await client.GetStringAsync($"api/CargoesAPI/" + cargoID);
                return await Task.Run(() => JsonConvert.DeserializeObject<Cargo>(json));
            }

            return null;

        }
        public async Task<bool> PutCargoAsync(Cargo cargo)
        {
            if (cargo == null || !IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(cargo);

            var response = await client.PutAsync($"api/Cargoes/"+cargo.CargoID+"/Update", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }


        public async Task<Driver> GetDriverAsync(int driverID)
        {

            if (IsConnected)
            {
                var json = await client.GetStringAsync($"api/DriversApi/" + driverID);
                return await Task.Run(() => JsonConvert.DeserializeObject<Driver>(json));
            }

            return null;

        }
        public async Task<bool> PutDriverAsync(Driver driver)
        {
            if (driver == null || !IsConnected)
                return false;

            var serializedItem = JsonConvert.SerializeObject(driver);

            var response = await client.PutAsync($"api/Drivers/" + driver.DriverID + "/Update", new StringContent(serializedItem, Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }


    }
}