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

namespace BookingSystem.Activities
{
    [Activity(Label = "Register")]
    public class regView : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.RegView);

            Spinner spnGen = FindViewById<Spinner>(Resource.Id.spnGender);
            var genAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.genSpinner, Android.Resource.Layout.SimpleSpinnerDropDownItem);
            genAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spnGen.Adapter = genAdapter;

            Button btnCont = FindViewById<Button>(Resource.Id.btnCont);
            btnCont.Click += BtnCont_Click;


        }

        private void BtnCont_Click(object sender, EventArgs e)
        {

            ProgressDialog prog = new ProgressDialog(this);
            prog.Indeterminate = true;
            prog.SetProgressStyle(ProgressDialogStyle.Spinner);
            prog.SetMessage(MainActivity.user.Gender = FindViewById<Spinner>(Resource.Id.spnGender).SelectedItem.ToString());
            prog.SetCancelable(false);
            prog.Show();

            try
            {
                if (FindViewById<EditText>(Resource.Id.firstName).Text == "") throw new Exception("Please enter a first name.");
                if (FindViewById<EditText>(Resource.Id.mobNo).Text == "") throw new Exception("Please enter a 10 digit mobile number.");
                if (Convert.ToInt32(FindViewById<EditText>(Resource.Id.mobNo).Text) == 0) throw new Exception("Please enter a 10 digit mobile number.");
                if (FindViewById<EditText>(Resource.Id.mobNo).Text.Length < 10) throw new Exception("Please enter a 10 digit mobile number.");
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

            MainActivity.user.FirstName = FindViewById<EditText>(Resource.Id.firstName).Text;
            MainActivity.user.LastName = FindViewById<EditText>(Resource.Id.lastName).Text;
            MainActivity.user.Mobile = Convert.ToInt32(FindViewById<EditText>(Resource.Id.mobNo).Text);
            MainActivity.user.Gender = FindViewById<Spinner>(Resource.Id.spnGender).SelectedItem.ToString();
            MainActivity.user.DateofBirth = getDate();

            MainActivity.updateUser();
            prog.Dismiss();

            StartActivity(typeof(mainMenu));
        }

        private string getDate()
        {
            int day = FindViewById<DatePicker>(Resource.Id.datePicker).DayOfMonth;
            int month = FindViewById<DatePicker>(Resource.Id.datePicker).Month;
            int year = FindViewById<DatePicker>(Resource.Id.datePicker).Year;
            return (day + "-" + month + "-" + year);
        }
    }
}