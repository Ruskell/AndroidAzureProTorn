using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Microsoft.WindowsAzure.MobileServices;
using BookingSystem.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using BookingSystem.Activities;
using Android.Views;

namespace BookingSystem
{
    [Activity(Label = "ProTorn", MainLauncher = true, Icon = "@drawable/ic_logo")]
    public class MainActivity : Activity
    {
        public static MobileServiceClient MobileService;
        public static IMobileServiceTable<Players> playersTable;
        public static Players user;
        public static string email, pass;
        ProgressDialog prog;

        protected override void OnCreate(Bundle bundle)
        {
            prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetMessage("Initialising...");
            prog.SetCancelable(false);
            prog.Show();

            base.OnCreate(bundle);
            SetTheme(Resource.Style.customThemeNoBar);
            SetContentView (Resource.Layout.Main);

            CurrentPlatform.Init();

            MobileService = new MobileServiceClient("https://protornmobile.azurewebsites.net");

            playersTable = MobileService.GetTable<Players>();

            FindViewById<Button>(Resource.Id.btnLogin).Click += async (sender, e) =>
            {
                await login();
            };

            FindViewById<Button>(Resource.Id.btnRegister).Click += async (sender, e) =>
            {
                await createAccount();
                //var intent = new Intent(this, typeof(register));
                //StartActivity(intent);
            };
            prog.Dismiss();
        }

        public async Task login()
        {
            prog.SetMessage("Logging In...");
            prog.Show();

            email = FindViewById<EditText>(Resource.Id.etEmail).Text;
            pass = FindViewById<EditText>(Resource.Id.etPass).Text;
            user = new Players
            {
                EMailAddress = email,
                Password = pass
            };

            List<Players> players = await playersTable.Where(user => user.EMailAddress == email).ToListAsync();

            prog.Dismiss();

            try
            {
                if (email == "") throw new Exception("Please enter an email address.");
                if (pass == "") throw new Exception("Please enter a password.");
                if (players.Count < 1) throw new Exception("That account does not exist");
                if (pass != players[0].Password) throw new Exception("Please enter the correct password.");
            }
            catch (Exception e)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error!");
                alert.SetMessage(e.Message);
                alert.Show();
                return;
            }


            user = players[0];

            if(user.FirstName == "" || user.Mobile == 0) StartActivity(typeof(regView));
            else StartActivity(typeof(mainMenu));


        }

        public async Task createAccount()
        {
            email = FindViewById<EditText>(Resource.Id.etEmail).Text;
            pass = FindViewById<EditText>(Resource.Id.etPass).Text;
            user = new Players {
                EMailAddress = email,
                Password = pass
            };
            
            prog.SetMessage("Creating Account...");
            prog.Show();

            playersTable = MobileService.GetTable<Players>();
            List<Players> players = await playersTable.Where(user => user.EMailAddress == email).ToListAsync();


            try
            {
                if (email == "") throw new Exception("Please enter an email address.");
                if (pass == "") throw new Exception("Please enter a password.");
                if (players.Count > 0 && user.EMailAddress == players[0].EMailAddress) throw new Exception("An account already exists with that email address.");
            }
            catch (Exception e)
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Error!");
                alert.SetMessage(e.Message + " ");
                prog.Dismiss();
                alert.Show();
                return;
            }

            await playersTable.InsertAsync(user);
            prog.Dismiss();

            StartActivity(typeof(regView));

        }

        public static async void updateUser()
        {
            await playersTable.UpdateAsync(user);
        }

        public override void OnBackPressed()
        {
            FinishAffinity();
        }
    }

}

