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
    public class Players
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string      id        { get; set; }

        [Newtonsoft.Json.JsonProperty("DateRegistered")]
        public string   DateRegistered  { get; set; }

        [Newtonsoft.Json.JsonProperty("FirstName")]
        public string   FirstName       { get; set; }

        [Newtonsoft.Json.JsonProperty("LastName")]
        public string   LastName        { get; set; }

        [Newtonsoft.Json.JsonProperty("Address")]
        public string   Address         { get; set; }

        [Newtonsoft.Json.JsonProperty("City")]
        public string   City            { get; set; }

        [Newtonsoft.Json.JsonProperty("State")]
        public string   State           { get; set; }

        [Newtonsoft.Json.JsonProperty("Postcode")]
        public int      Postcode        { get; set; }

        [Newtonsoft.Json.JsonProperty("Country")]
        public string   Country         { get; set; }

        [Newtonsoft.Json.JsonProperty("HomePhone")]
        public int      HomePhone       { get; set; }

        [Newtonsoft.Json.JsonProperty("Mobile")]
        public int      Mobile          { get; set; }

        [Newtonsoft.Json.JsonProperty("Gender")]
        public string   Gender          { get; set; }

        [Newtonsoft.Json.JsonProperty("DateofBirth")]
        public string   DateofBirth     { get; set; }

        [Newtonsoft.Json.JsonProperty("EMailAddress")]
        public string   EMailAddress    { get; set; }

        [Newtonsoft.Json.JsonProperty("AccountStatus")]
        public string   AccountStatus   { get; set; }

        [Newtonsoft.Json.JsonProperty("Password")]
        public string   Password        { get; set; }
    }            
}