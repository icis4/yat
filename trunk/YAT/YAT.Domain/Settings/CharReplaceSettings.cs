//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class CharReplaceSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const bool ReplaceControlCharsDefault = true;

		/// <summary></summary>
		public const ControlCharRadix ControlCharRadixDefault = ControlCharRadix.AsciiMnemonic;

		/// <summary></summary>
		public const bool ReplaceTabDefault = false;

		/// <summary></summary>
		public const bool ReplaceSpaceDefault = false;

		/// <summary></summary>
		public const string SpaceReplaceChar = "␣";

		private bool replaceControlChars;
		private ControlCharRadix controlCharRadix;
		private bool replaceTab;
		private bool replaceSpace;

		/// <summary></summary>
		public CharReplaceSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public CharReplaceSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public CharReplaceSettings(CharReplaceSettings rhs)
			: base(rhs)
		{
			ReplaceControlChars = rhs.ReplaceControlChars;
			ControlCharRadix    = rhs.ControlCharRadix;
			ReplaceTab          = rhs.ReplaceTab;
			ReplaceSpace        = rhs.ReplaceSpace;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			ReplaceControlChars = ReplaceControlCharsDefault;
			ControlCharRadix    = ControlCharRadixDefault;
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
				}
			}
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

			CharReplaceSettings other = (CharReplaceSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(ReplaceControlChars == other.ReplaceControlChars) &&
				(ControlCharRadix    == other.ControlCharRadix) &&
				(ReplaceTab          == other.ReplaceTab) &&
				(ReplaceSpace        == other.ReplaceSpace)
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

				hashCode = (hashCode * 397) ^ ReplaceControlChars.GetHashCode();
				hashCode = (hashCode * 397) ^ ControlCharRadix   .GetHashCode();
				hashCode = (hashCode * 397) ^ ReplaceTab         .GetHashCode();
				hashCode = (hashCode * 397) ^ ReplaceSpace       .GetHashCode();

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
