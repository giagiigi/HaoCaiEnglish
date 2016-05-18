using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Request;
using System;
using System.Runtime.CompilerServices;

namespace Hishop.Weixin.MP.Request.Event
{
	public class LocationEventRequest : EventRequest
	{
		public override RequestEventType Event
		{
			get
			{
				return RequestEventType.Location;
			}
			set
			{
			}
		}

		public float Latitude
		{
			get;
			set;
		}

		public float Longitude
		{
			get;
			set;
		}

		public float Precision
		{
			get;
			set;
		}

		public LocationEventRequest()
		{
		}
	}
}