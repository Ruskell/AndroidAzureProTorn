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
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using BookingSystem.Classes;

namespace BookingSystem.Activities
{
    [Activity(Label = "Personal Schedule")]
    public class scheduleView : Activity
    {
        static MobileServiceClient MobileService;
        static IMobileServiceTable<PlayerTeams> teamTable;
        static IMobileServiceTable<Matches> matchTable;

        List<PlayerTeams> teamList;
        List<Matches> matchList;

        ProgressDialog prog;
        AlertDialog.Builder alert;
        TableLayout scTable;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ScheduleView);
            MobileService = MainActivity.MobileService;

            alert = new AlertDialog.Builder(this);
            prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetCancelable(false);
            prog.SetMessage("Loading Schedule...");
            prog.Show();

            scTable = FindViewById<TableLayout>(Resource.Id.tabLaySched);

            populateSchedule();

        }

        private async Task populateSchedule()
        {
            teamList = await teamTable.Where(tm => tm.playerID == MainActivity.user.id).ToListAsync();
            if (teamList.Count == 0)
            {
                prog.Dismiss();
                return;
            }

            try
            {
                foreach (PlayerTeams team in teamList)
                {
                    List<Matches> tempList = await matchTable.Where(match => match.teamID1 == team.teamID || match.teamID2 == team.teamID).ToListAsync();
                    for (int i = 0; i != tempList.Count; i++)
                    {
                        matchList.Add(tempList[i]);
                    }
                }
            }
            catch (Exception e)
            {
                alert.SetTitle("Something went wrong!");
                alert.SetMessage("Error: " + e);
                alert.Show();
                return;
            }

            foreach (Matches match in matchList)
            {
                TableRow tr = new TableRow(this);
                TextView date = new TextView(this);
                TextView home = new TextView(this);
                TextView score = new TextView(this);
                TextView away = new TextView(this);
                TextView time = new TextView(this);
                TextView location = new TextView(this);


                date.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                date.SetPadding(4, 2, 2, 2);
                date.Text = match.matchDate;

                home.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                home.SetPadding(4, 2, 2, 2);
                home.Text = match.teamID1;

                score.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                score.SetPadding(4, 2, 2, 2);
                score.Text = match.team1Score + " v " + match.team2Score;

                away.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                away.SetPadding(4, 2, 2, 2);
                away.Text = match.teamID2;

                time.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                time.SetPadding(4, 2, 2, 2);
                time.Text = match.timeStatus;

                location.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                location.SetPadding(4, 2, 2, 2);
                location.Text = match.location;

                tr.AddView(date);
                tr.AddView(home);
                tr.AddView(score);
                tr.AddView(away);
                tr.AddView(time);
                tr.AddView(location);

                scTable.AddView(tr);
            }
            prog.Dismiss();



            return;
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
            StartActivity(typeof(mainMenu));
        }
    }
}