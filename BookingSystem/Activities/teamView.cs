using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Microsoft.WindowsAzure.MobileServices;
using BookingSystem.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Collections;
using Android.Views;

namespace BookingSystem.Activities
{
    [Activity(Label = "Team Viewer")]
    public class teamView : Activity
    {
        static IMobileServiceTable<Leagues> leaguesTable;
        static IMobileServiceTable<Teams> teamsTable;
        //static IMobileServiceTable<PlayerTeams> playerTeamTable;
        static IMobileServiceTable<Matches> matchTable;
        static IMobileServiceTable<Requests> reqTable;
        static MobileServiceClient MobileService;
        Leagues lg;
        List<Teams> teamList;
        List<Teams> teamGridList;
        List<Matches> matchList;
        List<Leagues> leagueList;
        List<Requests> reqList;
        Spinner lgSpinner, tmSpinner;
        ProgressDialog prog;
        AlertDialog.Builder alert;
        TableLayout tmTable;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TeamView);

            alert = new AlertDialog.Builder(this);
            prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetCancelable(false);
            prog.SetMessage("Loading Leagues...");
            prog.Show();

            lgSpinner = FindViewById<Spinner>(Resource.Id.spnLeague);
            tmSpinner = FindViewById<Spinner>(Resource.Id.spnTeam);
            tmTable = FindViewById<TableLayout>(Resource.Id.tabLayTeam);

            MobileService = MainActivity.MobileService;

            populateLeagues();

            lgSpinner.ItemSelected += (sender, e) =>
            {
                prog.SetMessage("Loading Leagues...");
                prog.Show();
                populateTeams();
            };

            tmSpinner.ItemSelected += (sender, e) =>
            {
                prog.SetMessage("Loading Teams...");
                prog.Show();
                populateTeamGrid();
                prog.Dismiss();
            };

            FindViewById<Button>(Resource.Id.btnTeamReq).Click += (sender, e) =>
            {
                prog.SetMessage("Generating Request...");
                prog.Show();
                requestTeamJoin();
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
            StartActivity(typeof(mainMenu));
        }

        private async Task requestTeamJoin()
        {
            Console.WriteLine("teamGridList[0] = " + teamGridList[0]);
            if (teamGridList[0] == null)
            {
                alert.SetTitle("Invalid");
                alert.SetMessage("There are no teams in this league.");
                prog.Dismiss();
                alert.Show();
                return;
            }

            reqTable = MobileService.GetTable<Requests>();

            reqList = await reqTable.Where(req => req.teamReqID == teamList[tmSpinner.SelectedItemPosition].id && req.playerReqID == MainActivity.user.id && !req.deleted).ToListAsync();

            if (reqList.Count == 0)
            {
                Requests req = new Requests
                {
                    playerReqID = MainActivity.user.id,
                    teamReqID = teamList[tmSpinner.SelectedItemPosition].id
                };
                await reqTable.InsertAsync(req);
                prog.Dismiss();
                alert.SetTitle("Request Sent!");
                alert.SetMessage("Now just wait until the team owner reviews your request.");
                alert.Show();
                return;
            }
            else
            {
                prog.Dismiss();
                alert.SetTitle("Request Denied!");
                alert.SetMessage("You have already sent a request to this team or are a member already.");
                alert.Show();
                return;
            }

        }

        private async Task populateTeamGrid()
        {

            if (tmTable.ChildCount > 1)
            {
                for (int i = tmTable.ChildCount; i != 1; i--)
                {
                    tmTable.RemoveViewAt(i - 1);
                }
            }

            tmTable.Visibility = ViewStates.Gone;
            tmTable.Visibility = ViewStates.Visible;

            //Get a list of teams in the league where the team's leagueID is equal to the selected leagueID
            if (teamList[tmSpinner.SelectedItemPosition] != null) teamGridList = await teamsTable.Where(tm => tm.id == teamList[tmSpinner.SelectedItemPosition].id).ToListAsync();
            

            if (teamGridList[0] != null)
            {

                TableRow row = FindViewById<TableRow>(Resource.Id.lgRow1);
                TextView GP = new TextView(this);
                TextView wins = new TextView(this);
                TextView losses = new TextView(this);
                TextView ties = new TextView(this);
                TextView GF = new TextView(this);
                TextView GA = new TextView(this);
                
                GP.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                GP.SetPadding(4, 2, 2, 2);
                GP.Text = teamGridList[0].gamesPlayed.ToString();
                wins.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                wins.SetPadding(5, 2, 2, 2);
                wins.Text = teamGridList[0].wins.ToString();
                losses.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                losses.SetPadding(4, 2, 2, 2);
                losses.Text = teamGridList[0].losses.ToString();
                ties.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                ties.SetPadding(5, 2, 2, 2);
                ties.Text = teamGridList[0].ties.ToString();
                GF.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                GF.SetPadding(5, 2, 2, 2);
                GF.Text = teamGridList[0].goalsFor.ToString();
                GA.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                GA.SetPadding(5, 2, 2, 2);
                GA.Text = teamGridList[0].goalsAgainst.ToString();
                
                row.AddView(GP);
                row.AddView(wins);
                row.AddView(losses);
                row.AddView(ties);
                row.AddView(GF);
                row.AddView(GA);

                tmTable.AddView(row);
            }
            prog.Dismiss();

        }

        public async Task populateSchedGrid()
        {
            matchTable = MobileService.GetTable<Matches>();
            prog.SetMessage("Awaiting Team Schedule...");
            prog.Show();

            //Get a list of teams in the league where the team's leagueID is equal to the selected leagueID
            matchList = await matchTable.Where(tm => tm.id == teamList[lgSpinner.SelectedItemPosition].id).ToListAsync();

            for (int i = tmTable.ChildCount; i != 1; i--)
            {
                tmTable.RemoveViewAt(i - 1);
            }
            prog.Dismiss();
        }

        public async Task populateLeagues()
        {
            leaguesTable = MobileService.GetTable<Leagues>();
            lg = new Leagues
            {
                id = null
            };

            leagueList = await leaguesTable.Where(lg => lg.id != null).ToListAsync();
            string[] strLeague = new string[leagueList.Count];

            for (int i = 0; i != leagueList.Count; i++)
            {
                strLeague[i] = leagueList[i].leagueName;
            }

            ArrayAdapter<String> lgAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, strLeague);
            lgAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            lgSpinner.Adapter = lgAdapter;
            prog.Dismiss();
        }

        public async Task populateTeams()
        {
            teamsTable = MobileService.GetTable<Teams>();
            
            teamList = await teamsTable.Where(tm => tm.leagueID == leagueList[lgSpinner.SelectedItemPosition].id).ToListAsync();
            string[] strTeam;
            if (teamList.Count < 1)
            {
                strTeam = new string[1];
                strTeam[0] = "There are no teams in this league.";
            }
            else
            {
                strTeam = new string[teamList.Count];
                for (int i = 0; i != teamList.Count; i++)
                {
                    strTeam[i] = teamList[i].teamName;
                }
            }

            ArrayAdapter<String> tmAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, strTeam);
            tmAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            tmSpinner.Adapter = tmAdapter;
            prog.Dismiss();
         }
       
    }
}