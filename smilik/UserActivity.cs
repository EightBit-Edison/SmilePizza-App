
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;


namespace smilik
{
	[Activity(Label = "Смайлик Курьер")]
	public class UserActivity : GeoManager
	{
		private string login;
		private static readonly int ButtonClickNotificationId = 1000;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			login = Intent.GetStringExtra("login") ?? "Data not available";
			string name = Intent.GetStringExtra("name") ?? "Data not available";

			SetContentView(Resource.Layout.User);
			this.ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

			var tab = this.ActionBar.NewTab();
			tab.SetText("Активные");
			tab.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				Bundle args = new Bundle();
				ActiveFragment fr = new ActiveFragment();
				args.PutString("login", login);
				fr.Arguments = args;

				e.FragmentTransaction.Replace(Resource.Id.fragment_container, fr);
			};
			this.ActionBar.AddTab(tab);


			var tab2 = this.ActionBar.NewTab();
			tab2.SetText("Закрытые");
			tab2.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				Bundle args = new Bundle();
				ClosedFragment fr = new ClosedFragment();
				args.PutString("login", login);
				fr.Arguments = args;
				e.FragmentTransaction.Replace(Resource.Id.fragment_container, fr);

			};
			this.ActionBar.AddTab(tab2);

			var tab3 = this.ActionBar.NewTab();
			tab3.SetText("Настройки");
			tab3.TabSelected += delegate (object sender, ActionBar.TabEventArgs e)
			{
				Bundle args = new Bundle();
				SettingsFragment fr = new SettingsFragment();
				args.PutString("name", name);
				fr.Arguments = args;
				e.FragmentTransaction.Replace(Resource.Id.fragment_container, fr);
			};
			this.ActionBar.AddTab(tab3);

			CountDown();
		}

		private void CountDown()
		{

			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 50000;
			timer.Elapsed += delegate
			{
				UpdateLocation();
			};
			timer.Enabled = true;
		}

		public void UpdateLocation()
		{
			new Thread(new ThreadStart(delegate
			{
				string URL;
				if (_location != null)
				{
					URL = "http://13.65.148.113/api/Geopositions";
					using (var wc = new System.Net.WebClient())
					{
						Geoposition geo = new Geoposition
						{
							driver = Convert.ToInt32(login),
							lattitude = _location.Latitude.ToString(),
							longitude = _location.Longitude.ToString()
						};
						var json = JsonConvert.SerializeObject(geo);
						string myParameters = json;
						wc.Headers["Content-Type"] = "application/json";
						wc.UploadString(URL, myParameters);
					}
				}
			})).Start();

		}
	}
}
