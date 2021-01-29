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
// YAT Version 2.4.0
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	public class GeneralSettings : MKY.Settings.SettingsItem, IEquatable<GeneralSettings>
	{
		/// <summary></summary>
		public static readonly string AutoSaveRoot = System.Windows.Forms.Application.LocalUserAppDataPath;

		/// <summary></summary>
		public const string AutoSaveTerminalFileNamePrefix = "Terminal-";

		/// <summary></summary>
		public const string AutoSaveWorkspaceFileNamePrefix = "Workspace-";

		private bool autoOpenWorkspace;
		private bool autoSaveWorkspace;
		private bool useRelativePaths;

		private bool notifyNonAvailableIO;

		private bool retrieveSerialPortCaptions;
		private bool detectSerialPortsInUse;
		private bool askForAlternateSerialPort;

		private bool askForAlternateNetworkInterface;

		private bool matchUsbSerial;
		private bool askForAlternateUsbDevice;

		/// <summary></summary>
		public GeneralSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public GeneralSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public GeneralSettings(GeneralSettings rhs)
			: base(rhs)
		{
			AutoOpenWorkspace = rhs.AutoOpenWorkspace;
			AutoSaveWorkspace = rhs.AutoSaveWorkspace;
			UseRelativePaths  = rhs.UseRelativePaths;

			NotifyNonAvailableIO = rhs.NotifyNonAvailableIO;

			RetrieveSerialPortCaptions = rhs.RetrieveSerialPortCaptions;
			DetectSerialPortsInUse     = rhs.DetectSerialPortsInUse;
			AskForAlternateSerialPort  = rhs.AskForAlternateSerialPort;

			AskForAlternateNetworkInterface = rhs.AskForAlternateNetworkInterface;

			MatchUsbSerial           = rhs.MatchUsbSerial;
			AskForAlternateUsbDevice = rhs.AskForAlternateUsbDevice;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			AutoOpenWorkspace = true;
			AutoSaveWorkspace = true;
			UseRelativePaths  = true;

			NotifyNonAvailableIO = true;

			RetrieveSerialPortCaptions = true;
			DetectSerialPortsInUse     = true;
			AskForAlternateSerialPort  = true;

			AskForAlternateNetworkInterface = true;

			MatchUsbSerial           = true;
			AskForAlternateUsbDevice = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Auto save of workspace means that the last open workspace will tried to be automatically
		/// opened when starting the application.
		/// </summary>
		[XmlElement("AutoOpenWorkspace")]
		public virtual bool AutoOpenWorkspace
		{
			get { return (this.autoOpenWorkspace); }
			set
			{
				if (this.autoOpenWorkspace != value)
				{
					this.autoOpenWorkspace = value;
					SetMyChanged();
				}
			}
		}

		/// <summary>
		/// Auto save of workspace means that the workspace and terminal settings are tried to be
		/// automatically saved, either at an automatically chosen location, or at the current file
		/// location.
		/// </summary>
		[XmlElement("AutoSaveWorkspace")]
		public virtual bool AutoSaveWorkspace
		{
			get { return (this.autoSaveWorkspace); }
			set
			{
				if (this.autoSaveWorkspace != value)
				{
					this.autoSaveWorkspace = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("UseRelativePaths")]
		public virtual bool UseRelativePaths
		{
			get { return (this.useRelativePaths); }
			set
			{
				if (this.useRelativePaths != value)
				{
					this.useRelativePaths = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NotifyNonAvailableIO")]
		public virtual bool NotifyNonAvailableIO
		{
			get { return (this.notifyNonAvailableIO); }
			set
			{
				if (this.notifyNonAvailableIO != value)
				{
					this.notifyNonAvailableIO = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RetrieveSerialPortCaptions")]
		public virtual bool RetrieveSerialPortCaptions
		{
			get { return (this.retrieveSerialPortCaptions); }
			set
			{
				if (this.retrieveSerialPortCaptions != value)
				{
					this.retrieveSerialPortCaptions = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DetectSerialPortsInUse")]
		public virtual bool DetectSerialPortsInUse
		{
			get { return (this.detectSerialPortsInUse); }
			set
			{
				if (this.detectSerialPortsInUse != value)
				{
					this.detectSerialPortsInUse = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AskForAlternateSerialPort")]
		public virtual bool AskForAlternateSerialPort
		{
			get { return (this.askForAlternateSerialPort); }
			set
			{
				if (this.askForAlternateSerialPort != value)
				{
					this.askForAlternateSerialPort = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AskForAlternateNetworkInterface")]
		public virtual bool AskForAlternateNetworkInterface
		{
			get { return (this.askForAlternateNetworkInterface); }
			set
			{
				if (this.askForAlternateNetworkInterface != value)
				{
					this.askForAlternateNetworkInterface = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MatchUsbSerial")]
		public virtual bool MatchUsbSerial
		{
			get { return (this.matchUsbSerial); }
			set
			{
				if (this.matchUsbSerial != value)
				{
					this.matchUsbSerial = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AskForAlternateUsbDevice")]
		public virtual bool AskForAlternateUsbDevice
		{
			get { return (this.askForAlternateUsbDevice); }
			set
			{
				if (this.askForAlternateUsbDevice != value)
				{
					this.askForAlternateUsbDevice = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

				hashCode = (hashCode * 397) ^ AutoOpenWorkspace.GetHashCode();
				hashCode = (hashCode * 397) ^ AutoSaveWorkspace.GetHashCode();
				hashCode = (hashCode * 397) ^ UseRelativePaths .GetHashCode();

				hashCode = (hashCode * 397) ^ NotifyNonAvailableIO.GetHashCode();

				hashCode = (hashCode * 397) ^ RetrieveSerialPortCaptions.GetHashCode();
				hashCode = (hashCode * 397) ^ DetectSerialPortsInUse    .GetHashCode();
				hashCode = (hashCode * 397) ^ AskForAlternateSerialPort .GetHashCode();

				hashCode = (hashCode * 397) ^ AskForAlternateNetworkInterface.GetHashCode();

				hashCode = (hashCode * 397) ^ MatchUsbSerial          .GetHashCode();
				hashCode = (hashCode * 397) ^ AskForAlternateUsbDevice.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as GeneralSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(GeneralSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				AutoOpenWorkspace.Equals(other.AutoOpenWorkspace) &&
				AutoSaveWorkspace.Equals(other.AutoSaveWorkspace) &&
				UseRelativePaths .Equals(other.UseRelativePaths)  &&

				NotifyNonAvailableIO.Equals(other.NotifyNonAvailableIO) &&

				RetrieveSerialPortCaptions.Equals(other.RetrieveSerialPortCaptions) &&
				DetectSerialPortsInUse    .Equals(other.DetectSerialPortsInUse)     &&
				AskForAlternateSerialPort .Equals(other.AskForAlternateSerialPort)  &&

				AskForAlternateNetworkInterface.Equals(other.AskForAlternateNetworkInterface) &&

				MatchUsbSerial          .Equals(other.MatchUsbSerial)           &&
				AskForAlternateUsbDevice.Equals(other.AskForAlternateUsbDevice)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(GeneralSettings lhs, GeneralSettings rhs)
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
		public static bool operator !=(GeneralSettings lhs, GeneralSettings rhs)
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
