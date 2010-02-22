//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using MKY.Utilities.Event;
using MKY.Utilities.Time;

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
		public event EventHandler<TimeSpanEventArgs> TimeSpanChanged;

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
				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		public void Stop()
		{
			if (timer_Chronometer.Enabled)
			{
				timer_Chronometer.Stop();
				_accumulatedTimeSpan += (DateTime.Now - _startTimeStamp);
				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
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
			OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan.Zero));
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
			return (XTimeSpan.FormatTimeSpan(TimeSpan, true));
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		private void timer_Chronometer_Tick(object sender, EventArgs e)
		{
			OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTimeSpanChanged(TimeSpanEventArgs e)
		{
			EventHelper.FireSync<TimeSpanEventArgs>(TimeSpanChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
