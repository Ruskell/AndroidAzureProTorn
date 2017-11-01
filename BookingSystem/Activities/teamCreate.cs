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
using BookingSystem.Classes;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;

namespace BookingSystem.Activities
{
    [Activity(Label = "Create New Team")]
    public class teamCreate : Activity
    {
        static IMobileServiceTable<Leagues> leaguesTable;
        static IMobileServiceTable<Teams> teamsTable;
        static IMobileServiceTable<PlayerTeams> playerTeamTable;
        static MobileServiceClient MobileService;
        Leagues lg;
        Spinner lgSpinner;
        ProgressDialog prog;
        List<Leagues> leagueList;
        Teams team;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TeamCreateView);

            prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetMessage("Analysing Schematics...");
            prog.SetCancelable(false);
            prog.Show();
            
            lgSpinner = FindViewById<Spinner>(Resource.Id.spnLeague);
            MobileService = MainActivity.MobileService;

            populateLeagues();

            FindViewById<Button>(Resource.Id.btnCreateTeam).Click += (sender, e) => { createTeam(); };
            
        }

        private async Task createTeam()
        {
            prog.SetMessage("Creating Team...");
            prog.Show();

            teamsTable = MobileService.GetTable<Teams>();

            team = new Teams
            {
                teamName = FindViewById<EditText>(Resource.Id.teamName).Text,
                teamOwner = MainActivity.user.id,
                leagueID = leagueList.ElementAt(lgSpinner.SelectedItemPosition).id
            };

            //Get a list of all teams owned by user
            List<Teams> teamList = await teamsTable.Where(team => team.teamOwner == MainActivity.user.id).ToListAsync();



            //set the int value to the position in the leaguelist where the user already has a team in the requested league
            int teamFound = 0;
            for (int i=0; i!=teamList.Count; i++)
            {
                if (teamList[i].leagueID == leagueList[lgSpinner.SelectedItemPosition].id) teamFound = i;
            }

            try
            {
                if (teamFound > 0) throw new Exception("You already have a team in this league: \n" + teamList[teamFound].teamName);
                if (FindViewById<EditText>(Resource.Id.teamName).Text == "") throw new Exception("Please enter a team name.");
            }
            catch (Exception ex)
            {
                prog.Dismiss();
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error!");
                alert.SetMessage(ex.Message + " ");
                alert.Show();
                return;
            }
            //update DB with new team
            await teamsTable.InsertAsync(team);

            playerTeamTable = MobileService.GetTable<PlayerTeams>();

            PlayerTeams pt = new PlayerTeams
            {
                playerID = MainActivity.user.id,
                teamID = team.id
            };

            await playerTeamTable.InsertAsync(pt);

            prog.Dismiss();

            await showDialog();

            StartActivity(typeof(teamManager));

        }
        
        //Dialog used for awaiting input before continuing
        public Task showDialog()
        {
            var tcs = new TaskCompletionSource<bool>();
            AlertDialog.Builder al = new AlertDialog.Builder(this);
            al.SetTitle("Radical!");
            al.SetMessage("Team Created");
            al.SetPositiveButton("OK", (sender, args) =>
            {
                tcs.SetResult(true);
            });
            al.Show();
            return tcs.Task;
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
    }
}