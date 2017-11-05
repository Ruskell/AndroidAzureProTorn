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
        static IMobileServiceTable<PlayerTeams> playerTeamTable;
        static IMobileServiceTable<Matches> matchTable;
        static IMobileServiceTable<Teams> teamTable;

        List<PlayerTeams> playerTeamList;
        List<Matches> matchList;
        List<Teams> teamList;

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

            playerTeamTable = MobileService.GetTable<PlayerTeams>();
            matchTable = MobileService.GetTable<Matches>();
            teamTable = MobileService.GetTable<Teams>();

            populateSchedule();

        }

        public async Task populateTeamList()
        {
        }

        public async Task populateSchedule()
        {
            matchList = new List<Matches>();
            playerTeamList = await playerTeamTable.Where(tm => tm.playerID == MainActivity.user.id).ToListAsync();
            if (playerTeamList.Count == 0)
            {
                prog.Dismiss();
                return;
            }
            
            try
            {
                foreach (PlayerTeams team in playerTeamList)
                {
                    List<Matches> tempList = await matchTable.Where(match => match.teamID1 == team.teamID || match.teamID2 == team.teamID).ToListAsync();
                    foreach (Matches match in tempList)
                    {
                        matchList.Add(match);
                    }
                }
            }
            catch (Exception e)
            {
                prog.Dismiss();
                alert.SetTitle("Something went wrong!");
                alert.SetMessage("Error: " + e);
                alert.Show();

                return;
            }

            Console.WriteLine("testpoint");
            teamList = await teamTable.ToListAsync();
            //await populateTeamList();

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

                home.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 2);
                home.SetPadding(4, 2, 2, 2);
                home.Text = getTeamName(match.teamID1);

                score.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                score.SetPadding(4, 2, 2, 2);
                score.Text = match.team1Score + " v " + match.team2Score;

                away.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 2);
                away.SetPadding(4, 2, 2, 2);
                away.Text = getTeamName(match.teamID2);

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
        }

        string getTeamName(string ID)
        {
            try
            {
                foreach (Teams team in teamList)
                {
                    if (team.id == ID) return team.teamName;
                }

            }
            catch (Exception e)
            {
                prog.Dismiss();
                alert.SetTitle("Something went wrong!");
                alert.SetMessage("Error: " + e);
                alert.Show();
                return "ERROR team";
            }
            return "NULL team";
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