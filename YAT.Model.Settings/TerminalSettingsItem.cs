//==================================================================================================
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
using System.Xml.Serialization;

using MKY.Utilities.Guid;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettingsItem : MKY.Utilities.Settings.Settings, IEquatable<TerminalSettingsItem>, IGuidProvider
	{
		private string filePath;
		private Guid guid;
		private WindowSettings window;

		/// <summary></summary>
		public TerminalSettingsItem()
			: base(MKY.Utilities.Settings.SettingsType.Implicit)
		{
			SetMyDefaults();

			Window = new WindowSettings(SettingsType);

			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TerminalSettingsItem(TerminalSettingsItem rhs)
			: base(rhs)
		{
			this.filePath = rhs.FilePath;
			this.guid = rhs.Guid;

			Window = new WindowSettings(rhs.Window);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			FilePath = "";
			Guid = Guid.Empty;
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
				if (value != this.filePath)
				{
					this.filePath = value;
					SetChanged();
				}
				
				// Create GUID from file path
				if ((this.guid == Guid.Empty) && (this.filePath != ""))
					this.guid = XGuid.CreateGuidFromFilePath(this.filePath, YAT.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual Guid Guid
		{
			get { return (this.guid); }
			set
			{
				if (value != this.guid)
				{
					this.guid = value;
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
				if (this.window == null)
				{
					this.window = value;
					AttachNode(this.window);
				}
				else if (value != this.window)
				{
					WindowSettings old = this.window;
					this.window = value;
					ReplaceNode(old, this.window);
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
			if (obj is TerminalSettingsItem)
				return (Equals((TerminalSettingsItem)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(TerminalSettingsItem value)
		{
			// Ensure that object.operator!=() is called.
			if ((object)value != null)
			{
				return
					(
					(this.filePath == value.filePath) &&
					(this.guid     == value.guid) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // Compare all settings nodes.
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
		public static bool operator ==(TerminalSettingsItem lhs, TerminalSettingsItem rhs)
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
