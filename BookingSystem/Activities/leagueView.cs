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
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using BookingSystem.Classes;

namespace BookingSystem.Activities
{
    [Activity(Label = "Leagues")]
    public class leagueView : Activity
    {

        static IMobileServiceTable<Teams> teamsTable;
        static IMobileServiceTable<Leagues> leaguesTable;
        static MobileServiceClient MobileService;
        Leagues lg;
        Spinner lgSpinner;
        TableLayout lgTable;
        ProgressDialog prog;
        List<Leagues> leagueList;
        List<Teams> teamList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.LeagueView);

            prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetMessage("Finding Leagues...");
            prog.SetCancelable(false);
            prog.Show();

            lgTable = FindViewById<TableLayout>(Resource.Id.tabLayLeague);
            lgSpinner = FindViewById<Spinner>(Resource.Id.spnLeague);
            MobileService = MainActivity.MobileService;

            populateLeagues();

            lgSpinner.ItemSelected += (sender, e) =>
            {
                populateTable();
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

        public async Task populateLeagues()
        {
            leaguesTable = MobileService.GetTable<Leagues>();

            lg = new Leagues
            {
                id = null
            };

            //query db for all leagues
            leagueList = await leaguesTable.Where(lg => lg.id != null).ToListAsync();

            //create a list of league name strings to be used in the league spinner
            string[] strLeague = new string[leagueList.Count];
            for (int i = 0; i != leagueList.Count; i++)
            {
                strLeague[i] = leagueList[i].leagueName;
            }

            //bind strings to league adapter => league spinner
            ArrayAdapter<String> lgAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, strLeague);
            lgAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            lgSpinner.Adapter = lgAdapter;
            prog.Dismiss();
        }

        public async Task populateTable()
        {

            teamsTable = MobileService.GetTable<Teams>();
            prog.SetMessage("Awaiting Teams...");
            prog.Show();

            //Get a list of teams in the league where the team's leagueID is equal to the selected leagueID
            teamList = await teamsTable.Where(tm => tm.leagueID == leagueList[lgSpinner.SelectedItemPosition].id).ToListAsync();
            
            for (int i = lgTable.ChildCount; i != 1; i--)
            {
                lgTable.RemoveViewAt(i-1);
            }

            for (int i = 0; i != teamList.Count; i++)
            {
                TableRow row = new TableRow(this);
                TextView tmName = new TextView(this);
                TextView GP = new TextView(this);
                TextView wins = new TextView(this);
                TextView losses = new TextView(this);
                TextView ties = new TextView(this);
                TextView GF = new TextView(this);
                TextView GA = new TextView(this);

                tmName.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                tmName.Text = teamList[i].teamName;
                tmName.SetPadding(2, 2, 2, 2);
                GP.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                GP.Text = teamList[i].gamesPlayed.ToString();
                GP.SetPadding(2, 2, 2, 2);
                wins.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                wins.Text = teamList[i].wins.ToString();
                wins.SetPadding(2, 2, 2, 2);
                losses.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                losses.Text = teamList[i].losses.ToString();
                losses.SetPadding(2, 2, 2, 2);
                ties.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                ties.Text = teamList[i].ties.ToString();
                ties.SetPadding(2, 2, 2, 2);
                GF.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                GF.Text = teamList[i].goalsFor.ToString();
                GF.SetPadding(2, 2, 2, 2);
                GA.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                GA.Text = teamList[i].goalsAgainst.ToString();
                GA.SetPadding(2, 2, 2, 2);

                row.AddView(tmName);
                row.AddView(GP);
                row.AddView(wins);
                row.AddView(losses);
                row.AddView(ties);
                row.AddView(GF);
                row.AddView(GA);
                
                lgTable.AddView(row);

            }

            lgTable.Visibility = ViewStates.Gone;
            lgTable.Visibility = ViewStates.Visible;

            prog.Dismiss();
        }

    }
}