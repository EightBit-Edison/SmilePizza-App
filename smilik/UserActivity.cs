
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



namespace smilik
{
	[Activity(Label = "Смайлик Курьер")]
	public class UserActivity : Activity
	{
		private static readonly int ButtonClickNotificationId = 1000;
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			string login = Intent.GetStringExtra("login") ?? "Data not available";
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

		}
	}
}
