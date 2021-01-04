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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	internal class MonitorDataStatusHelper
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		private const Domain.RepositoryType RepositoryTypeDefault = Domain.RepositoryType.None;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private Domain.RepositoryType repositoryType = RepositoryTypeDefault;

		private Model.BytesLinesTuple counts = new Model.BytesLinesTuple(); // = { 0, 0, 0, 0 };
		private Model.BytesLinesTuple rates  = new Model.BytesLinesTuple(); // = { 0, 0, 0, 0 };

		private string txStatusText;
		private string rxStatusText;

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
		public MonitorDataStatusHelper()
		{
			SetStatusText();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		public virtual Domain.RepositoryType RepositoryType
		{
			get { return (this.repositoryType); }
			set
			{
				if (this.repositoryType != value)
				{
					this.repositoryType = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual Model.BytesLinesTuple Counts
		{
			get { return (this.counts); }
			set
			{
				if (this.counts != value)
				{
					this.counts = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual Model.BytesLinesTuple Rates
		{
			get { return (this.rates); }
			set
			{
				if (this.rates != value)
				{
					this.rates = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual string TxStatusText
		{
			get { return (this.txStatusText); }
		}

		/// <summary></summary>
		public virtual string RxStatusText
		{
			get { return (this.rxStatusText); }
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void SetCounts(Model.BytesLinesTuple counts)
		{
			this.counts = counts;

			SetStatusText();
		}

		/// <summary></summary>
		public virtual void SetRates(Model.BytesLinesTuple rates)
		{
			this.rates = rates;

			SetStatusText();
		}

		/// <summary></summary>
		public virtual void SetCountsAndRates(Model.CountsRatesTuple status)
		{
			this.counts = status.Counts;
			this.rates  = status.Rates;

			SetStatusText();
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			this.counts.Reset();
			this.rates .Reset();

			SetStatusText();
		}

		private void SetStatusText()
		{
			if (RepositoryType != Domain.RepositoryType.None)
			{
				switch (RepositoryType)
				{
					case Domain.RepositoryType.Tx:    SetTxStatusText();                    break;
					case Domain.RepositoryType.Bidir: SetTxStatusText(); SetRxStatusText(); break;
					case Domain.RepositoryType.Rx:                       SetRxStatusText(); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + RepositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				OnStatusTextChanged(EventArgs.Empty);
			}
		}

		private void SetTxStatusText()
		{
			var sb = new StringBuilder();

			sb.Append(this.counts.TxBytes);
			sb.Append(" | ");
			sb.Append(this.counts.TxLines);
			sb.Append(" @ ");
			sb.Append(this.rates.TxBytes);
			sb.Append("/s");  // " B/s" is not really readable, compare 1024/s vs. 1024 B/s,
			sb.Append(" | "); //   and consider that the values may be flickering...
			sb.Append(this.rates.TxLines);
			sb.Append("/s");

			this.txStatusText = sb.ToString();
		}

		private void SetRxStatusText()
		{
			var sb = new StringBuilder();

			sb.Append(this.counts.RxBytes);
			sb.Append(" | ");
			sb.Append(this.counts.RxLines);
			sb.Append(" @ ");
			sb.Append(this.rates.RxBytes);
			sb.Append("/s");
			sb.Append(" | ");
			sb.Append(this.rates.RxLines);
			sb.Append("/s");

			this.rxStatusText = sb.ToString();
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
