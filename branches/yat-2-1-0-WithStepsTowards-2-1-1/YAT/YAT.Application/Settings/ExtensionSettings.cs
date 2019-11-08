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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY;

using YAT.Application.Utilities;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	public class ExtensionSettings : MKY.Settings.SettingsItem, IEquatable<ExtensionSettings>
	{
		/// <summary></summary>
		public static readonly string TextSendFilesDefault = ExtensionHelper.TextSendExtensionDefault;

		/// <summary></summary>
		public static readonly string BinarySendFilesDefault = ExtensionHelper.BinarySendExtensionDefault;

		/// <summary></summary>
		public static readonly string ControlLogFilesDefault = ExtensionHelper.ControlLogExtensionDefault;

		/// <summary></summary>
		public static readonly string RawLogFilesDefault = ExtensionHelper.RawLogExtensionDefault;

		/// <summary></summary>
		public static readonly string NeatLogFilesDefault = ExtensionHelper.NeatLogExtensionDefault;

		/// <summary></summary>
		public static readonly string MonitorFilesDefault = ExtensionHelper.MonitorExtensionDefault;

		private string textSendFiles;
		private string binarySendFiles;

		private string controlLogFiles;
		private string rawLogFiles;
		private string neatLogFiles;

		private string monitorFiles;

		/// <summary></summary>
		public ExtensionSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public ExtensionSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public ExtensionSettings(ExtensionSettings rhs)
			: base(rhs)
		{
			TextSendFiles   = rhs.TextSendFiles;
			BinarySendFiles = rhs.BinarySendFiles;

			ControlLogFiles = rhs.ControlLogFiles;
			RawLogFiles     = rhs.RawLogFiles;
			NeatLogFiles    = rhs.NeatLogFiles;

			MonitorFiles    = rhs.MonitorFiles;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			TextSendFiles   = TextSendFilesDefault;
			BinarySendFiles = BinarySendFilesDefault;

			ControlLogFiles = ControlLogFilesDefault;
			RawLogFiles     = RawLogFilesDefault;
			NeatLogFiles    = NeatLogFilesDefault;

			MonitorFiles    = MonitorFilesDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("TextSendFiles")]
		public virtual string TextSendFiles
		{
			get { return (this.textSendFiles); }
			set
			{
				if (this.textSendFiles != value)
				{
					this.textSendFiles = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("BinarySendFiles")]
		public virtual string BinarySendFiles
		{
			get { return (this.binarySendFiles); }
			set
			{
				if (this.binarySendFiles != value)
				{
					this.binarySendFiles = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ControlLogFiles")]
		public virtual string ControlLogFiles
		{
			get { return (this.controlLogFiles); }
			set
			{
				if (this.controlLogFiles != value)
				{
					this.controlLogFiles = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("RawLogFiles")]
		public virtual string RawLogFiles
		{
			get { return (this.rawLogFiles); }
			set
			{
				if (this.rawLogFiles != value)
				{
					this.rawLogFiles = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("NeatLogFiles")]
		public virtual string NeatLogFiles
		{
			get { return (this.neatLogFiles); }
			set
			{
				if (this.neatLogFiles != value)
				{
					this.neatLogFiles = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("MonitorFiles")]
		public virtual string MonitorFiles
		{
			get { return (this.monitorFiles); }
			set
			{
				if (this.monitorFiles != value)
				{
					this.monitorFiles = value;
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

				hashCode = (hashCode * 397) ^ (TextSendFiles   != null ? TextSendFiles  .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (BinarySendFiles != null ? BinarySendFiles.GetHashCode() : 0);

				hashCode = (hashCode * 397) ^ (ControlLogFiles != null ? ControlLogFiles.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RawLogFiles     != null ? RawLogFiles    .GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (NeatLogFiles    != null ? NeatLogFiles   .GetHashCode() : 0);

				hashCode = (hashCode * 397) ^ (MonitorFiles    != null ? MonitorFiles   .GetHashCode() : 0);

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ExtensionSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ExtensionSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				StringEx.EqualsOrdinalIgnoreCase(TextSendFiles,   other.TextSendFiles)   &&
				StringEx.EqualsOrdinalIgnoreCase(BinarySendFiles, other.BinarySendFiles) &&

				StringEx.EqualsOrdinalIgnoreCase(ControlLogFiles, other.ControlLogFiles) &&
				StringEx.EqualsOrdinalIgnoreCase(RawLogFiles,     other.RawLogFiles)     &&
				StringEx.EqualsOrdinalIgnoreCase(NeatLogFiles,    other.NeatLogFiles)    &&

				StringEx.EqualsOrdinalIgnoreCase(MonitorFiles,    other.MonitorFiles)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ExtensionSettings lhs, ExtensionSettings rhs)
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
		public static bool operator !=(ExtensionSettings lhs, ExtensionSettings rhs)
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
