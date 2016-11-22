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
// YAT 2.0 Gamma 2'' Version 1.99.52
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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	public class GeneralSettings : MKY.Settings.SettingsItem
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

		private bool retrieveSerialPortCaptions;
		private bool detectSerialPortsInUse;

		private bool matchUsbSerial;

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
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public GeneralSettings(GeneralSettings rhs)
			: base(rhs)
		{
			AutoOpenWorkspace          = rhs.AutoOpenWorkspace;
			AutoSaveWorkspace          = rhs.AutoSaveWorkspace;
			UseRelativePaths           = rhs.UseRelativePaths;

			RetrieveSerialPortCaptions = rhs.RetrieveSerialPortCaptions;
			DetectSerialPortsInUse     = rhs.DetectSerialPortsInUse;

			MatchUsbSerial             = rhs.MatchUsbSerial;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			AutoOpenWorkspace          = true;
			AutoSaveWorkspace          = true;
			UseRelativePaths           = true;

			RetrieveSerialPortCaptions = true;
			DetectSerialPortsInUse     = true;

			MatchUsbSerial             = true;
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

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

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

			GeneralSettings other = (GeneralSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(AutoOpenWorkspace          == other.AutoOpenWorkspace) &&
				(AutoSaveWorkspace          == other.AutoSaveWorkspace) &&
				(UseRelativePaths           == other.UseRelativePaths) &&

				(RetrieveSerialPortCaptions == other.RetrieveSerialPortCaptions) &&
				(DetectSerialPortsInUse     == other.DetectSerialPortsInUse) &&

				(MatchUsbSerial             == other.MatchUsbSerial)
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

				hashCode = (hashCode * 397) ^ AutoOpenWorkspace         .GetHashCode();
				hashCode = (hashCode * 397) ^ AutoSaveWorkspace         .GetHashCode();
				hashCode = (hashCode * 397) ^ UseRelativePaths          .GetHashCode();

				hashCode = (hashCode * 397) ^ RetrieveSerialPortCaptions.GetHashCode();
				hashCode = (hashCode * 397) ^ DetectSerialPortsInUse    .GetHashCode();

				hashCode = (hashCode * 397) ^ MatchUsbSerial            .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(GeneralSettings lhs, GeneralSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			return (lhs.Equals(rhs));
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
