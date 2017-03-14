using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace smilik
{
	[Activity(Label = "Смайлик Курьер", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : GeoManager
	{
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate { 
				TextView login = FindViewById<TextView>(Resource.Id.editText1);
				TextView password = FindViewById<TextView>(Resource.Id.editText2);

				
				var progressDialog = ProgressDialog.Show(this, "Подождите...", "Осуществляется вход...", true);
				try
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
								driver = Convert.ToInt32(login.Text),
								lattitude = _location.Latitude.ToString(),
								longitude = _location.Longitude.ToString()
							};
							var json = JsonConvert.SerializeObject(geo);
							string myParameters = json;
							wc.Headers["Content-Type"] = "application/json";
							wc.UploadString(URL, myParameters);
						}
					}
					else
					{
						AlertDialog.Builder alert = new AlertDialog.Builder(this);
						alert.SetTitle("Проблемы с подключением");
						alert.SetMessage("Не удается получить местоположение");
						alert.SetPositiveButton("ОК", (senderAlert, args) =>
						{
						});

						RunOnUiThread(() =>
						{
							alert.Show();
							RunOnUiThread(() => progressDialog.Hide());
						});
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
				catch(Exception e)
				{
					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Проблемы с подключением");
					alert.SetMessage(e.ToString());
					alert.SetPositiveButton("ОК", (senderAlert, args) =>
					{
					});

					RunOnUiThread(() =>
					{
						alert.Show();
						RunOnUiThread(() => progressDialog.Hide());
					});
				}
			};
		}
	}
}

