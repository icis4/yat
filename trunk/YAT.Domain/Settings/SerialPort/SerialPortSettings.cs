using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings.SerialPort
{
	/// <summary></summary>
	[Serializable]
	public class SerialPortSettings : MKY.Utilities.Settings.Settings, IEquatable<SerialPortSettings>
	{
		/// <summary></summary>
		public const bool ReplaceParityErrorsDefault = false;
		/// <summary></summary>
		public const string ParityErrorReplacementDefault = @"\h(00)";
		/// <summary></summary>
		public const byte ParityErrorReplacementDefaultAsByte = 0x00;

		private MKY.IO.Ports.SerialPortId _portId;
		private SerialCommunicationSettings _communication;
		private bool _replaceParityErrors;
		private string _parityErrorReplacement;
		private bool _rtsEnabled;
		private bool _dtrEnabled;

		/// <summary></summary>
		public SerialPortSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public SerialPortSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			Communication = new SerialCommunicationSettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SerialPortSettings(SerialPortSettings rhs)
			: base(rhs)
		{
			_portId = rhs.PortId;
			Communication = new SerialCommunicationSettings(rhs.Communication);
			_replaceParityErrors = rhs._replaceParityErrors;
			_parityErrorReplacement = rhs.ParityErrorReplacement;
			_rtsEnabled = rhs.RtsEnabled;
			_dtrEnabled = rhs.DtrEnabled;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			PortId = MKY.IO.Ports.SerialPortId.DefaultPort;
			ReplaceParityErrors = ReplaceParityErrorsDefault;
			ParityErrorReplacement = ParityErrorReplacementDefault;
			RtsEnabled = false;
			DtrEnabled = false;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("PortId")]
		public MKY.IO.Ports.SerialPortId PortId
		{
			get { return (_portId); }
			set
			{
				if (_portId != value)
				{
					_portId = value;

					if (!_portId.HasDescriptionFromSystem)
						_portId.GetDescriptionFromSystem();

					SetChanged();
				}
			}
		}

		/// <summary></summary>
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

		/// <summary></summary>
		[XmlElement("ReplaceParityErrors")]
		public bool ReplaceParityErrors
		{
			get { return (_replaceParityErrors); }
			set
			{
				if (_replaceParityErrors != value)
				{
					_replaceParityErrors = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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
					_replaceParityErrors.Equals(value._replaceParityErrors) &&
					_parityErrorReplacement.Equals(value._parityErrorReplacement) &&
					_rtsEnabled.Equals(value._rtsEnabled) &&
					_dtrEnabled.Equals(value._dtrEnabled)
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
