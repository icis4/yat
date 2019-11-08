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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Xml.Serialization;

using MKY.Collections;
using MKY.Collections.Specialized;

namespace YAT.Model.Settings
{
	/// <remarks>
	/// \remind (2017-11-19 / MKY), (2019-08-02 / MKY)
	/// Should be migrated to a separate 'YAT.Application.Settings' project. Not easily possible
	/// because of dependencies among 'YAT.*' and 'YAT.Application', e.g. 'ExtensionSettings'.
	/// Requires slight refactoring of project dependencies. Could be done when refactoring the
	/// projects on integration with Albatros.
	/// </remarks>
	public class ViewSettings : MKY.Settings.SettingsItem, IEquatable<ViewSettings>
	{
		/// <summary></summary>
		public const int MaxCustomColors = 16;

		private bool findIsVisible;
		private bool autoActionIsVisible;
		private bool autoResponseIsVisible;
	#if (WITH_SCRIPTING)
		private bool scriptPanelIsVisible;
	#endif

		private RecentItemCollection<string> customColors;

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

			CustomColors = new RecentItemCollection<string>(rhs.CustomColors);

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

			CustomColors = new RecentItemCollection<string>(MaxCustomColors);
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

		/// <remarks>
		/// Using string because...
		/// ...<see cref="Color"/> does not implement <see cref="IEquatable{T}"/> that is needed for a recent item collection, and because...
		/// ...<see cref="Color"/> cannot be serialized.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Public getter is required for default XML serialization/deserialization.")]
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Public setter is required for default XML serialization/deserialization.")]
		[XmlElement("CustomColors")]
		public RecentItemCollection<string> CustomColors
		{
			get { return (this.customColors); }
			set
			{
				if (this.customColors != value)
				{
					this.customColors = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		public int[] CustomColorsToWin32()
		{
			var l = new List<int>(this.customColors.Count);

			foreach (RecentItem<string> ri in this.customColors)
			{
				Color c = ColorTranslator.FromHtml(ri.Item);
				int win32 = ColorTranslator.ToWin32(c);
				l.Add(win32);
			}

			return (l.ToArray());
		}

		/// <summary></summary>
		public bool UpdateCustomColorsFromWin32(int[] customColors)
		{
			// Do not add 'White', as that...
			// ...is the default color, and...
			// ...is available predefined anyway.
			int win32White = ColorTranslator.ToWin32(Color.White);

			List<string> otherThanWhite = new List<string>(customColors.Length);

			foreach (int win32 in customColors)
			{
				if (win32 != win32White)
				{
					Color c = ColorTranslator.FromWin32(win32);
					string html = ColorTranslator.ToHtml(c);
					otherThanWhite.Add(html);
				}
			}

			return (this.customColors.UpdateFrom(otherThanWhite));
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

				hashCode = (hashCode * 397) ^ FindIsVisible        .GetHashCode();
				hashCode = (hashCode * 397) ^ AutoActionIsVisible  .GetHashCode();
				hashCode = (hashCode * 397) ^ AutoResponseIsVisible.GetHashCode();
			#if (WITH_SCRIPTING)
				hashCode = (hashCode * 397) ^ ScriptPanelIsVisible .GetHashCode();
			#endif

				hashCode = (hashCode * 397) ^ (CustomColors != null ? CustomColors.GetHashCode() : 0);

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

				FindIsVisible        .Equals(other.FindIsVisible)         &&
				AutoActionIsVisible  .Equals(other.AutoActionIsVisible)   &&
				AutoResponseIsVisible.Equals(other.AutoResponseIsVisible) &&
			#if (WITH_SCRIPTING)
				ScriptPanelIsVisible .Equals(other.ScriptPanelIsVisible)  &&
			#endif

				IEnumerableEx.ItemsEqual(CustomColors, other.CustomColors)
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
