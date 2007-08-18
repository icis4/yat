using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.YAT.Settings;

namespace MKY.YAT.Settings.Workspace
{
	[Serializable]
	[XmlRoot("Settings")]
	public class WorkspaceSettingsRoot : Utilities.Settings.Settings, IEquatable<WorkspaceSettingsRoot>
	{
		private string _productVersion = System.Windows.Forms.Application.ProductVersion;
		private WorkspaceSettings _workspace;

		public WorkspaceSettingsRoot()
			: base(Utilities.Settings.SettingsType.Explicit)
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
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

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
			set
			{
				if (_productVersion != value)
				{
					_productVersion = value;
					SetChanged();
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
					base.Equals((Utilities.Settings.Settings)value) // compares all settings nodes
					);
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
		/// Determines whether the two specified objects have reference and value equality.
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