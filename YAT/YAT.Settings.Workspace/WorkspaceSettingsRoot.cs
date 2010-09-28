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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Guid;

using YAT.Settings;
using YAT.Model.Settings;

namespace YAT.Settings.Workspace
{
	/// <summary></summary>
	[Serializable]
	[XmlRoot("Settings")]
	public class WorkspaceSettingsRoot : MKY.Utilities.Settings.Settings, IEquatable<WorkspaceSettingsRoot>
	{
		private string productVersion = System.Windows.Forms.Application.ProductVersion;
		private bool autoSaved = false;
		private WorkspaceSettings workspace;

		/// <summary></summary>
		public WorkspaceSettingsRoot()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			Workspace = new WorkspaceSettings();
			ClearChanged();
		}

		/// <summary></summary>
		public WorkspaceSettingsRoot(WorkspaceSettingsRoot rhs)
			: base(rhs)
		{
			Workspace = new WorkspaceSettings(rhs.Workspace);
			ClearChanged();
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>File type is a kind of title, therefore capital 'W' and 'S'.</remarks>
		[XmlElement("FileType")]
		public virtual string FileType
		{
			get { return ("YAT Workspace Settings"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Warning")]
		public virtual string Warning
		{
			get { return ("Modifying this file may cause undefined behaviour!"); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("Saved")]
		public virtual SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("ProductVersion")]
		public virtual string ProductVersion
		{
			get { return (this.productVersion); }
			set { } // Do nothing.
		}

		/// <summary></summary>
		[XmlElement("AutoSaved")]
		public virtual bool AutoSaved
		{
			get { return (this.autoSaved); }
			set
			{
				if (value != this.autoSaved)
				{
					this.autoSaved = value;

					// Do not set changed.
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Workspace")]
		public virtual WorkspaceSettings Workspace
		{
			get { return (this.workspace); }
			set
			{
				if (this.workspace == null)
				{
					this.workspace = value;
					AttachNode(this.workspace);
				}
				else if (value != this.workspace)
				{
					WorkspaceSettings old = this.workspace;
					this.workspace = value;
					ReplaceNode(old, this.workspace);
				}
			}
		}

		#endregion

		#region Property Shortcuts
		//------------------------------------------------------------------------------------------
		// Property Shortcuts
		//------------------------------------------------------------------------------------------

		/// <remarks>Attention, this is just a shortcut for convenience, not a true property.</remarks>
		[XmlIgnore]
		public GuidList<TerminalSettingsItem> TerminalSettings
		{
			get { return (this.workspace.TerminalSettings); }
			set { this.workspace.TerminalSettings = value; }
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

			WorkspaceSettingsRoot casted = obj as WorkspaceSettingsRoot;
			if (casted == null)
				return (false);

			return (Equals(casted));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(WorkspaceSettingsRoot other)
		{
			// Ensure that object.operator==() is called.
			if ((object)other == null)
				return (false);

			return
			(
				base.Equals((MKY.Utilities.Settings.Settings)other) && // Compare all settings nodes.

				(this.productVersion == other.productVersion)
				// Do not compare AutoSaved.
			);
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
		public static bool operator ==(WorkspaceSettingsRoot lhs, WorkspaceSettingsRoot rhs)
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
		public static bool operator !=(WorkspaceSettingsRoot lhs, WorkspaceSettingsRoot rhs)
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
