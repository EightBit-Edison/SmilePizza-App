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
using Android.Graphics.Drawables;
using System.Threading;

namespace smilik
{
	public class OrderListAdapter: BaseAdapter
	{
		Activity context;
		public List<Order> items;

		public OrderListAdapter(Fragment context, List<Order> items): base()
    {
			this.context = context.Activity;
			//For demo purposes we hard code some data here
			this.items =items;
		}

		public override int Count
		{
			get { return items.Count; }
		}
		public override Java.Lang.Object GetItem(int position)
		{
			return position;
		}
		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			//Get our object for this position
			var item = items[position];
			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// This gives us some performance gains by not always inflating a new view
			// This will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = (
			  context.LayoutInflater.Inflate(
				            Resource.Layout.OrderItemLayout,
					null,
				false)) as LinearLayout;
			//Find references to each subview in the list item's view
			//var imageItem = view.FindViewById(Resource.id.imageItem) as ImageView;
			var textTop = view.FindViewById(Resource.Id.orderid) as TextView;
			var textBottom = view.FindViewById(Resource.Id.orderaddress) as TextView;
			Button button = view.FindViewById<Button>(Resource.Id.button1);

			button.Click += delegate
			{
				CloseOrder(item.id, item.phone);
			};

			Button button2 = view.FindViewById<Button>(Resource.Id.button2);

			button2.Click += delegate
			{

				var geoUri = Android.Net.Uri.Parse("google.navigation:q=" + remSkob(item.address));

				var mapIntent = new Intent(Intent.ActionView, geoUri);
				context.StartActivity(mapIntent);
			};
				//Assign this item's values to the various subviews
				//imageItem.SetImageResource(item.Image);
			textTop.SetText(item.number, TextView.BufferType.Normal);
			textBottom.SetText(item.address, TextView.BufferType.Normal);
			//Finally return the view
			return view;
		}
		private string remSkob(string input)
		{
			char[] arr = input.ToCharArray();
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] == '(')
				{
					int k = i + 1;
					while (arr[k] != ')')
					{
						arr[k] = '\0';
						k++;
					}
				}
			}
			string result = "";
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] != '\0')
				{
					result += arr[i];
				}
			}
			return result;
		}

		public Order GetItemAtPosition(int position)
		{
			return items[position];
		}
		public void CloseOrder(string id, string phone) { 

			/*new Thread(new ThreadStart(delegate
			{

				string URL = "http://13.65.148.113/api/Close/" + id + "/" + phone;

				using (var webClient = new System.Net.WebClient())
				{
					var json = webClient.DownloadString(URL);
				}
			})).Start();*/


			var builder = new AlertDialog.Builder(this.context);
			var dialogView = context.LayoutInflater.Inflate(Resource.Layout.DialogLayout, null);
			builder.SetView(dialogView);
			builder.SetTitle("Завершить заказ");


			builder.SetPositiveButton(Android.Resource.String.Ok, (sender, e) =>
			{
				var text = dialogView.FindViewById<EditText>(Resource.Id.editText1);
				string s = id.Substring(id.Length - 4);
				int i = Convert.ToInt32(s);
				int code = 9999 - i;
				if (code.ToString() == text.Text)
				{
					new Thread(new ThreadStart(delegate
					{
						string URL = "http://13.65.148.113/api/Finish/" + id;

						using (var webClient = new System.Net.WebClient())
						{
							var json = webClient.DownloadString(URL);
						}
					})).Start();
					Toast.MakeText(this.context, "OK", ToastLength.Short).Show();
				}
				else { 
					Toast.MakeText(this.context, "Неправильный код", ToastLength.Short).Show();
				}
			});

			builder.Show();


		}
	}
}
