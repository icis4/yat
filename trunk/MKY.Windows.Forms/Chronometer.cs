using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MKY.Utilities.Event;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public partial class Chronometer : Component
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private TimeSpan _accumulatedTimeSpan = TimeSpan.Zero;
		private DateTime _startTimeStamp = DateTime.Now;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Action")]
		[Description("Event raised the tick interval elapsed or the time span was reset.")]
		public event EventHandler TimeSpanChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public Chronometer()
		{
			InitializeComponent();
		}

		/// <summary></summary>
		public Chronometer(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[DefaultValue(100)]
		public int Interval
		{
			get { return (timer_Chronometer.Interval); }
			set { timer_Chronometer.Interval = value;  }
		}

		/// <summary></summary>
		public TimeSpan TimeSpan
		{
			get
			{
				if (!timer_Chronometer.Enabled)
					return (_accumulatedTimeSpan);
				else
					return (_accumulatedTimeSpan + (DateTime.Now - _startTimeStamp));
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public void Start()
		{
			if (!timer_Chronometer.Enabled)
			{
				timer_Chronometer.Start();
				_startTimeStamp = DateTime.Now;
				OnTimeSpanChanged(new EventArgs());
			}
		}

		/// <summary></summary>
		public void Stop()
		{
			if (timer_Chronometer.Enabled)
			{
				timer_Chronometer.Stop();
				_accumulatedTimeSpan += (DateTime.Now - _startTimeStamp);
				OnTimeSpanChanged(new EventArgs());
			}
		}

		/// <summary></summary>
		public void StartStop()
		{
			if (!timer_Chronometer.Enabled)
				Start();
			else
				Stop();
		}

		/// <summary></summary>
		public void Reset()
		{
			_startTimeStamp = DateTime.Now;
			_accumulatedTimeSpan = TimeSpan.Zero;
			OnTimeSpanChanged(new EventArgs());
		}

		/// <summary></summary>
		public void Restart()
		{
			Stop();
			Reset();
			Start();
		}

		/// <summary></summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			TimeSpan ts = TimeSpan;

			sb.Insert(0, (ts.Milliseconds/10).ToString("D2"));
			sb.Insert(0, ".");
			sb.Insert(0, ts.Seconds.ToString("D2"));
			sb.Insert(0, ":");
			sb.Insert(0, ts.Minutes.ToString());
			if (ts.Hours > 0)
			{
				sb.Insert(0, ":");
				sb.Insert(0, ts.Hours.ToString());

				if (ts.Days > 0)
				{
					sb.Insert(0, "days ");
					sb.Insert(0, ts.Days.ToString());
				}
			}

			return (sb.ToString());
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void timer_Chronometer_Tick(object sender, EventArgs e)
		{
			OnTimeSpanChanged(new EventArgs());
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTimeSpanChanged(EventArgs e)
		{
			EventHelper.FireSync(TimeSpanChanged, this, e);
		}

		#endregion
	}
}
