using System;

namespace trafficserver
{
	public class TrafficData
	{
		private int _carsWaiting = 0;
		private int _lightColour = 0; // 0 = Red, 1 = Amber, 2 = Red
		bool _editTrigger = false;

		public TrafficData()
		{
		}

		public int CarsWaiting
		{
			get { return this._carsWaiting; }
			set { this._carsWaiting = value; }
		}

		public int LightColour
		{
			get { return this._lightColour; }
			set { this._lightColour = value; }
		}

		public bool EditTrigger
		{
			get { return this._editTrigger; }
			set { this._editTrigger = value; }
		}
	}
}

