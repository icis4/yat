//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary></summary>
	public partial class Chronometer : Component
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private TimeSpan accumulatedTimeSpan = TimeSpan.Zero;
		private DateTime startTimeStamp = DateTime.Now;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		[Category("Property Changed")]
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

		/// <remarks>
		/// No explicit 'Default' constant as the default is given by <see cref="Timer"/>.
		/// </remarks>
		[DefaultValue(100)]
		public virtual int Interval
		{
			get { return (timer_Chronometer.Interval); }
			set { timer_Chronometer.Interval = value;  }
		}

		/// <summary></summary>
		public virtual TimeSpan TimeSpan
		{
			get
			{
				if (!timer_Chronometer.Enabled)
					return (this.accumulatedTimeSpan);
				else
					return (this.accumulatedTimeSpan + (DateTime.Now - this.startTimeStamp));
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Start()
		{
			if (!timer_Chronometer.Enabled)
			{
				timer_Chronometer.Start();
				this.startTimeStamp = DateTime.Now;
				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "'Stop' is a common term to start/stop something.")]
		public virtual void Stop()
		{
			if (timer_Chronometer.Enabled)
			{
				timer_Chronometer.Stop();
				this.accumulatedTimeSpan += (DateTime.Now - this.startTimeStamp);
				OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
			}
		}

		/// <summary></summary>
		public virtual void StartStop()
		{
			if (!timer_Chronometer.Enabled)
				Start();
			else
				Stop();
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			this.startTimeStamp = DateTime.Now;
			this.accumulatedTimeSpan = TimeSpan.Zero;
			OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan.Zero));
		}

		/// <summary></summary>
		public virtual void Restart()
		{
			Stop();
			Reset();
			Start();
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields. This ensures that 'intelligent' properties,
		/// i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override string ToString()
		{
			return (TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(TimeSpan));
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_Chronometer_Tick(object sender, EventArgs e)
		{
			OnTimeSpanChanged(new TimeSpanEventArgs(TimeSpan));
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTimeSpanChanged(TimeSpanEventArgs e)
		{
			EventHelper.RaiseSync<TimeSpanEventArgs>(TimeSpanChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
