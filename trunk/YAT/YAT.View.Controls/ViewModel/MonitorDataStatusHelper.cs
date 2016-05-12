﻿//==================================================================================================
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

		private int txByteCount;
		private int rxByteCount;
		private int txLineCount;
		private int rxLineCount;

		private int txByteRate;
		private int rxByteRate;
		private int txLineRate;
		private int rxLineRate;

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
		public virtual int TxByteCount
		{
			get { return (this.txByteCount); }
			set
			{
				if (this.txByteCount != value)
				{
					this.txByteCount = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int RxByteCount
		{
			get { return (this.rxByteCount); }
			set
			{
				if (this.rxByteCount != value)
				{
					this.rxByteCount = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int TxLineCount
		{
			get { return (this.txLineCount); }
			set
			{
				if (this.txLineCount != value)
				{
					this.txLineCount = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int RxLineCount
		{
			get { return (this.rxLineCount); }
			set
			{
				if (this.rxLineCount != value)
				{
					this.rxLineCount = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int TxByteRate
		{
			get { return (this.txByteRate); }
			set
			{
				if (this.txByteRate != value)
				{
					this.txByteRate = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int RxByteRate
		{
			get { return (this.rxByteRate); }
			set
			{
				if (this.rxByteRate != value)
				{
					this.rxByteRate = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int TxLineRate
		{
			get { return (this.txLineRate); }
			set
			{
				if (this.txLineRate != value)
				{
					this.txLineRate = value;
					SetStatusText();
				}
			}
		}

		/// <summary></summary>
		public virtual int RxLineRate
		{
			get { return (this.rxLineRate); }
			set
			{
				if (this.rxLineRate != value)
				{
					this.rxLineRate = value;
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
		public virtual void SetCount(int txByteCount, int txLineCount, int rxByteCount, int rxLineCount)
		{
			this.txByteCount = txByteCount;
			this.txLineCount = txLineCount;
			this.rxByteCount = rxByteCount;
			this.rxLineCount = rxLineCount;

			SetStatusText();
		}

		/// <summary></summary>
		public virtual void SetRate(int txByteRate, int txLineRate, int rxByteRate, int rxLineRate)
		{
			this.txByteRate = txByteRate;
			this.txLineRate = txLineRate;
			this.rxByteRate = rxByteRate;
			this.rxLineRate = rxLineRate;

			SetStatusText();
		}

		/// <summary></summary>
		public virtual void Reset()
		{
			this.txByteCount = 0;
			this.txLineCount = 0;
			this.rxByteCount = 0;
			this.rxLineCount = 0;

			this.txByteRate = 0;
			this.txLineRate = 0;
			this.rxByteRate = 0;
			this.rxLineRate = 0;

			SetStatusText();
		}

		private void SetStatusText()
		{
			StringBuilder sb = new StringBuilder();
			switch (this.repositoryType)
			{
				case Domain.RepositoryType.Tx:
				{
					AppendTxStatus(sb);
					break;
				}
				case Domain.RepositoryType.Bidir:
				{
					AppendTxStatus(sb);
					sb.Append(Environment.NewLine);
					AppendRxStatus(sb);
					break;
				}
				case Domain.RepositoryType.Rx:
				{
					AppendRxStatus(sb);
					break;
				}
			}
			this.statusText = sb.ToString();

			OnStatusTextChanged(EventArgs.Empty);
		}

		private void AppendTxStatus(StringBuilder sb)
		{
			sb.Append(this.txByteCount);
			sb.Append(" | ");
			sb.Append(this.txLineCount);
			sb.Append(" @ ");
			sb.Append(this.txByteRate);
			sb.Append("/s");  // " B/s" is not really readable, compare 1024/s vs. 1024 B/s,
			sb.Append(" | "); //   and consider that the values may be flickering...
			sb.Append(this.txLineRate);
			sb.Append("/s");
		}

		private void AppendRxStatus(StringBuilder sb)
		{
			sb.Append(this.rxByteCount);
			sb.Append(" | ");
			sb.Append(this.rxLineCount);
			sb.Append(" @ ");
			sb.Append(this.rxByteRate);
			sb.Append("/s");
			sb.Append(" | ");
			sb.Append(this.rxLineRate);
			sb.Append("/s");
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
