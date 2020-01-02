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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class CharReplaceSettings : MKY.Settings.SettingsItem, IEquatable<CharReplaceSettings>
	{
		/// <remarks>
		/// Default is <see cref="ReplaceControlCharsBinaryDefault"/> as its value of
		/// <c>false</c> is less intrusive.
		/// </remarks>
		public const bool ReplaceControlCharsDefault = ReplaceControlCharsBinaryDefault;

		/// <summary></summary>
		public const bool ReplaceControlCharsTextDefault = true;

		/// <summary></summary>
		public const bool ReplaceControlCharsBinaryDefault = false;

		/// <summary></summary>
		public const ControlCharRadix ControlCharRadixDefault = ControlCharRadix.AsciiMnemonic;

		/// <summary></summary>
		public const bool ReplaceBackspaceDefault = false;

		/// <summary></summary>
		public const bool ReplaceTabDefault = false;

		/// <summary></summary>
		public const bool ReplaceSpaceDefault = false;

		/// <summary></summary>
		public const string SpaceReplaceChar = "␣";

		private bool replaceControlChars;
		private ControlCharRadix controlCharRadix;
		private bool replaceBackspace;
		private bool replaceTab;
		private bool replaceSpace;

		/// <summary></summary>
		public CharReplaceSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public CharReplaceSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public CharReplaceSettings(CharReplaceSettings rhs)
			: base(rhs)
		{
			ReplaceControlChars = rhs.ReplaceControlChars;
			ControlCharRadix    = rhs.ControlCharRadix;
			ReplaceBackspace    = rhs.ReplaceBackspace;
			ReplaceTab          = rhs.ReplaceTab;
			ReplaceSpace        = rhs.ReplaceSpace;

			ClearChanged();
		}

		/// <remarks>
		/// Fields are assigned via properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ReplaceControlChars = ReplaceControlCharsDefault;
			ControlCharRadix    = ControlCharRadixDefault;
			ReplaceBackspace    = ReplaceBackspaceDefault;
			ReplaceTab          = ReplaceTabDefault;
			ReplaceSpace        = ReplaceSpaceDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("ReplaceControlChars")]
		public virtual bool ReplaceControlChars
		{
			get { return (this.replaceControlChars); }
			set
			{
				if (this.replaceControlChars != value)
				{
					this.replaceControlChars = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ControlCharRadix")]
		public virtual ControlCharRadix ControlCharRadix
		{
			get { return (this.controlCharRadix); }
			set
			{
				if (this.controlCharRadix != value)
				{
					this.controlCharRadix = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ReplaceBackspace")]
		public virtual bool ReplaceBackspace
		{
			get { return (this.replaceBackspace); }
			set
			{
				if (this.replaceBackspace != value)
				{
					this.replaceBackspace = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ReplaceTab")]
		public virtual bool ReplaceTab
		{
			get { return (this.replaceTab); }
			set
			{
				if (this.replaceTab != value)
				{
					this.replaceTab = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ReplaceSpace")]
		public virtual bool ReplaceSpace
		{
			get { return (this.replaceSpace); }
			set
			{
				if (this.replaceSpace != value)
				{
					this.replaceSpace = value;
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

				hashCode = (hashCode * 397) ^ ReplaceControlChars.GetHashCode();
				hashCode = (hashCode * 397) ^ ControlCharRadix   .GetHashCode();
				hashCode = (hashCode * 397) ^ ReplaceBackspace   .GetHashCode();
				hashCode = (hashCode * 397) ^ ReplaceTab         .GetHashCode();
				hashCode = (hashCode * 397) ^ ReplaceSpace       .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as CharReplaceSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(CharReplaceSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				ReplaceControlChars.Equals(other.ReplaceControlChars) &&
				ControlCharRadix   .Equals(other.ControlCharRadix)    &&
				ReplaceBackspace   .Equals(other.ReplaceBackspace)    &&
				ReplaceTab         .Equals(other.ReplaceTab)          &&
				ReplaceSpace       .Equals(other.ReplaceSpace)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(CharReplaceSettings lhs, CharReplaceSettings rhs)
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
		public static bool operator !=(CharReplaceSettings lhs, CharReplaceSettings rhs)
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
