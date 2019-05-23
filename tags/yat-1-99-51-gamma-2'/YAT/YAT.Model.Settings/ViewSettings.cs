﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Version 1.99.50
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

using MKY.Recent;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	public class ViewSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const int MaxCustomColors = 16;

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
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public ViewSettings(ViewSettings rhs)
			: base(rhs)
		{
			CustomColors = new RecentItemCollection<string>(rhs.CustomColors);

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			CustomColors = new RecentItemCollection<string>(MaxCustomColors);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
			List<int> l = new List<int>(this.customColors.Count);

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

			ViewSettings other = (ViewSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(CustomColors == other.CustomColors)
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
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ (CustomColors != null ? CustomColors.GetHashCode() : 0);

				return (hashCode);
			}
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