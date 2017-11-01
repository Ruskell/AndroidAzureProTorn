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
    [Activity(Label = "Manage a Team")]
    public class teamManager : Activity
    {
        static IMobileServiceTable<Teams> teamsTable;
        static IMobileServiceTable<Requests> reqsTable;
        static IMobileServiceTable<Players> playerTable;
        static IMobileServiceTable<PlayerTeams> playerTeamTable;
        static MobileServiceClient MobileService;

        List<Teams> teamList;
        List<Requests> reqList;
        List<Players> playerList;
        List<Button> acceptList = new List<Button>();
        List<Button> declineList = new List<Button>();

        Teams team;
        ProgressDialog prog;
        Spinner tmSpinner;
        TableLayout tabReqs;

        AlertDialog.Builder alert;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.TeamManageView);

            alert = new AlertDialog.Builder(this);
            prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetMessage("Loading Teams...");
            prog.SetCancelable(false);
            prog.Show();

            MobileService = MainActivity.MobileService;
            tmSpinner = FindViewById<Spinner>(Resource.Id.spnTeam);
            tabReqs = FindViewById<TableLayout>(Resource.Id.tabLayRequests);
            populateTeams();
            tmSpinner.ItemSelected += (sender, e) =>
            {
                populateReqs();
            };

            FindViewById<Button>(Resource.Id.btnCreateTeam).Click += (sender, e) =>
            {
                StartActivity(typeof(teamCreate));
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

        private async Task populateTeams()
        {
            teamsTable = MobileService.GetTable<Teams>();

            team = new Teams
            {
                teamOwner = MainActivity.user.id
            };

            teamList = await teamsTable.Where(team => team.teamOwner == MainActivity.user.id).ToListAsync();
            string[] strTeam;
            if(teamList.Count < 1)
            {
                strTeam = new string[1];
                strTeam[0] = "You are the manager of NO teams! LOL";
            }
            else
            {
                strTeam = new string[teamList.Count];
                for (int i=0; i!=teamList.Count; i++)
                {
                    strTeam[i] = teamList[i].teamName;
                }
            }

            ArrayAdapter<String> tmAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, strTeam);
            tmAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            tmSpinner.Adapter = tmAdapter;
            prog.Dismiss();
        }

        private async Task populateReqs()
        {
            prog.SetMessage("Populating Requests");
            prog.Show();
            if (teamList.Count < 1)
            {
                prog.Dismiss();
                return;
            }
            reqsTable = MobileService.GetTable<Requests>();
            playerTable = MobileService.GetTable<Players>();

            reqList = await reqsTable.Where(req => req.teamReqID == teamList[tmSpinner.SelectedItemPosition].id && !req.deleted).ToListAsync();


            //Clear existing data
            if (tabReqs.ChildCount > 1)
            {
                for (int i = tabReqs.ChildCount; i != 1; i--)
                {
                    tabReqs.RemoveViewAt(i - 1);
                }
            }

            tabReqs.Visibility = ViewStates.Gone;
            tabReqs.Visibility = ViewStates.Visible;

            for (int i = 0; i != reqList.Count; i++)
            {
                playerList = await playerTable.Where(pl => pl.id == reqList[i].playerReqID).ToListAsync();

                TableRow tr = new TableRow(this);
                TextView name = new TextView(this);
                Button acc = new Button(this);
                Button dec = new Button(this);
                
                name.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent, 1);
                name.SetPadding(4, 2, 2, 2);
                try
                {
                    name.Text = playerList[0].FirstName + " " + playerList[0].LastName;
                }
                catch
                {
                    name.Text = "NULL PLAYER";
                }
                acc.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);
                dec.LayoutParameters = new TableRow.LayoutParams(TableRow.LayoutParams.MatchParent, TableRow.LayoutParams.WrapContent);

                acc.Click += Acc_Click;
                acceptList.Add(acc);

                dec.Click += Dec_Click;
                declineList.Add(dec);

                tr.AddView(name);
                tr.AddView(acc);
                tr.AddView(dec);

                tabReqs.AddView(tr);
            }
            prog.Dismiss();
        }

        private void Dec_Click(object sender, EventArgs e)
        {
            Button butt = sender as Button;
            int decSelectedButton = -1;
            for (int i = 0; i != declineList.Count; i++)
            {
                if (butt == declineList[i]) decSelectedButton = i;
            }

            prog.SetMessage("Declining Request");
            prog.Show();
            decProcess(decSelectedButton);

        }

        async Task decProcess(int index)
        {
            try
            {
                //reqList[index].deleted = true;
                //await reqsTable.UpdateAsync(reqList[index]);
                await reqsTable.DeleteAsync(reqList[index]);
            }
            catch(Exception e)
            {
                alert.SetTitle("Error!");
                alert.SetMessage(e.Message + " ");
                prog.Dismiss();
                alert.Show();
            }
            finally
            {
                alert.SetTitle("Success!");
                alert.SetMessage("Request Declined");
                populateReqs();
                prog.Dismiss();
                alert.Show();
                
            }

        }

        private void Acc_Click(object sender, EventArgs e)
        {
            Button butt = sender as Button;

            int accSelectedButton = -1;
            for (int i = 0; i != acceptList.Count; i++)
            {
                if (butt == acceptList[i]) accSelectedButton = i;
            }

            prog.SetMessage("Accepting Request");
            prog.Show();
            accProcess(accSelectedButton);
        }

        async Task accProcess(int index)
        {
            try {

                //reqList[index].deleted = true;

                playerTeamTable = MobileService.GetTable<PlayerTeams>();

                PlayerTeams pt = new PlayerTeams
                {
                    playerID = reqList[index].playerReqID,
                    teamID = reqList[index].teamReqID
                };

                await playerTeamTable.InsertAsync(pt);
                await reqsTable.DeleteAsync(reqList[index]);
            }
            catch (Exception e)
            {
                alert.SetTitle("Error!");
                alert.SetMessage(e.Message + " ");
                prog.Dismiss();
                alert.Show();
            }
            finally
            {
                alert.SetTitle("Success!");
                alert.SetMessage("Request Accepted");
                populateReqs();
                prog.Dismiss();
                alert.Show();
            }
        }
    }
}