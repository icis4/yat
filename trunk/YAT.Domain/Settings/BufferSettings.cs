using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class BufferSettings : Utilities.Settings.Settings, IEquatable<BufferSettings>
	{
		/// <summary></summary>
		public const int BufferSizeDefault = 8192;

		private int _txBufferSize;
		private int _rxBufferSize;

		/// <summary></summary>
		public BufferSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public BufferSettings(Utilities.Settings.SettingsType settingsType)
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
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("TxBufferSize")]
		public int TxBufferSize
		{
			get { return (_txBufferSize); }
			set
			{
				if (_txBufferSize != value)
				{
					_txBufferSize = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RxBufferSize")]
		public int RxBufferSize
		{
			get { return (_rxBufferSize); }
			set
			{
				if (_rxBufferSize != value)
				{
					_rxBufferSize = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public int BidirBufferSize
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
			// ensure that object.operator!=() is called
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
