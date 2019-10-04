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
// MKY Version 1.0.27
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
	#region Enum SerialHidDeviceSettingsPreset

	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum SerialHidDeviceSettingsPreset
	{
		None,
		Plain,
		Common,
		LengthIndicated,
		ZeroTerminated,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		MT_SerHid,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		TI_HidApi,

		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		Signal11_HidApi,

		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "YAT", Justification = "As always, there are exceptions to the rules...")]
		YAT
	}

	#pragma warning restore 1591

	#endregion

	/// <summary>
	/// Extended enum SerialHidDeviceSettingsPresetEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1203:ConstantsMustAppearBeforeFields", Justification = "Order according to meaning.")]
	[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class SerialHidDeviceSettingsPresetEx : EnumEx
	{
		#region String Definitions

		private const string             None_string      =   "[No preset selected]";
		private static readonly string[] None_stringStart = { "No", "<No", "[No" }; // Covers "None", "<None", "[None".

		private const string             Plain_string      =   "Plain (payload only)";
		private static readonly string[] Plain_stringStart = { "Plain", "<Plain", "[Plain" };

		private const string             Common_string      =   "Common (report ID + payload)";
		private static readonly string[] Common_stringStart = { "Common", "<Common", "[Common" };

		private const string             LengthIndicated_string      =   "Length indicated (report ID + length + payload)";
		private static readonly string[] LengthIndicated_stringStart = { "Length", "<Length", "[Length" };

		private const string             ZeroTerminated_string      =   "Zero terminated (report ID + payload + zero)";
		private static readonly string[] ZeroTerminated_stringStart = { "Zero", "<Zero", "[Zero" };

		private const string             MT_SerHid_string      =   "MT Ser/HID (same as [Zero terminated])";
		private static readonly string[] MT_SerHid_stringStart = { "MT", "METTLER TOLEDO", "Mettler-Toledo", "OH" }; // Covers "OHAUS", MT's 2nd brand.

		private const string             Signal11_HidApi_string      =   "Signal 11 HID API";
		private static readonly string[] Signal11_HidApi_stringStart = { "Signal" }; // Covers "Signal11" and "Signal 11".

		private const string             TI_HidApi_string      =   "TI HID API";                // Note that comparison will be done with 'OrdinalIgnoreCase'.
		private static readonly string[] TI_HidApi_stringStart = { "TI", "Texas Instruments" }; // Also note that comparison could be improved such as e.g.
		                                                                                        // strings "Texas" and "Instruments" are individually compared.
		private const string             YAT_string      =   "YAT default (resulting in [Common])";
		private static readonly string[] YAT_stringStart = { "YAT" };

		/// <summary></summary>
		public const string UserSummary = @"""Plain"", ""Length"", ""Zero"", ""MT"", ""OH"", ""Signal11"" , ""TI"" and ""YAT""";

		#endregion

		#region Constants

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "An URL may spell 'ti' like this, and may contain 'slaa'...")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		public const byte MT_SerHid_ReportId = 0x00;

	////private const string MT_SerHid_LinkText = "METTER TOLEDO Ser/HID Report Format";
	////private const string MT_SerHid_LinkUri => Requested as RB DCR #4671

		/// <summary></summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "An URL may spell 'ti' like this, and may contain 'slaa'...")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		public const byte Signal11_HidApi_DefaultReportId = 0x00;

		private const string Signal11_HidApi_LinkText = "Signal 11 HID API";
		private const string Signal11_HidApi_LinkUri = "http://www.signal11.us/oss/hidapi/";

		/// <summary>
		/// The TI HID API identifier is specified to 0x3F according to http://www.ti.com/lit/an/slaa453/slaa453.pdf.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "An URL may spell 'ti' like this, and may contain 'slaa'...")]
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Underscore for improved readability.")]
		public const byte TI_HidApi_ReportId = 0x3F;

		private const string TI_HidApi_LinkText = "TI Application Report SLAA453";
		private const string TI_HidApi_LinkUri = "http://www.ti.com/lit/an/slaa453/slaa453.pdf";

		#endregion

		/// <summary>Default is <see cref="SerialHidDeviceSettingsPreset.YAT"/>.</summary>
		public const SerialHidDeviceSettingsPreset Default = SerialHidDeviceSettingsPreset.YAT;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public SerialHidDeviceSettingsPresetEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		public SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset preset)
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
			switch ((SerialHidDeviceSettingsPreset)UnderlyingEnum)
			{
				case SerialHidDeviceSettingsPreset.None:            return (None_string);
				case SerialHidDeviceSettingsPreset.Plain:           return (Plain_string);
				case SerialHidDeviceSettingsPreset.Common:          return (Common_string);
				case SerialHidDeviceSettingsPreset.LengthIndicated: return (LengthIndicated_string);
				case SerialHidDeviceSettingsPreset.ZeroTerminated:  return (ZeroTerminated_string);
				case SerialHidDeviceSettingsPreset.MT_SerHid:       return (MT_SerHid_string);
				case SerialHidDeviceSettingsPreset.Signal11_HidApi: return (Signal11_HidApi_string);
				case SerialHidDeviceSettingsPreset.TI_HidApi:       return (TI_HidApi_string);
				case SerialHidDeviceSettingsPreset.YAT:             return (YAT_string);

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		#endregion

		#region InfoLink
		//==========================================================================================
		// InfoLink
		//==========================================================================================

		/// <summary>
		/// Returns whether link information is available.
		/// </summary>
		public bool HasInfoLink()
		{
			string linkText;
			string linkUri;
			return (HasInfoLink(out linkText, out linkUri));
		}

		/// <summary>
		/// Returns link information if available.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public bool HasInfoLink(out string linkUri)
		{
			string linkText;
			return (HasInfoLink(out linkText, out linkUri));
		}

		/// <summary>
		/// Returns link information if available.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public bool HasInfoLink(out string linkText, out string linkUri)
		{
			switch ((SerialHidDeviceSettingsPreset)UnderlyingEnum)
			{
				case SerialHidDeviceSettingsPreset.Signal11_HidApi:
				{
					linkText = Signal11_HidApi_LinkText;
					linkUri  = Signal11_HidApi_LinkUri;
					return (true);
				}

				case SerialHidDeviceSettingsPreset.TI_HidApi:
				{
					linkText = TI_HidApi_LinkText;
					linkUri  = TI_HidApi_LinkUri;
					return (true);
				}

				default:
				{
					linkText = null;
					linkUri  = null;
					return (false);
				}
			}
		}

		#endregion

		#region GetItems
		//==========================================================================================
		// GetItems
		//==========================================================================================

		/// <remarks>
		/// An array of extended enum items is returned for more versatile use, e.g. UI controls lists.
		/// </remarks>
		public static SerialHidDeviceSettingsPresetEx[] GetItems()
		{
			var a = new List<SerialHidDeviceSettingsPresetEx>(8); // Preset the required capacity to improve memory management.

			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.None));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.Plain));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.Common));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.LengthIndicated));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.ZeroTerminated));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.MT_SerHid));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.Signal11_HidApi));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.TI_HidApi));
			a.Add(new SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset.YAT));

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
		public static SerialHidDeviceSettingsPresetEx Parse(string s)
		{
			SerialHidDeviceSettingsPresetEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid preset string! String must be one of the underlying enumeration designations."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out SerialHidDeviceSettingsPresetEx result)
		{
			SerialHidDeviceSettingsPreset enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new SerialHidDeviceSettingsPresetEx(enumResult);
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
		public static bool TryParse(string s, out SerialHidDeviceSettingsPreset result)
		{
			if (s != null)
				s = s.Trim();

			if (string.IsNullOrEmpty(s)) // None!
			{
				result = SerialHidDeviceSettingsPreset.None;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, None_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.None;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, Plain_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.Plain;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, Common_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.Common;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, LengthIndicated_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.LengthIndicated;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, ZeroTerminated_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.ZeroTerminated;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, MT_SerHid_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.MT_SerHid;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, Signal11_HidApi_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.Signal11_HidApi;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, TI_HidApi_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.TI_HidApi;
				return (true);
			}
			else if (StringEx.StartsWithAnyOrdinalIgnoreCase(s, YAT_stringStart))
			{
				result = SerialHidDeviceSettingsPreset.YAT;
				return (true);
			}
			else // Invalid string!
			{
				result = new SerialHidDeviceSettingsPresetEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region To/From ReportFormat/RxFilterUsage
		//==========================================================================================
		// To/From ReportFormat/RxFilterUsage
		//==========================================================================================

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public SerialHidReportFormat ToReportFormat()
		{
			switch ((SerialHidDeviceSettingsPreset)UnderlyingEnum)
			{
				case SerialHidDeviceSettingsPreset.None:            return (new SerialHidReportFormat(true,  0x00,                            false, false, true )); // = Common.
				case SerialHidDeviceSettingsPreset.Plain:           return (new SerialHidReportFormat(false, 0x00,                            false, false, true ));
				case SerialHidDeviceSettingsPreset.Common:          return (new SerialHidReportFormat(true,  0x00,                            false, false, true ));
				case SerialHidDeviceSettingsPreset.LengthIndicated: return (new SerialHidReportFormat(true,  0x00,                            true,  false, true ));
				case SerialHidDeviceSettingsPreset.ZeroTerminated:  return (new SerialHidReportFormat(true,  0x00,                            false, true,  true ));
				case SerialHidDeviceSettingsPreset.MT_SerHid:       return (new SerialHidReportFormat(true,  MT_SerHid_ReportId,              false, true,  true )); // = ZeroTerminated since report ID = 0x00.
				case SerialHidDeviceSettingsPreset.Signal11_HidApi: return (new SerialHidReportFormat(true,  Signal11_HidApi_DefaultReportId, false, false, true )); // = Common since default report ID = 0x00.
				case SerialHidDeviceSettingsPreset.TI_HidApi:       return (new SerialHidReportFormat(true,  TI_HidApi_ReportId,              true,  false, true ));
				case SerialHidDeviceSettingsPreset.YAT:             return (new SerialHidReportFormat(true,  0x00,                            false, false, true )); // = Common.

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Indication of a fatal bug that shall be reported but cannot be easily handled with 'Debug|Trace.Assert()'.")]
		public SerialHidRxFilterUsage ToRxFilterUsage()
		{
			switch ((SerialHidDeviceSettingsPreset)UnderlyingEnum)
			{
				case SerialHidDeviceSettingsPreset.None:            return (new SerialHidRxFilterUsage(false, false, 0x00 )); // = Common.
				case SerialHidDeviceSettingsPreset.Plain:           return (new SerialHidRxFilterUsage(false, false, 0x00 ));
				case SerialHidDeviceSettingsPreset.Common:          return (new SerialHidRxFilterUsage(false, false, 0x00 ));
				case SerialHidDeviceSettingsPreset.LengthIndicated: return (new SerialHidRxFilterUsage(false, false, 0x00 ));
				case SerialHidDeviceSettingsPreset.ZeroTerminated:  return (new SerialHidRxFilterUsage(false, false, 0x00 ));
				case SerialHidDeviceSettingsPreset.MT_SerHid:       return (new SerialHidRxFilterUsage(false, false, MT_SerHid_ReportId ));
				case SerialHidDeviceSettingsPreset.Signal11_HidApi: return (new SerialHidRxFilterUsage(true,  true,  Signal11_HidApi_DefaultReportId ));
				case SerialHidDeviceSettingsPreset.TI_HidApi:       return (new SerialHidRxFilterUsage(false, false, TI_HidApi_ReportId ));
				case SerialHidDeviceSettingsPreset.YAT:             return (new SerialHidRxFilterUsage(false, false, 0x00 )); // = Common.

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + UnderlyingEnum.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public static SerialHidDeviceSettingsPreset FromReportFormatAndRxFilterUsage(SerialHidReportFormat reportFormat, SerialHidRxFilterUsage rxFilterUsage)
		{
			foreach (var preset in GetItems())
			{
				if (preset != SerialHidDeviceSettingsPreset.None)
				{
					if ((reportFormat  == preset.ToReportFormat()) &&
						(rxFilterUsage == preset.ToRxFilterUsage()))
					{
						return (preset);
					}
				}
			}

			// Not found:
			return (SerialHidDeviceSettingsPreset.None);
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		public static implicit operator SerialHidDeviceSettingsPreset(SerialHidDeviceSettingsPresetEx preset)
		{
			return ((SerialHidDeviceSettingsPreset)preset.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator SerialHidDeviceSettingsPresetEx(SerialHidDeviceSettingsPreset preset)
		{
			return (new SerialHidDeviceSettingsPresetEx(preset));
		}

		/// <summary></summary>
		public static implicit operator SerialHidReportFormat(SerialHidDeviceSettingsPresetEx preset)
		{
			return (preset.ToReportFormat());
		}

		/// <summary></summary>
		public static implicit operator SerialHidRxFilterUsage(SerialHidDeviceSettingsPresetEx preset)
		{
			return (preset.ToRxFilterUsage());
		}

		/// <summary></summary>
		public static implicit operator int(SerialHidDeviceSettingsPresetEx preset)
		{
			return (preset.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator SerialHidDeviceSettingsPresetEx(int preset)
		{
			return (new SerialHidDeviceSettingsPresetEx((SerialHidDeviceSettingsPreset)preset));
		}

		/// <summary></summary>
		public static implicit operator string(SerialHidDeviceSettingsPresetEx preset)
		{
			return (preset.ToString());
		}

		/// <summary></summary>
		public static implicit operator SerialHidDeviceSettingsPresetEx(string preset)
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
