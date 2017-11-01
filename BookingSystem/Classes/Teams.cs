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
    class Teams
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string id { get; set; }

        public string teamName { get; set; }
        public string teamOwner { get; set; }
        public string leagueID { get; set; }
        public int gamesPlayed { get; set; }
        public int wins { get; set; }
        public int losses { get; set; }
        public int ties { get; set; }
        public int goalsFor { get; set; }
        public int goalsAgainst { get; set; }
    }
}