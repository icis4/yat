//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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
	public class SerialPortSettings : MKY.Settings.SettingsItem
	{
		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		/// <remarks>
		/// Implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static AutoRetry AutoReopenDefault
		{
			get { return (new AutoRetry(true, 2000)); }
		}

		/// <summary></summary>
		public const int AutoReopenMinInterval = 100;

		/// <remarks>
		/// Implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static OutputBufferSize OutputBufferSizeDefault
		{
			get { return (new OutputBufferSize(false, 2048)); } // 2048 is default of 'SerialPort'.
		}

		/// <remarks>
		/// Implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static ChunkSize MaxChunkSizeDefault
		{
			get { return (new ChunkSize(true, 64)); } // 64 bytes reflects the typical USB report size, i.e. USB Ser/CDC converters.
		}

		/// <remarks>
		/// Implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static SendRate MaxSendRateDefault
		{
			get { return (new SendRate(false, 64, 10)); } // 64 bytes per 10 ms, an example limitation of an embedded sytems.
		}

		/// <summary></summary>
		public const int SendRateMaxInterval = 1000; // 1 second.

		/// <summary></summary>
		public const bool NoSendOnOutputBreakDefault = true;

		/// <summary></summary>
		public const bool NoSendOnInputBreakDefault = false;

		private const string Undefined = "<Undefined>";

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private SerialPortId portId;

		private SerialCommunicationSettings communication;
		private AutoRetry autoReopen;
		private OutputBufferSize outputBufferSize;
		private ChunkSize maxChunkSize;
		private SendRate maxSendRate;

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
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary>
		/// Creates new port settings with specified arguments.
		/// </summary>
		public SerialPortSettings(SerialPortId portId, SerialCommunicationSettings communication)
		{
			SetMyDefaults();
			InitializeNodes();

			// Attention: Port ID can be null (if no serial COM ports are available on system).
			if (portId != null)
				PortId = new SerialPortId(portId);
			else
				PortId = null;

			Communication = new SerialCommunicationSettings(communication);

			ClearChanged();
		}

		/// <summary></summary>
		public SerialPortSettings(Settings.SettingsType settingsType)
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

		/// <summary>
		/// Creates new port settings from <paramref name="rhs"/>.
		/// </summary>
		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
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
			AutoReopen          = rhs.autoReopen;
			OutputBufferSize    = rhs.OutputBufferSize;
			MaxChunkSize        = rhs.MaxChunkSize;
			MaxSendRate         = rhs.MaxSendRate;

			NoSendOnOutputBreak = rhs.NoSendOnOutputBreak;
			NoSendOnInputBreak  = rhs.NoSendOnInputBreak;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
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

			AutoReopen          = AutoReopenDefault;
			OutputBufferSize    = OutputBufferSizeDefault;
			MaxChunkSize        = MaxChunkSizeDefault;
			MaxSendRate         = MaxSendRateDefault;

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
					SetChanged();
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
				if (value == null)
				{
					DetachNode(this.communication);
					this.communication = null;
				}
				else if (this.communication == null)
				{
					this.communication = value;
					AttachNode(this.communication);
				}
				else if (this.communication != value)
				{
					SerialCommunicationSettings old = this.communication;
					this.communication = value;
					ReplaceNode(old, this.communication);
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoReopen")]
		public virtual AutoRetry AutoReopen
		{
			get { return (this.autoReopen); }
			set
			{
				if (this.autoReopen != value)
				{
					this.autoReopen = value;
					SetChanged();
				}
			}
		}

		/// <summary>
		/// The serial ports 'WriteBufferSize' typically is 2048. However, devices may
		/// not be able to deal with that much data.
		/// </summary>
		[XmlElement("OutputBufferSize")]
		public virtual OutputBufferSize OutputBufferSize
		{
			get { return (this.outputBufferSize); }
			set
			{
				if (this.outputBufferSize != value)
				{
					this.outputBufferSize = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MaxChunkSize")]
		public virtual ChunkSize MaxChunkSize
		{
			get { return (this.maxChunkSize); }
			set
			{
				if (this.maxChunkSize != value)
				{
					this.maxChunkSize = value;
					SetChanged();
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
		public virtual SendRate MaxSendRate
		{
			get { return (this.maxSendRate); }
			set
			{
				if (this.maxSendRate != value)
				{
					this.maxSendRate = value;
					SetChanged();
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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SerialPortSettings other = (SerialPortSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(PortId              == other.PortId) &&

				(AutoReopen          == other.AutoReopen) &&
				(OutputBufferSize    == other.OutputBufferSize) &&
				(MaxChunkSize        == other.MaxChunkSize) &&
				(MaxSendRate         == other.MaxSendRate) &&

				(NoSendOnOutputBreak == other.NoSendOnOutputBreak) &&
				(NoSendOnInputBreak  == other.NoSendOnInputBreak)
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

				hashCode = (hashCode * 397) ^  AutoReopen             .GetHashCode();
				hashCode = (hashCode * 397) ^  OutputBufferSize       .GetHashCode();
				hashCode = (hashCode * 397) ^  MaxChunkSize           .GetHashCode();
				hashCode = (hashCode * 397) ^  MaxSendRate            .GetHashCode();

				hashCode = (hashCode * 397) ^  NoSendOnOutputBreak    .GetHashCode();
				hashCode = (hashCode * 397) ^  NoSendOnInputBreak     .GetHashCode();

				return (hashCode);
			}
		}

		#region Object Members > Extensions
		//------------------------------------------------------------------------------------------
		// Object Members > Extensions
		//------------------------------------------------------------------------------------------

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
			string delimiters = "/,;";
			string[] sa = s.Trim().Split(delimiters.ToCharArray(), 2);
			if (sa.Length == 2)
			{
				SerialPortId portId;
				if (SerialPortId.TryParse(sa[0], out portId))
				{
					SerialCommunicationSettings communication;
					if (SerialCommunicationSettings.TryParse(sa[1], out communication))
					{
						settings = new SerialPortSettings(portId, communication);
						return (true);
					}
				}
			}

			settings = null;
			return (false);
		}

		#endregion

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
