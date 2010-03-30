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
	public class BufferSettings : MKY.Utilities.Settings.Settings, IEquatable<BufferSettings>
	{
		/// <summary></summary>
		public const int BufferSizeDefault = 65536;

		private int _txBufferSize;
		private int _rxBufferSize;

		/// <summary></summary>
		public BufferSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public BufferSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public BufferSettings(BufferSettings rhs)
			: base(rhs)
		{
			_txBufferSize = rhs.TxBufferSize;
			_rxBufferSize = rhs.RxBufferSize;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
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
			get { return (_txBufferSize); }
			set
			{
				if (value != _txBufferSize)
				{
					_txBufferSize = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxBufferSize")]
		public virtual int RxBufferSize
		{
			get { return (_rxBufferSize); }
			set
			{
				if (value != _rxBufferSize)
				{
					_rxBufferSize = value;
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
			if (obj is BufferSettings)
				return (Equals((BufferSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(BufferSettings value)
		{
			// Ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_txBufferSize.Equals(value._txBufferSize) &&
					_rxBufferSize.Equals(value._rxBufferSize)
					);
			}
			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(BufferSettings lhs, BufferSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(BufferSettings lhs, BufferSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
