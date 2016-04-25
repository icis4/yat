//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Text;

using MKY;

#endregion

namespace YAT.View.Controls.ViewModel
{
	/// <summary></summary>
	internal class MonitorTimeStatusHelper
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private TimeSpan activeConnectTime;
		private TimeSpan totalConnectTime;

		private string statusText;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler StatusTextChanged;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public MonitorTimeStatusHelper()
		{
			SetStatusText();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual TimeSpan ActiveConnectTime
		{
			get { return (this.activeConnectTime); }
			set
			{
				if (this.activeConnectTime != value)
				{
					this.activeConnectTime = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual TimeSpan TotalConnectTime
		{
			get { return (this.totalConnectTime); }
			set
			{
				if (this.totalConnectTime != value)
				{
					this.totalConnectTime = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual string StatusText
		{
			get { return (this.statusText); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void Set(TimeSpan activeConnectTime, TimeSpan totalConnectTime)
		{
			this.activeConnectTime = activeConnectTime;
			this.totalConnectTime  = totalConnectTime;

			SetStatusText();
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			this.activeConnectTime = TimeSpan.Zero;
			this.totalConnectTime  = TimeSpan.Zero;

			SetStatusText();
		}

		private void SetStatusText()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(TimeSpanEx.FormatInvariantTimeSpan(this.activeConnectTime));
			sb.Append(Environment.NewLine);
			sb.Append(TimeSpanEx.FormatInvariantTimeSpan(this.totalConnectTime));

			this.statusText = sb.ToString();

			OnStatusTextChanged(EventArgs.Empty);
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnStatusTextChanged(EventArgs e)
		{
			EventHelper.FireSync(StatusTextChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
