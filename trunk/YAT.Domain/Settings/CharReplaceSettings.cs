//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
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
	public class CharReplaceSettings : MKY.Utilities.Settings.Settings, IEquatable<CharReplaceSettings>
	{
		/// <summary></summary>
		public const bool ReplaceControlCharsDefault = true;
		/// <summary></summary>
		public const ControlCharRadix ControlCharRadixDefault = ControlCharRadix.AsciiMnemonic;
		/// <summary></summary>
		public const bool ReplaceSpaceDefault = false;
		/// <summary></summary>
		public const string ReplaceSpaceString = "␣";

		private bool _replaceControlChars;
		private ControlCharRadix _controlCharRadix;
		private bool _replaceSpace;

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
			_replaceControlChars = rhs.ReplaceControlChars;
			_controlCharRadix = rhs.ControlCharRadix;
			_replaceSpace = rhs.ReplaceSpace;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			ReplaceControlChars = ReplaceControlCharsDefault;
			ControlCharRadix = ControlCharRadixDefault;
			ReplaceSpace = ReplaceSpaceDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("ReplaceSpace")]
		public bool ReplaceSpace
		{
			get { return (_replaceSpace); }
			set
			{
				if (_replaceSpace != value)
				{
					_replaceSpace = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ReplaceControlChars")]
		public bool ReplaceControlChars
		{
			get { return (_replaceControlChars); }
			set
			{
				if (_replaceControlChars != value)
				{
					_replaceControlChars = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("ControlCharRadix")]
		public ControlCharRadix ControlCharRadix
		{
			get { return (_controlCharRadix); }
			set
			{
				if (_controlCharRadix != value)
				{
					_controlCharRadix = value;
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
			if (obj is CharReplaceSettings)
				return (Equals((CharReplaceSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(CharReplaceSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_replaceSpace.Equals       (value._replaceSpace) &&
					_replaceControlChars.Equals(value._replaceControlChars) &&
					_controlCharRadix.Equals   (value._controlCharRadix)
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
		public static bool operator ==(CharReplaceSettings lhs, CharReplaceSettings rhs)
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
		public static bool operator !=(CharReplaceSettings lhs, CharReplaceSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
