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


namespace smilik
{
	public class ClosedOrderListAdapter : BaseAdapter
	{
		Activity context;
		public List<Order> items;

		public ClosedOrderListAdapter(Fragment context, List<Order> items) : base()
		{
			this.context = context.Activity;
			//For demo purposes we hard code some data here
			this.items = items;
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
							Resource.Layout.ClosedOrderItemLayout,
					null,
				false)) as LinearLayout;
			//Find references to each subview in the list item's view
			//var imageItem = view.FindViewById(Resource.id.imageItem) as ImageView;
			var textTop = view.FindViewById(Resource.Id.orderid) as TextView;
			var textBottom = view.FindViewById(Resource.Id.orderaddress) as TextView;
			//Assign this item's values to the various subviews
			//imageItem.SetImageResource(item.Image);
			textTop.SetText(item.number, TextView.BufferType.Normal);
			textBottom.SetText(item.address, TextView.BufferType.Normal);
			//Finally return the view
			return view;
		}

		public Order GetItemAtPosition(int position)
		{
			return items[position];
		}

	}
}
