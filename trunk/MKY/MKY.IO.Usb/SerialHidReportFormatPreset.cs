//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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

#endregion

namespace MKY.IO.Usb
{
	#region Enum SerialHidReportFormatPreset

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SerialHidReportFormatPreset
	{
		None,
		Plain,
		Common,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		MT_SerHid,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		TI_HidApi,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "YAT", Justification = "As always, there are exceptions to the rules...")]
		YAT
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum SerialHidReportFormatPresetEx.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order of 'const' and 'readonly' according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	[Serializable]
	public class SerialHidReportFormatPresetEx : EnumEx
	{
		#region String Definitions

		private const string             None_string      =   "<No preset selected>";
		private static readonly string[] None_stringStart = { "No", "<No" }; // Covers "None", "<None"

		private const string             Plain_string      =   "Plain = Payload only";
		private static readonly string[] Plain_stringStart = { "Plain", "<Plain" };

		private const string             Common_string      =   "Common = ID + Payload";
		private static readonly string[] Common_stringStart = { "Common", "<Common" };

		private const string             MT_SerHid_string      =   "MT Ser/HID";
		private static readonly string[] MT_SerHid_stringStart = { "MT", "METTLER TOLEDO", "Mettler-Toledo" };

		private const string             TI_HidApi_string      =   "TI HID API";
		private static readonly string[] TI_HidApi_stringStart = { "TI", "Texas Instruments" };

		private const string             YAT_string      =   "YAT default";
		private static readonly string[] YAT_stringStart = { "YAT" };

		/// <summary></summary>
		public const string UserSummary = @"""Plain"", ""Common"", ""MT"", ""TI"" and ""YAT""";

		#endregion

		#region Constants

		/// <summary>
		/// The TI HID API identifier is specified to 0x3F according to http://www.ti.com/lit/an/slaa453/slaa453.pdf.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "An URL may spell 'ti' like this, and may contain 'slaa'...")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Underscore for improved readability.")]
		public const byte TI_ID = 0x3F;

		#endregion

		/// <summary>Default is <see cref="SerialHidReportFormatPreset.YAT"/>.</summary>
		public SerialHidReportFormatPresetEx()
			: base(SerialHidReportFormatPreset.YAT)
		{
		}

		/// <summary></summary>
		protected SerialHidReportFormatPresetEx(SerialHidReportFormatPreset preset)
			: base(preset)
		{
		}

		#region ToString

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public override string ToString()
		{
			switch ((SerialHidReportFormatPreset)UnderlyingEnum)
			{
				case SerialHidReportFormatPreset.None:      return (None_string);
				case SerialHidReportFormatPreset.Plain:     return (Plain_string);
				case SerialHidReportFormatPreset.Common:    return (Common_string);
				case SerialHidReportFormatPreset.MT_SerHid: return (MT_SerHid_string);
				case SerialHidReportFormatPreset.TI_HidApi: return (TI_HidApi_string);
				case SerialHidReportFormatPreset.YAT:       return (YAT_string);
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region GetItems

		/// <summary></summary>
		public static SerialHidReportFormatPresetEx[] GetItems()
		{
			List<SerialHidReportFormatPresetEx> a = new List<SerialHidReportFormatPresetEx>();
			a.Add(new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.None));
			a.Add(new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.Plain));
			a.Add(new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.Common));
			a.Add(new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.MT_SerHid));
			a.Add(new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.TI_HidApi));
			a.Add(new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.YAT));
			return (a.ToArray());
		}

		#endregion

		#region Parse

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static SerialHidReportFormatPresetEx Parse(string s)
		{
			SerialHidReportFormatPresetEx result;
			if (TryParse(s, out result))
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is no valid preset string."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialHidReportFormatPresetEx result)
		{
			s = s.Trim();

			if      (StringEx.StartsWithAny(s, None_stringStart, StringComparison.OrdinalIgnoreCase))
			{
				result = new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.None);
				return (true);
			}
			else if (StringEx.StartsWithAny(s, Plain_stringStart, StringComparison.OrdinalIgnoreCase))
			{
				result = new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.Plain);
				return (true);
			}
			else if (StringEx.StartsWithAny(s, Common_stringStart, StringComparison.OrdinalIgnoreCase))
			{
				result = new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.Common);
				return (true);
			}
			else if (StringEx.StartsWithAny(s, MT_SerHid_stringStart, StringComparison.OrdinalIgnoreCase))
			{
				result = new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.MT_SerHid);
				return (true);
			}
			else if (StringEx.StartsWithAny(s, TI_HidApi_stringStart, StringComparison.OrdinalIgnoreCase))
			{
				result = new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.TI_HidApi);
				return (true);
			}
			else if (StringEx.StartsWithAny(s, YAT_stringStart, StringComparison.OrdinalIgnoreCase))
			{
				result = new SerialHidReportFormatPresetEx(SerialHidReportFormatPreset.YAT);
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region ToReportFormat

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "The exception indicates a fatal bug that shall be reported.")]
		public SerialHidReportFormat ToReportFormat()
		{
			switch ((SerialHidReportFormatPreset)UnderlyingEnum)
			{
				case SerialHidReportFormatPreset.None:		return (new SerialHidReportFormat(true,  0x00,  false, false, true )); // = Common.
				case SerialHidReportFormatPreset.Plain:		return (new SerialHidReportFormat(false, 0x00,  false, false, true ));
				case SerialHidReportFormatPreset.Common:	return (new SerialHidReportFormat(true,  0x00,  false, false, true ));
				case SerialHidReportFormatPreset.MT_SerHid:	return (new SerialHidReportFormat(true,  0x00,  false, true,  true ));
				case SerialHidReportFormatPreset.TI_HidApi: return (new SerialHidReportFormat(true,  TI_ID, true,  false, true ));
				case SerialHidReportFormatPreset.YAT:		return (new SerialHidReportFormat(true,  0x00,  false, false, true )); // = Common.
			}
			throw (new InvalidOperationException("Program execution should never get here, item " + UnderlyingEnum.ToString() + " is unknown, please report this bug!"));
		}

		#endregion

		#region FromReportFormat

		/// <summary></summary>
		public static SerialHidReportFormatPreset FromReportFormat(SerialHidReportFormat format)
		{
			return (FromReportFormat(format.UseId, format.Id, format.PrependPayloadByteLength, format.AppendTerminatingZero, format.FillLastReport));
		}

		/// <summary></summary>
		public static SerialHidReportFormatPreset FromReportFormat(bool useId, byte id, bool prependPayloadByteLength, bool appendTerminatingZero, bool fillLastReport)
		{
			if (!useId && (id == 0x00)  && !prependPayloadByteLength && !appendTerminatingZero &&  fillLastReport) return (SerialHidReportFormatPreset.Plain);
			if ( useId && (id == 0x00)  && !prependPayloadByteLength && !appendTerminatingZero &&  fillLastReport) return (SerialHidReportFormatPreset.Common);
			if ( useId && (id == 0x00)  && !prependPayloadByteLength &&  appendTerminatingZero &&  fillLastReport) return (SerialHidReportFormatPreset.MT_SerHid);
			if ( useId && (id == TI_ID) &&  prependPayloadByteLength && !appendTerminatingZero &&  fillLastReport) return (SerialHidReportFormatPreset.TI_HidApi);
			
			// Any other case:
			return (SerialHidReportFormatPreset.None);
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator SerialHidReportFormatPreset(SerialHidReportFormatPresetEx formatPreset)
		{
			return ((SerialHidReportFormatPreset)formatPreset.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialHidReportFormatPresetEx(SerialHidReportFormatPreset formatPreset)
		{
			return (new SerialHidReportFormatPresetEx(formatPreset));
		}

		/// <summary></summary>
		public static implicit operator SerialHidReportFormat(SerialHidReportFormatPresetEx formatPreset)
		{
			return (formatPreset.ToReportFormat());
		}

		/// <summary></summary>
		public static implicit operator SerialHidReportFormatPresetEx(SerialHidReportFormat format)
		{
			return (FromReportFormat(format));
		}

		/// <summary></summary>
		public static implicit operator int(SerialHidReportFormatPresetEx formatPreset)
		{
			return (formatPreset.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialHidReportFormatPresetEx(int formatPreset)
		{
			return (new SerialHidReportFormatPresetEx((SerialHidReportFormatPreset)formatPreset));
		}

		/// <summary></summary>
		public static implicit operator string(SerialHidReportFormatPresetEx formatPreset)
		{
			return (formatPreset.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialHidReportFormatPresetEx(string formatPreset)
		{
			return (Parse(formatPreset));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
