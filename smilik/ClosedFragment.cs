
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
	public class ClosedFragment : Fragment
	{
		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)

		{
			
			var view = inflater.Inflate(
				Resource.Layout.ClosedActivity, container, false);

			var progressDialog = ProgressDialog.Show(this.Activity, "Подождите...", "Загрузка данных...", true);
			new Thread(new ThreadStart(delegate
			{
				LoadData(view);
				CheckFree();
				//HIDE PROGRESS DIALOG
				Activity.RunOnUiThread(() => progressDialog.Hide());
			})).Start();
			CountDown(view);
			return view;
		}

		private void CountDown(View view)
		{

			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 50000;
			timer.Elapsed += delegate
			{
				try
				{
					var locator = CrossGeolocator.Current;
					locator.DesiredAccuracy = 50;
					var gps = locator.GetPositionAsync(timeoutMilliseconds: 15000);
					gps.Wait();
					var position = gps.Result;
					string URL = "http://13.65.148.113/api/Geopositions";
					using (var wc = new System.Net.WebClient())
					{
						Geoposition geo = new Geoposition();
						//geo.geoid = Guid.NewGuid();
						geo.driver = Convert.ToInt32(Arguments.GetString("login"));
						geo.lattitude = position.Latitude.ToString();
						geo.longitude = position.Longitude.ToString();
						var json = JsonConvert.SerializeObject(geo);

						string myParameters = json;

						wc.Headers["Content-Type"] = "application/json";
						string HtmlResult = wc.UploadString(URL, myParameters);

					}
					LoadData(view);
					CheckFree();
				}
				catch
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(Activity);

					alert.SetTitle("Проблемы с подключением");
					alert.SetMessage("Для правильной работы приложения необходимо включить геолокацию и доступ в интернет");
					alert.SetPositiveButton("ОК", (senderAlert, args) =>
					{
					});

					Activity.RunOnUiThread(() =>
					{
						alert.Show();
					});
				}
			};
			timer.Enabled = true;


		}


		private void CheckFree()
		{
			string URL = "http://13.65.148.113/api/Orders/Empty";

			using (var webClient = new System.Net.WebClient())
			{
				var json = webClient.DownloadString(URL);
				List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);
				if (orders != null)
				{
					ClosedOrderListAdapter listAdapter = new ClosedOrderListAdapter(this, orders);
					if (listAdapter.Count > 0)
					{
						// Instantiate the builder and set notification elements:
						Notification.Builder builder = new Notification.Builder(Activity)
							.SetContentTitle("Смайлик Курьер")
							.SetContentText("У нас есть для тебя работа!")
							.SetSmallIcon(Android.Resource.Drawable.SymDefAppIcon);

						// Build the notification:
						Notification notification = builder.Build();

						// Get the notification manager:
						NotificationManager notificationManager =
							Activity.GetSystemService(Context.NotificationService) as NotificationManager;

						// Publish the notification:
						const int notificationId = 0;
						notificationManager.Notify(notificationId, notification);
					}

				}

			}
		}

		private void LoadData(View view)
		{
			string URL = "http://13.65.148.113/api/Orders/Closed/" + Arguments.GetString("login");

			using (var webClient = new System.Net.WebClient())
			{
				var json = webClient.DownloadString(URL);
				List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);
				if (orders != null)
				{
					ClosedOrderListAdapter listAdapter = new ClosedOrderListAdapter(this, orders);
					var listView = view.FindViewById<ListView>(Resource.Id.listView1);
					Activity.RunOnUiThread(() => listView.Adapter = listAdapter);

				}

			}
		}
	}
}
