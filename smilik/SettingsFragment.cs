
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.Geolocator;

namespace smilik
{
	public class SettingsFragment : Fragment
	{
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);

			var view = inflater.Inflate(
				Resource.Layout.SettingsLayout, container, false);

			TextView text = view.FindViewById<TextView>(Resource.Id.textView1);
			text.Text = Arguments.GetString("name");

			
				    
			Button button = view.FindViewById<Button>(Resource.Id.button1);
			button.Click += delegate
			{
				Activity.Finish();
			};
			return view;

		}
	}
}
