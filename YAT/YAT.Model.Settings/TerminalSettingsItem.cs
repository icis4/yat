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
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

using MKY;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettingsItem : MKY.Settings.Settings, IGuidProvider
	{
		private string filePath;
		private Guid guid;
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
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public TerminalSettingsItem(TerminalSettingsItem rhs)
			: base(rhs)
		{
			FilePath = rhs.FilePath;
			Guid = rhs.Guid;

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
				if ((this.guid == Guid.Empty) && (this.filePath.Length > 0))
					this.guid = GuidEx.CreateGuidFromFilePath(this.filePath, YAT.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix);
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
				if (value == null)
				{
					this.window = value;
					DetachNode(this.window);
				}
				else if (this.window == null)
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
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			TerminalSettingsItem other = (TerminalSettingsItem)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(this.filePath == other.filePath) &&
				(this.guid     == other.guid)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.filePath.GetHashCode() ^
				this.guid    .GetHashCode()
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
