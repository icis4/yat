using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings
{
	public class BufferSettings : Utilities.Settings.Settings
	{
		public const int BufferSizeDefault = 8192;

		private int _txBufferSize;
		private int _rxBufferSize;

		public BufferSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public BufferSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public BufferSettings(BufferSettings rhs)
			: base(rhs)
		{
			TxBufferSize = rhs.TxBufferSize;
			RxBufferSize = rhs.RxBufferSize;
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			TxBufferSize = BufferSizeDefault;
			RxBufferSize = BufferSizeDefault;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
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
