using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings.SerialPort
{
	public class SerialPortSettings : Utilities.Settings.Settings
	{
		public const string ParityErrorReplacementDefault = "\\h(00)";
		public const byte   ParityErrorReplacementDefaultAsByte = 0x00;

		private HSR.IO.Ports.SerialPortId _portId;
		private SerialCommunicationSettings _communication;
		private string _parityErrorReplacement;
		private bool _rtsEnabled;
		private bool _dtrEnabled;

		public SerialPortSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public SerialPortSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public SerialPortSettings(SerialPortSettings rhs)
			: base(rhs)
		{
			PortId = rhs.PortId;
			Communication = new SerialCommunicationSettings(rhs.Communication);
			ParityErrorReplacement = rhs.ParityErrorReplacement;
			RtsEnabled = rhs.RtsEnabled;
			DtrEnabled = rhs.DtrEnabled;
			ClearChanged();
		}

		private void InitializeNodes()
		{
			Communication = new SerialCommunicationSettings(SettingsType);
		}

		protected override void SetMyDefaults()
		{
			PortId = HSR.IO.Ports.SerialPortId.DefaultPort;
			ParityErrorReplacement = ParityErrorReplacementDefault;
			RtsEnabled = true;
			DtrEnabled = true;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("PortId")]
		public HSR.IO.Ports.SerialPortId PortId
		{
			get { return (_portId); }
			set
			{
				if (_portId != value)
				{
					_portId = value;
					SetChanged();
				}
			}
		}

		[XmlElement("Communication")]
		public SerialCommunicationSettings Communication
		{
			get { return (_communication); }
			set
			{
				if (_communication == null)
				{
					_communication = value;
					AttachNode(_communication);
				}
				else if (_communication != value)
				{
					SerialCommunicationSettings old = _communication;
					_communication = value;
					ReplaceNode(old, _communication);
				}
			}
		}

		[XmlElement("ParityErrorReplacement")]
		public string ParityErrorReplacement
		{
			get { return (_parityErrorReplacement); }
			set
			{
				if (_parityErrorReplacement != value)
				{
					_parityErrorReplacement = value;
					SetChanged();
				}
			}
		}

		[XmlElement("RtsEnabled")]
		public bool RtsEnabled
		{
			get { return (_rtsEnabled); }
			set
			{
				if (_rtsEnabled != value)
				{
					_rtsEnabled = value;
					SetChanged();
				}
			}
		}

		[XmlElement("DtrEnabled")]
		public bool DtrEnabled
		{
			get { return (_dtrEnabled); }
			set
			{
				if (_dtrEnabled != value)
				{
					_dtrEnabled = value;
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
			if (obj is SerialPortSettings)
				return (Equals((SerialPortSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SerialPortSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return 
					(
					_portId.Equals(value._portId) &&
					_communication.Equals(value._communication) &&
					_parityErrorReplacement.Equals(value._parityErrorReplacement) &&
					_rtsEnabled.Equals(value._rtsEnabled) &&
					_dtrEnabled.Equals(value._dtrEnabled)
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
		public static bool operator ==(SerialPortSettings lhs, SerialPortSettings rhs)
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
		public static bool operator !=(SerialPortSettings lhs, SerialPortSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
