//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace YAT.Settings
{
	[Serializable]
	public class GeneralSettings : MKY.Utilities.Settings.Settings, IEquatable<GeneralSettings>
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
		private bool detectSerialPortsInUse;

		public GeneralSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public GeneralSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public GeneralSettings(GeneralSettings rhs)
			: base(rhs)
		{
			this.autoOpenWorkspace      = rhs.AutoOpenWorkspace;
			this.autoSaveWorkspace      = rhs.AutoSaveWorkspace;
			this.useRelativePaths       = rhs.UseRelativePaths;
			this.detectSerialPortsInUse = rhs.DetectSerialPortsInUse;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			AutoOpenWorkspace      = true;
			AutoSaveWorkspace      = true;
			UseRelativePaths       = true;
			DetectSerialPortsInUse = true;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		[XmlElement("AutoOpenWorkspace")]
		public virtual bool AutoOpenWorkspace
		{
			get { return (this.autoOpenWorkspace); }
			set
			{
				if (value != this.autoOpenWorkspace)
				{
					this.autoOpenWorkspace = value;
					SetChanged();
				}
			}
		}

		[XmlElement("AutoSaveWorkspace")]
		public virtual bool AutoSaveWorkspace
		{
			get { return (this.autoSaveWorkspace); }
			set
			{
				if (value != this.autoSaveWorkspace)
				{
					this.autoSaveWorkspace = value;
					SetChanged();
				}
			}
		}

		[XmlElement("UseRelativePaths")]
		public virtual bool UseRelativePaths
		{
			get { return (this.useRelativePaths); }
			set
			{
				if (value != this.useRelativePaths)
				{
					this.useRelativePaths = value;
					SetChanged();
				}
			}
		}

		[XmlElement("DetectSerialPortsInUse")]
		public virtual bool DetectSerialPortsInUse
		{
			get { return (this.detectSerialPortsInUse); }
			set
			{
				if (value != this.detectSerialPortsInUse)
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
		public override bool Equals(object obj)
		{
			if (obj == null)
				return (false);

			GeneralSettings casted = obj as GeneralSettings;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(GeneralSettings other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.

				this.autoOpenWorkspace      == other.autoOpenWorkspace &&
				this.autoSaveWorkspace      == other.autoSaveWorkspace &&
				this.useRelativePaths       == other.useRelativePaths &&
				this.detectSerialPortsInUse == other.detectSerialPortsInUse
			);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(GeneralSettings lhs, GeneralSettings rhs)
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
