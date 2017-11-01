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

namespace BookingSystem
{
    [Activity(Label = "Main Menu")]
    public class mainMenu : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.MainMenu);

            FindViewById<Button>(Resource.Id.btnSchedule).Click += (sender, e) =>
            {
                StartActivity(typeof(Activities.scheduleView));
            };

            FindViewById<Button>(Resource.Id.btnTeams).Click += (sender, e) =>
            {
                StartActivity(typeof(Activities.teamView));
            };

            FindViewById<Button>(Resource.Id.btnLeague).Click += (sender, e) =>
            {
                StartActivity(typeof(Activities.leagueView));
            };

            FindViewById<Button>(Resource.Id.btnTeamManage).Click += (sender, e) =>
            {
                StartActivity(typeof(Activities.teamManager));
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            MainActivity.user = null;
            StartActivity(typeof(MainActivity));
            return true;
        }

        public override void OnBackPressed()
        {
        }
    }
}