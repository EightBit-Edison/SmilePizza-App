using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Newtonsoft.Json;
using ILocationListener = Android.Gms.Location.ILocationListener;

namespace smilik
{
	public abstract class GeoManager: Activity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, ILocationListener
	{
		protected Location _location;
		GoogleApiClient googleApiClient;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			googleApiClient = new GoogleApiClient.Builder(this)
				.AddApi(Android.Gms.Location.LocationServices.API)
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.Build();
			googleApiClient.Connect();
		}

		public async void OnConnected(Bundle connectionHint)
		{
			// Get Last known location
			var lastLocation = LocationServices.FusedLocationApi.GetLastLocation(googleApiClient);

			if (lastLocation != null)
			{
				_location = lastLocation;
			}

			await RequestLocationUpdates();
		}

		async Task RequestLocationUpdates()
		{
			// Describe our location request
			var locationRequest = new LocationRequest()
				.SetInterval(10000)
				.SetFastestInterval(1000)
				.SetPriority(LocationRequest.PriorityHighAccuracy);

			// Check to see if we can request updates first
			if (await CheckLocationAvailability(locationRequest))
			{

				// Request updates
				await LocationServices.FusedLocationApi.RequestLocationUpdates(googleApiClient,
					locationRequest,
					this);
			}
		}

		async Task<bool> CheckLocationAvailability(LocationRequest locationRequest)
		{
			// Build a new request with the given location request
			var locationSettingsRequest = new LocationSettingsRequest.Builder()
				.AddLocationRequest(locationRequest)
				.Build();

			// Ask the Settings API if we can fulfill this request
			var locationSettingsResult = await LocationServices.SettingsApi.CheckLocationSettingsAsync(googleApiClient, locationSettingsRequest);


			// If false, we might be able to resolve it by showing the location settings 
			// to the user and allowing them to change the settings
			if (!locationSettingsResult.Status.IsSuccess)
			{

				if (locationSettingsResult.Status.StatusCode == LocationSettingsStatusCodes.ResolutionRequired)
					locationSettingsResult.Status.StartResolutionForResult(this, 101);
				else
					Toast.MakeText(this, "Location Services Not Available for the given request.", ToastLength.Long).Show();

				return false;
			}

			return true;
		}

		public void OnConnectionSuspended(int cause)
		{
			Console.WriteLine("GooglePlayServices Connection Suspended: {0}", cause);
		}

		public void OnConnectionFailed(Android.Gms.Common.ConnectionResult result)
		{
			Console.WriteLine("GooglePlayServices Connection Failed: {0}", result);
		}

		public void OnLocationChanged(Android.Locations.Location location)
		{

			if (location != null)
			{
				_location = location;
			}
			else
			{
				Toast.MakeText(this, "Location Dot Determined", ToastLength.Long).Show();
			}
		}

		protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);

			// See if we returned from a location settings dialog 
			// and if succesfully, we can try location updates again
			if (requestCode == 101)
			{
				if (resultCode == Result.Ok)
					await RequestLocationUpdates();
				else
					Toast.MakeText(this, "Failed to resolve Location Settings changes", ToastLength.Long).Show();
			}
		}

	}
}