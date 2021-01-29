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
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class CharHideSettings : MKY.Settings.SettingsItem, IEquatable<CharHideSettings>
	{
		/// <summary></summary>
		public const bool HideXOnXOffDefault = false;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ult", Justification = "?!?")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ult", Justification = "?!?")]
		public const bool Hide0x00Default = false;

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "ult", Justification = "?!?")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ult", Justification = "?!?")]
		public const bool Hide0xFFDefault = false;

		private bool hideXOnXOff;

		private bool hide0x00;
		private bool hide0xFF;

		/// <summary></summary>
		public CharHideSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public CharHideSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public CharHideSettings(CharHideSettings rhs)
			: base(rhs)
		{
			HideXOnXOff = rhs.HideXOnXOff;

			Hide0x00 = rhs.Hide0x00;
			Hide0xFF = rhs.Hide0xFF;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			HideXOnXOff = HideXOnXOffDefault;

			Hide0x00 = Hide0x00Default;
			Hide0xFF = Hide0xFFDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("HideXOnXOff")]
		public virtual bool HideXOnXOff
		{
			get { return (this.hideXOnXOff); }
			set
			{
				if (this.hideXOnXOff != value)
				{
					this.hideXOnXOff = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Hide0x00")]
		public virtual bool Hide0x00
		{
			get { return (this.hide0x00); }
			set
			{
				if (this.hide0x00 != value)
				{
					this.hide0x00 = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("Hide0xFF")]
		public virtual bool Hide0xFF
		{
			get { return (this.hide0xFF); }
			set
			{
				if (this.hide0xFF != value)
				{
					this.hide0xFF = value;
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

				hashCode = (hashCode * 397) ^ HideXOnXOff.GetHashCode();

				hashCode = (hashCode * 397) ^ Hide0x00.GetHashCode();
				hashCode = (hashCode * 397) ^ Hide0xFF.GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as CharHideSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(CharHideSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				HideXOnXOff.Equals(other.HideXOnXOff) &&

				Hide0x00.Equals(other.Hide0x00) &&
				Hide0xFF.Equals(other.Hide0xFF)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(CharHideSettings lhs, CharHideSettings rhs)
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
		public static bool operator !=(CharHideSettings lhs, CharHideSettings rhs)
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
