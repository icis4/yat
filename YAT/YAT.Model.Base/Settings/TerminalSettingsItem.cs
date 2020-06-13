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
// YAT Version 2.2.0 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.IO;
using System.Xml.Serialization;

using MKY;
using MKY.IO;

using YAT.Application.Settings;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class TerminalSettingsItem : MKY.Settings.SettingsItem, IEquatable<TerminalSettingsItem>, IGuidProvider
	{
		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstFixedId = 1;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID 0 refers to the active terminal, i.e. the 'default' terminal.
		/// </remarks>
		public const int ActiveFixedId = 0;

		/// <remarks>
		/// IDs are 1 (not 0) based for consistency with "Terminal1"...
		/// ID -1 means 'invalid', i.e. no terminal.
		/// </remarks>
		public const int InvalidFixedId = -1;

		private string filePath;
		private Guid guid;
		private int fixedId;
		private WindowSettings window;

		/// <summary></summary>
		public TerminalSettingsItem()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalSettingsItem(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();

			Window = new WindowSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalSettingsItem(TerminalSettingsItem rhs)
			: base(rhs)
		{
			FilePath = rhs.FilePath;
			Guid     = rhs.Guid;
			FixedId  = rhs.FixedId;

			Window = new WindowSettings(rhs.Window);

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FilePath = "";
			Guid     = Guid.Empty;
			FixedId  = ActiveFixedId;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("FilePath")]
		public virtual string FilePath
		{
			get { return (this.filePath); }
			set
			{
				if (this.filePath != value)
				{
					this.filePath = value;
					SetMyChanged();
				}

				// Create GUID from file path:
				if (!string.IsNullOrEmpty(this.filePath) && (this.guid == Guid.Empty))
				{
					Guid guid;
					if (GuidEx.TryParseTolerantly(Path.GetFileNameWithoutExtension(this.filePath), out guid))
						this.guid = guid;
					else
						this.guid = Guid.NewGuid();
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual Guid Guid
		{
			get { return (this.guid); }
			set
			{
				if (this.guid != value)
				{
					this.guid = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("FixedId")]
		public virtual int FixedId
		{
			get { return (this.fixedId); }
			set
			{
				if (this.fixedId != value)
				{
					this.fixedId = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Window")]
		public virtual WindowSettings Window
		{
			get { return (this.window); }
			set
			{
				if (this.window != value)
				{
					var oldNode = this.window;
					this.window = value; // New node must be referenced before replacing node below! Replace will invoke the 'Changed' event!

					AttachOrReplaceOrDetachNode(oldNode, value);
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsDefined
		{
			get
			{
				return (!string.IsNullOrEmpty(this.filePath) && (this.guid != Guid.Empty));
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

				hashCode = (hashCode * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^  Guid                       .GetHashCode();
				hashCode = (hashCode * 397) ^  FixedId                    .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as TerminalSettingsItem));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(TerminalSettingsItem other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx .Equals(FilePath, other.FilePath) &&
				Guid   .Equals(          other.Guid)     &&
				FixedId.Equals(          other.FixedId)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(TerminalSettingsItem lhs, TerminalSettingsItem rhs)
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
		public static bool operator !=(TerminalSettingsItem lhs, TerminalSettingsItem rhs)
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
