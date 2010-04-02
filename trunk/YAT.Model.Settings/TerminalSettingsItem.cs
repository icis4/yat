//==================================================================================================
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
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using MKY.Utilities.Guid;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class TerminalSettingsItem : MKY.Utilities.Settings.Settings, IEquatable<TerminalSettingsItem>, IGuidProvider
	{
		private string _filePath;
		private Guid _guid;
		private WindowSettings _window;

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
			_filePath = rhs.FilePath;
			_guid = rhs.Guid;

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
			get { return (_filePath); }
			set
			{
				if (value != _filePath)
				{
					_filePath = value;
					SetChanged();
				}
				
				// Create GUID from file path
				if ((_guid == Guid.Empty) && (_filePath != ""))
					_guid = XGuid.CreateGuidFromFilePath(_filePath, YAT.Settings.GeneralSettings.AutoSaveTerminalFileNamePrefix);
			}
		}

		/// <summary></summary>
		[XmlIgnore]
		public virtual Guid Guid
		{
			get { return (_guid); }
			set
			{
				if (value != _guid)
				{
					_guid = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Window")]
		public virtual WindowSettings Window
		{
			get { return (_window); }
			set
			{
				if (_window == null)
				{
					_window = value;
					AttachNode(_window);
				}
				else if (value != _window)
				{
					WindowSettings old = _window;
					_window = value;
					ReplaceNode(old, _window);
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
					(_filePath == value._filePath) &&
					(_guid     == value._guid) &&
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
