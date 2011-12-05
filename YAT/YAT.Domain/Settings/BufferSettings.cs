﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class BufferSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const int BufferSizeDefault = 65536;

		private int txBufferSize;
		private int rxBufferSize;

		/// <summary></summary>
		public BufferSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public BufferSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public BufferSettings(BufferSettings rhs)
			: base(rhs)
		{
			TxBufferSize = rhs.TxBufferSize;
			RxBufferSize = rhs.RxBufferSize;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TxBufferSize = BufferSizeDefault;
			RxBufferSize = BufferSizeDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TxBufferSize")]
		public virtual int TxBufferSize
		{
			get { return (this.txBufferSize); }
			set
			{
				if (value != this.txBufferSize)
				{
					this.txBufferSize = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxBufferSize")]
		public virtual int RxBufferSize
		{
			get { return (this.rxBufferSize); }
			set
			{
				if (value != this.rxBufferSize)
				{
					this.rxBufferSize = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int BidirBufferSize
		{
			get { return (TxBufferSize + RxBufferSize); }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			BufferSettings other = (BufferSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.txBufferSize == other.txBufferSize) &&
				(this.rxBufferSize == other.rxBufferSize)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.txBufferSize.GetHashCode() ^
				this.rxBufferSize.GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
