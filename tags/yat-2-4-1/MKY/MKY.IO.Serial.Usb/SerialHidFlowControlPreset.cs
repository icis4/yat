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
// MKY Version 1.0.30
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using MKY.IO.Usb;

#endregion

namespace MKY.IO.Serial.Usb
{
	#region Enum SerialHidFlowControlPreset

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SerialHidFlowControlPreset
	{
		None,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		MT_SerHid,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "YAT", Justification = "YAT is a name.")]
		YAT
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum SerialHidFlowControlPresetEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Semantic of readonly fields is constant.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class SerialHidFlowControlPresetEx : EnumEx
	{
		#region String Definitions

		private const string             None_string      =   "[No preset selected]";
		private static readonly string[] None_stringStart = { "No", "<No", "[No" }; // Covers "None", "<None", "[None".

		private const string             MT_SerHid_string      =   "MT Ser/HID";                           // OHAUS is MT's 2nd brand.
		private static readonly string[] MT_SerHid_stringStart = { "MT", "METTLER TOLEDO", "Mettler-Toledo", "OH" }; // Covers "OHAUS".

		private const string             YAT_string      =   "YAT default"; // Using "YAT" rather than "MKY" as that is the 3rd party
		private static readonly string[] YAT_stringStart = { "YAT" };       // where this preset originates from, as other 3rd party.

		/// <summary></summary>
		public const string UserSummary = @"""MT"", ""OH"" and ""YAT""";

		#endregion

		#region Constants

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		private static readonly int[] MT_SerHid_VendorIds = { 0x0EB8 }; // Covers "OHAUS", MT's 2nd brand.

		#endregion

		/// <summary>Default is <see cref="SerialHidFlowControlPreset.YAT"/>.</summary>
		public const SerialHidFlowControlPreset Default = SerialHidFlowControlPreset.YAT;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SerialHidFlowControlPresetEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public SerialHidFlowControlPresetEx(SerialHidFlowControlPreset preset)
			: base(preset)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public override string ToString()
		{
			switch ((SerialHidFlowControlPreset)UnderlyingEnum)
			{
				case SerialHidFlowControlPreset.None:      return (None_string);
				case SerialHidFlowControlPreset.MT_SerHid: return (MT_SerHid_string);
				case SerialHidFlowControlPreset.YAT:       return (YAT_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. view lists.
		/// </remarks>
		public static SerialHidFlowControlPresetEx[] GetItems()
		{
			var a = new List<SerialHidFlowControlPresetEx>(8); // Preset the required capacity to improve memory management.

			a.Add(new SerialHidFlowControlPresetEx(SerialHidFlowControlPreset.None));
			a.Add(new SerialHidFlowControlPresetEx(SerialHidFlowControlPreset.MT_SerHid));
			a.Add(new SerialHidFlowControlPresetEx(SerialHidFlowControlPreset.YAT));

			return (a.ToArray());
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialHidFlowControlPresetEx Parse(string s)
		{
			SerialHidFlowControlPresetEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid preset string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialHidFlowControlPresetEx result)
		{
			SerialHidFlowControlPreset enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new SerialHidFlowControlPresetEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialHidFlowControlPreset result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = SerialHidFlowControlPreset.None;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, None_stringStart))
			{
				result = SerialHidFlowControlPreset.None;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, MT_SerHid_stringStart))
			{
				result = SerialHidFlowControlPreset.MT_SerHid;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, YAT_stringStart))
			{
				result = SerialHidFlowControlPreset.YAT;
				return (true);
			}
			else // Invalid string!
			{
				result = new SerialHidFlowControlPresetEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region From
		//==========================================================================================
		// From
		//==========================================================================================

		/// <remarks>Returns <c>false</c> and <see cref="SerialHidFlowControlPreset.None"/> if no matching preset has been found.</remarks>
		public static bool TryFrom(DeviceInfo deviceInfo, out SerialHidFlowControlPresetEx result)
		{
			SerialHidFlowControlPreset enumResult;
			if (TryFrom(deviceInfo, out enumResult))
			{
				result = new SerialHidFlowControlPresetEx(enumResult);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		/// <remarks>Returns <c>false</c> and <see cref="SerialHidFlowControlPreset.None"/> if no matching preset has been found.</remarks>
		public static bool TryFrom(DeviceInfo deviceInfo, out SerialHidFlowControlPreset result)
		{
			if (Int32Ex.EqualsAny(deviceInfo.VendorId, MT_SerHid_VendorIds))
			{
				result = SerialHidFlowControlPreset.MT_SerHid;
				return (true);
			}
			else // Vendor ID not found, try parsing string:
			{
				return (TryParse(deviceInfo, out result));
			}
		}

		#endregion

		#region ToFlowControl
		//==========================================================================================
		// ToFlowControl
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public SerialHidFlowControl ToFlowControl()
		{
			switch ((SerialHidFlowControlPreset)UnderlyingEnum)
			{
				case SerialHidFlowControlPreset.None:            return (SerialHidFlowControl.None);
				case SerialHidFlowControlPreset.MT_SerHid:       return (SerialHidFlowControl.Software);
				case SerialHidFlowControlPreset.YAT:             return (SerialHidFlowControl.None); // = None.

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlPreset(SerialHidFlowControlPresetEx preset)
		{
			return ((SerialHidFlowControlPreset)preset.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlPresetEx(SerialHidFlowControlPreset preset)
		{
			return (new SerialHidFlowControlPresetEx(preset));
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControl(SerialHidFlowControlPresetEx preset)
		{
			return (preset.ToFlowControl());
		}

		/// <summary></summary>
		public static implicit operator int(SerialHidFlowControlPresetEx preset)
		{
			return (preset.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlPresetEx(int preset)
		{
			return (new SerialHidFlowControlPresetEx((SerialHidFlowControlPreset)preset));
		}

		/// <summary></summary>
		public static implicit operator string(SerialHidFlowControlPresetEx preset)
		{
			return (preset.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialHidFlowControlPresetEx(string preset)
		{
			return (Parse(preset));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
