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
	public class FontSettings : MKY.Settings.SettingsItem, IEquatable<FontSettings>
	{
		/// <summary></summary>
		public const bool CheckAvailabilityDefault = true;

		/// <summary></summary>
		public const bool CheckTerminalDefault = true;

		/// <summary></summary>
		public const bool ShowMonospaceOnlyDefault = true;

		private bool checkAvailability;
		private bool checkTerminal;
		private bool showMonospaceOnly;

		/// <summary></summary>
		public FontSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public FontSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public FontSettings(FontSettings rhs)
			: base(rhs)
		{
			CheckAvailability = rhs.CheckAvailability;
			CheckTerminal     = rhs.CheckTerminal;
			ShowMonospaceOnly = rhs.ShowMonospaceOnly;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			CheckAvailability = CheckAvailabilityDefault;
			CheckTerminal     = CheckTerminalDefault;
			ShowMonospaceOnly = ShowMonospaceOnlyDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("CheckAvailability")]
		public virtual bool CheckAvailability
		{
			get { return (this.checkAvailability); }
			set
			{
				if (this.checkAvailability != value)
				{
					this.checkAvailability = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CheckTerminal")]
		public virtual bool CheckTerminal
		{
			get { return (this.checkTerminal); }
			set
			{
				if (this.checkTerminal != value)
				{
					this.checkTerminal = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ShowMonospaceOnly")]
		public virtual bool ShowMonospaceOnly
		{
			get { return (this.showMonospaceOnly); }
			set
			{
				if (this.showMonospaceOnly != value)
				{
					this.showMonospaceOnly = value;
					SetMyChanged();
				}
			}
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

				hashCode = (hashCode * 397) ^ CheckAvailability.GetHashCode();
				hashCode = (hashCode * 397) ^ CheckTerminal    .GetHashCode();
				hashCode = (hashCode * 397) ^ ShowMonospaceOnly.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as FontSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(FontSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				CheckAvailability.Equals(other.CheckAvailability) &&
				CheckTerminal    .Equals(other.CheckTerminal)     &&
				ShowMonospaceOnly.Equals(other.ShowMonospaceOnly)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(FontSettings lhs, FontSettings rhs)
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
		public static bool operator !=(FontSettings lhs, FontSettings rhs)
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
