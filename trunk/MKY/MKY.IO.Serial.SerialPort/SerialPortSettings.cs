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
// MKY Version 1.0.28 Development
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
using System.Xml.Serialization;

using MKY.IO.Ports;

namespace MKY.IO.Serial.SerialPort
{
	/// <summary></summary>
	[Serializable]
	public class SerialPortSettings : Settings.SettingsItem, IEquatable<SerialPortSettings>
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <summary></summary>
		public const bool SignalXOnWhenOpenedDefault = true;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static IntervalSettingTuple AliveMonitorDefault
		{
			get { return (new IntervalSettingTuple(true, 500)); }
		}

		/// <summary></summary>
		public const int AliveMonitorMinInterval = 100;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static IntervalSettingTuple AutoReopenDefault
		{
			get { return (new IntervalSettingTuple(true, 2000)); }
		}

		/// <summary></summary>
		public const int AutoReopenMinInterval = 100;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static SizeSettingTuple OutputBufferSizeDefault
		{
			get { return (new SizeSettingTuple(false, SerialPortEx.WriteBufferSizeDefault)); }
		}

		/// <summary></summary>
		public const bool BufferMaxBaudRateDefault = true;

		/// <summary>
		/// Default of 48 bytes reflects the typical USB report size of 64, minus some bytes of
		/// meta data, minus some spare bytes, to a value that looks 'well' for computer engineers.
		///
		/// Some concrete values 'measured' by experiment:
		///  > Prolific USB/COM @ MT MKY loses data as soon as chunks above 356 bytes are sent.
		///  > Dell docking station @ MT SST (SPI/COM Intel chipset, Microsoft driver), looses
		///                                     data as soon as chunks above 56 bytes are sent.
		/// </summary>
		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static SizeSettingTuple MaxChunkSizeDefault
		{
			get { return (new SizeSettingTuple(true, 48)); }
		}

		/// <summary>
		/// Default is 48 bytes per 10 milliseconds, an example limitation of an embedded system.
		/// </summary>
		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static RateSettingTuple MaxSendRateDefault
		{
			get { return (new RateSettingTuple(false, 48, 10)); }
		}

		/// <summary></summary>
		public const int SendRateMaxInterval = 1000; // 1 second.

		/// <summary></summary>
		public const bool IgnoreFramingErrorsDefault = false;

		/// <summary></summary>
		public const bool EnableRetainingWarningsDefault = true;

		/// <summary></summary>
		public const bool NoSendOnOutputBreakDefault = true;

		/// <summary></summary>
		public const bool NoSendOnInputBreakDefault = false;

		private const string Undefined = "(undefined)";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SerialPortId portId;

		private SerialCommunicationSettings communication;
		private bool signalXOnWhenOpened;
		private IntervalSettingTuple aliveMonitor;
		private IntervalSettingTuple autoReopen;
		private SizeSettingTuple outputBufferSize;
		private bool bufferMaxBaudRate;
		private SizeSettingTuple maxChunkSize;
		private RateSettingTuple maxSendRate;

		private bool ignoreFramingErrors;
		private bool enableRetainingWarnings;

		private bool noSendOnOutputBreak;
		private bool noSendOnInputBreak;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialPortSettings()
			: this(Settings.SettingsType.Explicit)
		{
		}

		/// <summary>
		/// Creates new port settings with defaults.
		/// </summary>
		public SerialPortSettings(Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			Communication = new SerialCommunicationSettings(settingsType);

			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SerialPortSettings(SerialPortId portId)
			: this(portId, new SerialCommunicationSettings(SerialCommunicationSettings.BaudRateDefault))
		{
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SerialPortSettings(SerialPortId portId, SerialCommunicationSettings communication)
		{
			SetMyDefaults();

			// Attention: Port ID can be null (if no serial COM ports are available on system).
			if (portId != null)
				PortId = new SerialPortId(portId);
			else
				PortId = null;

			Communication = new SerialCommunicationSettings(communication);

			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SerialPortSettings(SerialPortSettings rhs)
			: base(rhs)
		{
			// Attention: Port ID can be null (if no serial COM ports are available on system).
			if (rhs.PortId != null)
				PortId = new SerialPortId(rhs.PortId);
			else
				PortId = null;

			Communication       = new SerialCommunicationSettings(rhs.Communication);
			SignalXOnWhenOpened = rhs.SignalXOnWhenOpened;
			AliveMonitor        = rhs.AliveMonitor;
			AutoReopen          = rhs.AutoReopen;
			OutputBufferSize    = rhs.OutputBufferSize;
			BufferMaxBaudRate   = rhs.BufferMaxBaudRate;
			MaxChunkSize        = rhs.MaxChunkSize;
			MaxSendRate         = rhs.MaxSendRate;

			IgnoreFramingErrors     = rhs.IgnoreFramingErrors;
			EnableRetainingWarnings = rhs.EnableRetainingWarnings;

			NoSendOnOutputBreak = rhs.NoSendOnOutputBreak;
			NoSendOnInputBreak  = rhs.NoSendOnInputBreak;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		///
		/// Attention: Do not use <see cref="Ports.SerialPortId.FirstAvailablePort"/>
		/// for the default port. <see cref="Ports.SerialPortId.FirstStandardPort"/>
		/// is way better performing and good enough for most use cases.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			// Attention: See remarks above.
			PortId              = SerialPortId.FirstStandardPort;

			SignalXOnWhenOpened = SignalXOnWhenOpenedDefault;
			AliveMonitor        = AliveMonitorDefault;
			AutoReopen          = AutoReopenDefault;
			OutputBufferSize    = OutputBufferSizeDefault;
			BufferMaxBaudRate   = BufferMaxBaudRateDefault;
			MaxChunkSize        = MaxChunkSizeDefault;
			MaxSendRate         = MaxSendRateDefault;

			IgnoreFramingErrors     = IgnoreFramingErrorsDefault;
			EnableRetainingWarnings = EnableRetainingWarningsDefault;

			NoSendOnOutputBreak = NoSendOnOutputBreakDefault;
			NoSendOnInputBreak  = NoSendOnInputBreakDefault;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("PortId")]
		public virtual SerialPortId PortId
		{
			get { return (this.portId); }
			set
			{
				if (this.portId != value)
				{
					this.portId = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Communication")]
		public virtual SerialCommunicationSettings Communication
		{
			get { return (this.communication); }
			set
			{
				if (this.communication != value)
				{
					var oldNode = this.communication;
					this.communication = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <remarks>Applies if <see cref="SerialCommunicationSettings.FlowControlUsesXOnXOff"/>.</remarks>
		[XmlElement("SignalXOnWhenOpened")]
		public virtual bool SignalXOnWhenOpened
		{
			get { return (this.signalXOnWhenOpened); }
			set
			{
				if (this.signalXOnWhenOpened != value)
				{
					this.signalXOnWhenOpened = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AliveMonitor")]
		public virtual IntervalSettingTuple AliveMonitor
		{
			get { return (this.aliveMonitor); }
			set
			{
				if (this.aliveMonitor != value)
				{
					this.aliveMonitor = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoReopen")]
		public virtual IntervalSettingTuple AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (this.autoReopen != value)
				{
					this.autoReopen = value;
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// The serial ports 'WriteBufferSize' typically is 2048. However, devices may
		/// not be able to deal with that much data. <see cref="SerialPort"/> for details.
		/// </summary>
		[XmlElement("OutputBufferSize")]
		public virtual SizeSettingTuple OutputBufferSize
		{
			get { return (this.outputBufferSize); }
			set
			{
				if (this.outputBufferSize != value)
				{
					this.outputBufferSize = value;
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// The serial ports 'WriteBufferSize' typically is 2048. However, devices may
		/// not be able to deal with that much data. <see cref="SerialPort"/> for details.
		/// </summary>
		/// <remarks>
		/// Somewhat awkward name, but neither "OutputMaxBaudRate" nor "WriteMaxBaudRate"
		/// nor "LimitBufferToBaudRate" are better... And "MaxBaudRate" would obviously
		/// be misleading...
		/// </remarks>
		[XmlElement("BufferMaxBaudRate")]
		public virtual bool BufferMaxBaudRate
		{
			get { return (this.bufferMaxBaudRate); }
			set
			{
				if (this.bufferMaxBaudRate != value)
				{
					this.bufferMaxBaudRate = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MaxChunkSize")]
		public virtual SizeSettingTuple MaxChunkSize
		{
			get { return (this.maxChunkSize); }
			set
			{
				if (this.maxChunkSize != value)
				{
					this.maxChunkSize = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual int MaxChunkSizeMaxSize
		{
			get
			{
				if (OutputBufferSize.Enabled)
					return (OutputBufferSize.Size);
				else
					return (OutputBufferSizeDefault.Size);
			}
		}

		/// <summary></summary>
		[XmlElement("MaxSendRate")]
		public virtual RateSettingTuple MaxSendRate
		{
			get { return (this.maxSendRate); }
			set
			{
				if (this.maxSendRate != value)
				{
					this.maxSendRate = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("IgnoreFramingErrors")]
		public virtual bool IgnoreFramingErrors
		{
			get { return (this.ignoreFramingErrors); }
			set
			{
				if (this.ignoreFramingErrors != value)
				{
					this.ignoreFramingErrors = value;
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// Enables warnings like "XOff state, retaining data..." on sending.
		/// </summary>
		/// <remarks>
		/// This setting is enabled by default.
		/// </remarks>
		[XmlElement("EnableRetainingWarnings")]
		public virtual bool EnableRetainingWarnings
		{
			get { return (this.enableRetainingWarnings); }
			set
			{
				if (this.enableRetainingWarnings != value)
				{
					this.enableRetainingWarnings = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NoSendOnOutputBreak")]
		public virtual bool NoSendOnOutputBreak
		{
			get { return (this.noSendOnOutputBreak); }
			set
			{
				if (this.noSendOnOutputBreak != value)
				{
					this.noSendOnOutputBreak = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NoSendOnInputBreak")]
		public virtual bool NoSendOnInputBreak
		{
			get { return (this.noSendOnInputBreak); }
			set
			{
				if (this.noSendOnInputBreak != value)
				{
					this.noSendOnInputBreak = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary></summary>
		public virtual string ToShortPortString()
		{
			if (PortId != null)
				return (PortId);
			else
				return (Undefined);
		}

		/// <summary>
		/// Returns port ID and port settings as a single string.
		/// </summary>
		public virtual string ToShortString()
		{
			return
			(
				PortId.ToString() + ", " +
				Communication.ToString()
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (PortId != null ? PortId.GetHashCode() : 0); // May be 'null' if no ports are available!

				hashCode = (hashCode * 397) ^  SignalXOnWhenOpened.GetHashCode();
				hashCode = (hashCode * 397) ^  AliveMonitor       .GetHashCode();
				hashCode = (hashCode * 397) ^  AutoReopen         .GetHashCode();
				hashCode = (hashCode * 397) ^  OutputBufferSize   .GetHashCode();
				hashCode = (hashCode * 397) ^  BufferMaxBaudRate  .GetHashCode();
				hashCode = (hashCode * 397) ^  MaxChunkSize       .GetHashCode();
				hashCode = (hashCode * 397) ^  MaxSendRate        .GetHashCode();

				hashCode = (hashCode * 397) ^  IgnoreFramingErrors    .GetHashCode();
				hashCode = (hashCode * 397) ^  EnableRetainingWarnings.GetHashCode();

				hashCode = (hashCode * 397) ^  NoSendOnOutputBreak.GetHashCode();
				hashCode = (hashCode * 397) ^  NoSendOnInputBreak .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SerialPortSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SerialPortSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ObjectEx.Equals(PortId, other.PortId) &&

				SignalXOnWhenOpened.Equals(other.SignalXOnWhenOpened) &&
				AliveMonitor       .Equals(other.AliveMonitor)        &&
				AutoReopen         .Equals(other.AutoReopen)          &&
				OutputBufferSize   .Equals(other.OutputBufferSize)    &&
				BufferMaxBaudRate  .Equals(other.BufferMaxBaudRate)   &&
				MaxChunkSize       .Equals(other.MaxChunkSize)        &&
				MaxSendRate        .Equals(other.MaxSendRate)         &&

				IgnoreFramingErrors    .Equals(other.IgnoreFramingErrors)     &&
				EnableRetainingWarnings.Equals(other.EnableRetainingWarnings) &&

				NoSendOnOutputBreak.Equals(other.NoSendOnOutputBreak) &&
				NoSendOnInputBreak .Equals(other.NoSendOnInputBreak)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SerialPortSettings lhs, SerialPortSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(SerialPortSettings lhs, SerialPortSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <summary>
		/// Parses <paramref name="s"/> for short serial port settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialPortSettings ParseShort(string s)
		{
			SerialPortSettings result;
			if (TryParseShort(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" does not specify valid short serial port settings."));
		}

		/// <summary>
		/// Tries to parse <paramref name="s"/> for short serial port settings and returns a corresponding settings object.
		/// </summary>
		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParseShort(string s, out SerialPortSettings settings)
		{
			var delimiters = " ,;|";
			var sa = s.Trim().Split(delimiters.ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);
			if (sa.Length > 0)
			{
				SerialPortId portId;
				if (SerialPortId.TryParse(sa[0], out portId))
				{
					if (sa.Length > 1)
					{
						SerialCommunicationSettings communication;
						if (SerialCommunicationSettings.TryParse(sa[1], out communication))
						{
							settings = new SerialPortSettings(portId, communication);
							return (true);
						}
					}
					else
					{
						settings = new SerialPortSettings(portId);
						return (true);
					}
				}
			}

			settings = null;
			return (false);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
