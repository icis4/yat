using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Guid;

using YAT.Settings;
using YAT.Model.Settings;

namespace YAT.Settings.Workspace
{
	[Serializable]
	[XmlRoot("Settings")]
	public class WorkspaceSettingsRoot : MKY.Utilities.Settings.Settings, IEquatable<WorkspaceSettingsRoot>
	{
		private string _productVersion = System.Windows.Forms.Application.ProductVersion;
		private bool _autoSaved = false;
		private WorkspaceSettings _workspace;

		public WorkspaceSettingsRoot()
			: base(MKY.Utilities.Settings.SettingsType.Explicit)
		{
			Workspace = new WorkspaceSettings();
			ClearChanged();
		}

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
		public string FileType
		{
			get { return ("YAT Workspace Settings"); }
			set { } // do nothing
		}

		[XmlElement("Warning")]
		public string Warning
		{
			get { return ("Modifying this file may cause undefined behaviour!"); }
			set { } // do nothing
		}

		[XmlElement("Saved")]
		public SaveInfo Saved
		{
			get { return (new SaveInfo(DateTime.Now, Environment.UserName)); }
			set { } // do nothing
		}

		[XmlElement("ProductVersion")]
		public string ProductVersion
		{
			get { return (_productVersion); }
			set { } // do nothing
		}

		[XmlElement("AutoSaved")]
		public bool AutoSaved
		{
			get { return (_autoSaved); }
			set
			{
				if (_autoSaved != value)
				{
					_autoSaved = value;
					// do not set changed;
				}
			}
		}

		[XmlElement("Workspace")]
		public WorkspaceSettings Workspace
		{
			get { return (_workspace); }
			set
			{
				if (_workspace == null)
				{
					_workspace = value;
					AttachNode(_workspace);
				}
				else if (_workspace != value)
				{
					WorkspaceSettings old = _workspace;
					_workspace = value;
					ReplaceNode(old, _workspace);
				}
			}
		}

		#endregion

		#region Property Shortcuts
		//------------------------------------------------------------------------------------------
		// Property Shortcuts
		//------------------------------------------------------------------------------------------

		[XmlIgnore]
		public GuidList<TerminalSettingsItem> TerminalSettings
		{
			get { return (_workspace.TerminalSettings); }
			set { _workspace.TerminalSettings = value; }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is WorkspaceSettingsRoot)
				return (Equals((WorkspaceSettingsRoot)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(WorkspaceSettingsRoot value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_productVersion.Equals(value._productVersion) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // compares all settings nodes
					);
				// do not compare AutoSaved
			}
			return (false);
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