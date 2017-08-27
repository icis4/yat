﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Delta Version 1.99.80
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Text;

using MKY;

// This code is intentionally placed into the YAT.View.Controls namespace even though the file is
// located in YAT.View.Controls.ViewModel for same location as parent control.
namespace YAT.View.Controls
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
			var sb = new StringBuilder();

			sb.Append(TimeSpanEx.FormatInvariantTimeSpan(this.activeConnectTime));
			sb.Append(Environment.NewLine);
			sb.Append(TimeSpanEx.FormatInvariantTimeSpan(this.totalConnectTime));

			this.statusText = sb.ToString();

			OnStatusTextChanged(EventArgs.Empty);
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnStatusTextChanged(EventArgs e)
		{
			EventHelper.RaiseSync(StatusTextChanged, this, e);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
