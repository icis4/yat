using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MKY.Utilities.Event;

namespace MKY.Utilities.Time
{
	/// <summary></summary>
	public class Chronometer : IDisposable
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _isDisposed;

		private System.Timers.Timer _timer;

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
			_timer = new System.Timers.Timer();
			_timer.AutoReset = true;
			_timer.Interval = 1000;
			_timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
		}

		#region Disposal
		//------------------------------------------------------------------------------------------
		// Disposal
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary></summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
				{
					if (_timer != null)
						_timer.Dispose();
				}
				_isDisposed = true;
			}
		}

		/// <summary></summary>
		~Chronometer()
		{
			Dispose(false);
		}

		/// <summary></summary>
		protected bool IsDisposed
		{
			get { return (_isDisposed); }
		}

		/// <summary></summary>
		protected void AssertNotDisposed()
		{
			if (_isDisposed)
				throw (new ObjectDisposedException(GetType().ToString(), "Object has already been disposed"));
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public double Interval
		{
			get { AssertNotDisposed(); return (_timer.Interval); }
			set { AssertNotDisposed(); _timer.Interval = value;  }
		}

		/// <summary></summary>
		public TimeSpan TimeSpan
		{
			get
			{
				AssertNotDisposed();

				if (!_timer.Enabled)
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
			AssertNotDisposed();

			if (!_timer.Enabled)
			{
				_timer.Start();
				_startTimeStamp = DateTime.Now;
				OnTimeSpanChanged(new EventArgs());
			}
		}

		/// <summary></summary>
		public void Stop()
		{
			AssertNotDisposed();

			if (_timer.Enabled)
			{
				_timer.Stop();
				_accumulatedTimeSpan += DateTime.Now - _startTimeStamp;
				OnTimeSpanChanged(new EventArgs());
			}
		}

		/// <summary></summary>
		public void StartStop()
		{
			if (!_timer.Enabled)
				Start();
			else
				Stop();
		}

		/// <summary></summary>
		public void Reset()
		{
			AssertNotDisposed();

			_startTimeStamp = DateTime.Now;
			_accumulatedTimeSpan = TimeSpan.Zero;
			OnTimeSpanChanged(new EventArgs());
		}

		/// <summary></summary>
		public void Restart()
		{
			AssertNotDisposed();

			Stop();
			Reset();
			Start();
		}

		/// <summary></summary>
		public override string ToString()
		{
			AssertNotDisposed();

			StringBuilder sb = new StringBuilder();
			TimeSpan ts = TimeSpan;

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

		#region Timer Event Handlers
		//==========================================================================================
		// Timer Event Handlers
		//==========================================================================================

		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
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
