using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TransmitSettings : Utilities.Settings.Settings, IEquatable<TransmitSettings>
	{
		/// <summary></summary>
		public const bool LocalEchoEnabledDefault = true;

		private bool _localEchoEnabled;

		/// <summary></summary>
		public TransmitSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public TransmitSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public TransmitSettings(TransmitSettings rhs)
			: base(rhs)
		{
			LocalEchoEnabled = rhs.LocalEchoEnabled;
			ClearChanged();
		}

		/// <summary></summary>
		protected override void SetMyDefaults()
		{
			LocalEchoEnabled = LocalEchoEnabledDefault;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("LocalEchoEnabled")]
		public bool LocalEchoEnabled
		{
			get { return (_localEchoEnabled); }
			set
			{
				if (_localEchoEnabled != value)
				{
					_localEchoEnabled = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is TransmitSettings)
				return (Equals((TransmitSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TransmitSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_localEchoEnabled.Equals(value._localEchoEnabled)
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
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(TransmitSettings lhs, TransmitSettings rhs)
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
		public static bool operator !=(TransmitSettings lhs, TransmitSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
