//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Settings
{
	/// <summary></summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[Serializable]
	public class GeneralSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public static readonly string AutoSaveRoot = Application.LocalUserAppDataPath;

		/// <summary></summary>
		public const string AutoSaveTerminalFileNamePrefix = "Terminal-";

		/// <summary></summary>
		public const string AutoSaveWorkspaceFileNamePrefix = "Workspace-";

		private bool autoOpenWorkspace;
		private bool autoSaveWorkspace;
		private bool useRelativePaths;
		private bool retrieveSerialPortCaptions;
		private bool detectSerialPortsInUse;

		/// <summary></summary>
		public GeneralSettings()
		{
			SetMyDefaults();
			ClearChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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

			GeneralSettings other = (GeneralSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(AutoOpenWorkspace          == other.AutoOpenWorkspace) &&
				(AutoSaveWorkspace          == other.AutoSaveWorkspace) &&
				(UseRelativePaths           == other.UseRelativePaths) &&
				(RetrieveSerialPortCaptions == other.RetrieveSerialPortCaptions) &&
				(DetectSerialPortsInUse     == other.DetectSerialPortsInUse)
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
			return
			(
				base.GetHashCode() ^

				AutoOpenWorkspace         .GetHashCode() ^
				AutoSaveWorkspace         .GetHashCode() ^
				UseRelativePaths          .GetHashCode() ^
				RetrieveSerialPortCaptions.GetHashCode() ^
				DetectSerialPortsInUse    .GetHashCode()
			);
		}

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
