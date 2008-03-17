using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MKY.Utilities.Event;

namespace YAT.Gui.Controls
{
	public partial class Chronometer : Component
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum State
		{
			Reset,
			Running,
			Stopped,
		}

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private State _state = State.Reset;
		private TimeSpan _accumulatedTimeSpan = TimeSpan.Zero;
		private DateTime _startTimeStamp = DateTime.Now;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		[Category("Action")]
		[Description("Event raised the tick interval elapsed.")]
		public event EventHandler Tick;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		public Chronometer()
		{
			InitializeComponent();
		}

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

		[DefaultValue(100)]
		public int Interval
		{
			get { return (timer_Chronometer.Interval); }
			set { timer_Chronometer.Interval = value;  }
		}

		public TimeSpan TimeSpan
		{
			get
			{
				switch (_state)
				{
					case State.Reset:
					case State.Stopped:
						return (_accumulatedTimeSpan);

					default:
						return (_accumulatedTimeSpan + (DateTime.Now - _startTimeStamp));
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		public void StartStop()
		{
			switch (_state)
			{
				case State.Reset:
				case State.Stopped:
					_startTimeStamp = DateTime.Now;
					_state = State.Running;
					break;

				default:
					_accumulatedTimeSpan += DateTime.Now - _startTimeStamp;
					_state = State.Stopped;
					break;
			}
		}

		public void Reset()
		{
			_accumulatedTimeSpan = TimeSpan.Zero;
			_startTimeStamp = DateTime.Now;
			_state = State.Reset;
		}

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
			OnTick(new EventArgs());
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnTick(EventArgs e)
		{
			EventHelper.FireSync(Tick, this, e);
		}

		#endregion
	}
}
