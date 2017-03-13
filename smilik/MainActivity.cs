using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Android.Locations;
using Plugin.Geolocator;

namespace smilik
{
	[Activity(Label = "Смайлик Курьер", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate { 
				TextView login = FindViewById<TextView>(Resource.Id.editText1);
				TextView password = FindViewById<TextView>(Resource.Id.editText2);

				var progressDialog = ProgressDialog.Show(this, "Подождите...", "Осуществляется вход...", true);
				try
				{
					new Thread(new ThreadStart(delegate
				{	var locator = CrossGeolocator.Current;
					locator.DesiredAccuracy = 50;
					var gps = locator.GetPositionAsync(timeoutMilliseconds: 15000);
					gps.Wait();
					var position = gps.Result;
					string URL = "http://13.65.148.113/api/Geopositions";
					using (var wc = new System.Net.WebClient())
					{
						Geoposition geo = new Geoposition();
						//geo.geoid = Guid.NewGuid();
						geo.driver = Convert.ToInt32(login.Text);
						geo.lattitude = position.Latitude.ToString();
						geo.longitude = position.Longitude.ToString();
						var json = JsonConvert.SerializeObject(geo);
						string myParameters = json;
						wc.Headers["Content-Type"] = "application/json";
						string HtmlResult = wc.UploadString(URL, myParameters);
					}
					
					URL = "http://13.65.148.113/api/Driver/" + login.Text;

					using (var webClient = new System.Net.WebClient())
					{
						var json = webClient.DownloadString(URL);
						User user = JsonConvert.DeserializeObject<User>(json);
						if (user != null && user.login.ToString() == login.Text && user.password == password.Text)
						{
							var activity2 = new Intent(this, typeof(UserActivity));
							activity2.PutExtra("login", login.Text);
							activity2.PutExtra("name", user.name);
							StartActivity(activity2);
						}
						else
						{
							AlertDialog.Builder alert = new AlertDialog.Builder(this);
							alert.SetTitle("Ошибка");
							alert.SetMessage("Авторизоваться не удалось");
							alert.SetPositiveButton("ОК", (senderAlert, args) =>
							{
							});

							RunOnUiThread(() =>
							{
								alert.Show();
							});
						}
					}
					//HIDE PROGRESS DIALOG
					RunOnUiThread(() => progressDialog.Hide());
				})).Start();


				}
				catch
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Проблемы с подключением");
					alert.SetMessage("Для правильной работы приложения необходимо включить геолокацию и доступ в интернет");
					alert.SetPositiveButton("ОК", (senderAlert, args) =>
					{
					});

					RunOnUiThread(() =>
					{
						alert.Show();
						RunOnUiThread(() => progressDialog.Hide());
					});
				}


					// Now parse with JSON.Net
			};
		}
	}
}

