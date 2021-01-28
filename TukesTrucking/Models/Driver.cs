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

namespace TukesTrucking.Models
{
    public class Driver
    {
        public int DriverID { get; set; }

        public virtual ICollection<Cargo> Cargos { get; set; }

        public string DriverName { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Password { get; set; }
    }
}