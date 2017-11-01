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

namespace BookingSystem.Classes
{
    class Leagues
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string id { get; set; }

        public string sport { get; set; }
        public string leagueName { get; set; }
        public string season { get; set; }
    }
}