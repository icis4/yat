//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/YAT.Domain/Settings/CharReplaceSettings.cs $
// $Author: maettu_this $
// $Date: 2010-08-29 15:51:39 +0200 (So, 29 Aug 2010) $
// $Revision: 306 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class CharReplaceSettings : MKY.Utilities.Settings.Settings
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
		public const string ReplaceSpaceString = "‣";

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
		public CharReplaceSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
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
				if (value != this.replaceControlChars)
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
				if (value != this.controlCharRadix)
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
				if (value != this.replaceTab)
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
				if (value != this.replaceSpace)
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

				(this.replaceControlChars == other.replaceControlChars) &&
				(this.controlCharRadix    == other.controlCharRadix) &&
				(this.replaceTab          == other.replaceTab) &&
				(this.replaceSpace        == other.replaceSpace)
			);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^

				this.replaceControlChars.GetHashCode() ^
				this.controlCharRadix   .GetHashCode() ^
				this.replaceTab         .GetHashCode() ^
				this.replaceSpace       .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Utilities.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/YAT.Domain/Settings/CharReplaceSettings.cs $
//==================================================================================================
