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
using System.Xml.Serialization;

using MKY;
using MKY.IO;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettingsItem : MKY.Settings.SettingsItem, IGuidProvider
	{
		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// </remarks>
		public const int FirstFixedIndex = 1;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index 0 means 'default'.
		/// </remarks>
		public const int DefaultFixedIndex = 0;

		/// <remarks>
		/// Indices are 1 (not 0) based for consistency with "Terminal1"...
		/// Index -1 means 'invalid'.
		/// </remarks>
		public const int InvalidFixedIndex = -1;

		private string filePath;
		private Guid guid;
		private int fixedIndex;
		private WindowSettings window;

		/// <summary></summary>
		public TerminalSettingsItem()
			: base(MKY.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			Window = new WindowSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public TerminalSettingsItem(TerminalSettingsItem rhs)
			: base(rhs)
		{
			FilePath   = rhs.FilePath;
			Guid       = rhs.Guid;
			FixedIndex = rhs.FixedIndex;

			Window = new WindowSettings(rhs.Window);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FilePath   = "";
			Guid       = Guid.Empty;
			FixedIndex = DefaultFixedIndex;
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
					SetChanged();
				}
				
				// Create GUID from file path:
				if (PathEx.IsDefined(this.filePath) && (this.guid == Guid.Empty))
				{
					Guid guid;
					if (GuidEx.TryCreateGuidFromFilePath(this.filePath, YAT.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix, out guid))
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
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("FixedIndex")]
		public virtual int FixedIndex
		{
			get { return (this.fixedIndex); }
			set
			{
				if (this.fixedIndex != value)
				{
					this.fixedIndex = value;
					SetChanged();
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
				if (value == null)
				{
					DetachNode(this.window);
					this.window = null;
				}
				else if (this.window == null)
				{
					this.window = value;
					AttachNode(this.window);
				}
				else if (this.window != value)
				{
					WindowSettings old = this.window;
					this.window = value;
					ReplaceNode(old, this.window);
				}
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual bool IsDefined
		{
			get
			{
				return (PathEx.IsDefined(this.filePath) && (this.guid != Guid.Empty));
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

			TerminalSettingsItem other = (TerminalSettingsItem)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				PathEx.Equals(FilePath, other.FilePath) &&
				(Guid                == other.Guid) &&
				(FixedIndex          == other.FixedIndex)
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

				FilePath  .GetHashCode() ^
				Guid      .GetHashCode() ^
				FixedIndex.GetHashCode()
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
