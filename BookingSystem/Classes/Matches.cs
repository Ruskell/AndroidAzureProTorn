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
    class Matches
    {

        [Newtonsoft.Json.JsonProperty("id")]
        public string id { get; set; }
        public string matchDate { get; set; }
        public string teamID1 { get; set; }
        public string teamID2 { get; set; }
        public string location { get; set; }
        public float team1Score { get; set; }
        public float team2Score { get; set; }
        public string timeStatus { get; set; }
    }
}