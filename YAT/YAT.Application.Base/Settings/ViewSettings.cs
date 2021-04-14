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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Application.Settings
{
	/// <summary></summary>
	public class ViewSettings : MKY.Settings.SettingsItem, IEquatable<ViewSettings>
	{
		private bool findIsVisible;
		private bool autoActionIsVisible;
		private bool autoResponseIsVisible;
	#if (WITH_SCRIPTING)
		private bool scriptPanelIsVisible;
	#endif

		/// <summary></summary>
		public ViewSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public ViewSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public ViewSettings(ViewSettings rhs)
			: base(rhs)
		{
			FindIsVisible         = rhs.FindIsVisible;
			AutoActionIsVisible   = rhs.AutoActionIsVisible;
			AutoResponseIsVisible = rhs.AutoResponseIsVisible;
		#if (WITH_SCRIPTING)
			ScriptPanelIsVisible  = rhs.ScriptPanelIsVisible;
		#endif

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			FindIsVisible         = false;
			AutoActionIsVisible   = false;
			AutoResponseIsVisible = false;
		#if (WITH_SCRIPTING)
			ScriptPanelIsVisible  = false;
		#endif
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("FindIsVisible")]
		public bool FindIsVisible
		{
			get { return (this.findIsVisible); }
			set
			{
				if (this.findIsVisible != value)
				{
					this.findIsVisible = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoActionIsVisible")]
		public bool AutoActionIsVisible
		{
			get { return (this.autoActionIsVisible); }
			set
			{
				if (this.autoActionIsVisible != value)
				{
					this.autoActionIsVisible = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("AutoResponseIsVisible")]
		public bool AutoResponseIsVisible
		{
			get { return (this.autoResponseIsVisible); }
			set
			{
				if (this.autoResponseIsVisible != value)
				{
					this.autoResponseIsVisible = value;
					SetMyChanged();
				}
			}
		}

	#if (WITH_SCRIPTING)

		/// <summary></summary>
		[XmlElement("ScriptPanelIsVisible")]
		public bool ScriptPanelIsVisible
		{
			get { return (this.scriptPanelIsVisible); }
			set
			{
				if (this.scriptPanelIsVisible != value)
				{
					this.scriptPanelIsVisible = value;
					SetMyChanged();
				}
			}
		}

	#endif // WITH_SCRIPTING

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

				hashCode = (hashCode * 397) ^ FindIsVisible        .GetHashCode();
				hashCode = (hashCode * 397) ^ AutoActionIsVisible  .GetHashCode();
				hashCode = (hashCode * 397) ^ AutoResponseIsVisible.GetHashCode();
			#if (WITH_SCRIPTING)
				hashCode = (hashCode * 397) ^ ScriptPanelIsVisible .GetHashCode();
			#endif

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as ViewSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(ViewSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				FindIsVisible        .Equals(other.FindIsVisible)          &&
				AutoActionIsVisible  .Equals(other.AutoActionIsVisible)    &&
			#if (!WITH_SCRIPTING)
				AutoResponseIsVisible.Equals(other.AutoResponseIsVisible)
			#else
				AutoResponseIsVisible.Equals(other.AutoResponseIsVisible)  &&
				ScriptPanelIsVisible .Equals(other.ScriptPanelIsVisible)
			#endif
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(ViewSettings lhs, ViewSettings rhs)
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
		public static bool operator !=(ViewSettings lhs, ViewSettings rhs)
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
