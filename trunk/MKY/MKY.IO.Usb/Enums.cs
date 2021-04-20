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
// Copyright © 2010-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
// warnings for each undocumented member below. Documenting each member makes little sense
// since they pretty much tell their purpose and documentation tags between the members
// makes the code less readable.
#pragma warning disable 1591

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.IO.Usb
{
	/// <summary>
	/// USB device classes as defined by the USB standard.
	/// See http://www.usb.org/developers/defined_class/ for latest list.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "An URL may spell 'usb' like this...")]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
	public enum DeviceClass : byte // 8 bits!
	{
		DeviceSpecific      = 0x00,

		Audio               = 0x01,
		CdcControl          = 0x02,
		Hid                 = 0x03,
		////                  0x04 is reserved
		Physical            = 0x05,
		Image               = 0x06,
		Printer             = 0x07,
		MassStorage         = 0x08,
		Hub                 = 0x09,
		CdcData             = 0x0A,
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SmartCard", Justification = "Item names are given by USB.")]
		SmartCard           = 0x0B,
		////                  0x0C is reserved
		ContentSecurity     = 0x0D,
		Video               = 0x0E,
		PersonalHealthcare  = 0x0F,
		AudioVideo          = 0x10,

		DiagnosticDevice    = 0xDC,

		WirelessController  = 0xE0,
		Miscellaneous       = 0xEF,

		ApplicationSpecific = 0xFE,
		VendorSpecific      = 0xFF,

		Any                 = 0x00
	}

	/// <summary>
	/// Descriptor types.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by USB.")]
	public enum DescriptorType
	{
		Device    = 0x01,
		Config    = 0x02,
		String    = 0x03,
		Interface = 0x04,
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Item names are given by USB.")]
		EndPoint  = 0x05,
		Hid       = 0x21,
		Report    = 0x22,
		Physical  = 0x23,
		Hub       = 0x29
	}

	/// <summary>
	/// Descriptor sizes per descriptor type.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by USB.")]
	public enum DescriptorTypeSize
	{
		Device        = 18,
		Config        = 9,
		Interface     = 9,
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Item names are given by USB.")]
		EndPoint      = 7,
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Item names are given by USB.")]
		EndPointAudio = 9, // Audio extension
		HubNonVar     = 7
	}

	/// <summary>
	/// String descriptor types.
	/// </summary>
	/// <remarks>
	/// Replication of private enum 'Win32.Hid.StringDescriptorIndex' for less coupling to Win32.Hid assembly.
	/// </remarks>
	public enum StringDescriptorIndex
	{
		LanguageIds  = 0,
		Manufacturer = 1,
		Product      = 2,
		Serial       = 3
	}

	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Item names are given by USB.")]
	[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Item names are given by USB.")]
	[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by USB.")]
	[Flags]
	public enum EndPointDirection
	{
		In  = 0x80,
		Out = 0x00
	}

	public enum TransferType
	{
		Control     = 0,
		Isochronous = 1,
		Bulk        = 2,
		Interrupt   = 3
	}

	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Item names are given by USB.")]
	[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "Type name is given by USB.")]
	[Flags]
	public enum RequestType
	{
		Standard = (0x00 << 5),
		Class    = (0x01 << 5),
		Vendor   = (0x02 << 5),
		[SuppressMessage("Microsoft.Naming", "CA1700:DoNotNameEnumValuesReserved", Justification = "Item name is given by USB.")]
		Reserved = (0x03 << 5)
	}

	public enum StandardRequest
	{
		GetStatus        = 0x00,
		ClearFeature     = 0x01,
		////               0x02 is reserved
		SetFeature       = 0x03,
		////               0x04 is reserved
		SetAddress       = 0x05,
		GetDescriptor    = 0x06,
		SetDescriptor    = 0x07,
		GetConfiguration = 0x08,
		SetConfiguration = 0x09,
		GetInterface     = 0x0A,
		SetInterface     = 0x0B,
		SynchFrame       = 0x0C
	}

	public enum RequestRecipient
	{
		Device    = 0x00,
		Interface = 0x01,
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoint", Justification = "Item names are given by USB.")]
		EndPoint  = 0x02,
		Other     = 0x03
	}

	public enum IsoSyncType
	{
		None     = 0,
		Async    = 1,
		Adaptive = 2,
		Sync     = 3
	}

	public enum IsoUsageType
	{
		Data     = 0,
		Feedback = 1,
		Implicit = 2
	}

	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values are given by USB.")]
	[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "Type name is given by USB.")]
	[Flags]
	public enum TransferFlags
	{
		ShortNotOk   = (1 << 0),
		FreeBuffer   = (1 << 1),
		FreeTransfer = (1 << 2)
	}

	public enum DeviceEvent
	{
		None,
		Connected,
		Disconnected
	}

	/// <summary>
	/// The USB usage page as described in http://www.usb.org/developers/devclass_docs/Hut1_12v2.pdf.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "These are NOT flags!")]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
	[CLSCompliant(false)]
	public enum HidUsagePage : ushort // 16 bits!
	{
		Undefined                    = 0x00,
		GenericDesktopControls       = 0x01,
		SimulationControls           = 0x02,
		VirtualRealityControls       = 0x03,
		SportControls                = 0x04,
		GameControls                 = 0x05,
		GenericDeviceControls        = 0x06,
		KeyboardKeypad               = 0x07,
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "LEDs", Justification = "Item names are given by USB.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "LEDs", Justification = "Item names are given by USB.")]
		LEDs                         = 0x08,
		Button                       = 0x09,
		Ordinal                      = 0x0A,
		Telephony                    = 0x0B,
		Consumer                     = 0x0C,
		Digitizer                    = 0x0D,
		////                           0x0E is reserved
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "PID", Justification = "Item names are given by USB.")]
		PID                          = 0x0F,
		Unicode                      = 0x10,
		////                           0x11..0x13 are reserved
		AlphanumericDisplay          = 0x14,
		////                           0x15..0x3F are reserved
		MedicalInstruments           = 0x40,
		////                           0x41..0x7F are reserved
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Monitor_1                    = 0x80,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Monitor_2                    = 0x81,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Monitor_3                    = 0x82,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Monitor_4                    = 0x83,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Power_1                      = 0x84,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Power_2                      = 0x85,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Power_3                      = 0x86,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Item names are given by USB.")]
		Power_4                      = 0x87,
		////                           0x88..0x8B are reserved
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "BarCode", Justification = "Item names are given by USB.")]
		BarCodeScanner               = 0x8C,
		Scale                        = 0x8D,
		MagneticStripeReadingDevices = 0x8E,
		PointOfSale                  = 0x8F,
		CameraControl                = 0x90,
		Arcade                       = 0x91,
		////                           0x92..0xFEFF are reserved
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Make obvious that item defines a range.")]
		VendorDefined_First        = 0xFF00,
		////                         0xFF00..0xFFFF are vendor-defined
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Make obvious that item defines a range.")]
		VendorDefined_Last         = 0xFFFF
	}

	#region HidUsagePage XEnum

	/// <summary>
	/// Extended enum HidUsagePageEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class HidUsagePageEx : EnumEx
	{
		/// <summary>Default is <see cref="HidUsagePage.Undefined"/>.</summary>
		[CLSCompliant(false)]
		public const HidUsagePage Default = HidUsagePage.Undefined;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public HidUsagePageEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public HidUsagePageEx(HidUsagePage page)
			: base(page)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static HidUsagePageEx Parse(string s)
		{
			HidUsagePageEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid HID usage page string! String must a HID usage page number, or one of the predefined encodings."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out HidUsagePageEx result)
		{
			HidUsagePage enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new HidUsagePageEx(enumResult);
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
		[CLSCompliant(false)]
		public static bool TryParse(string s, out HidUsagePage result)
		{
			int intResult;
			if (int.TryParse(s, out intResult)) // TryParse() trims whitespace.
			{
				result = (HidUsagePageEx)intResult;
				return (true);
			}
			else
			{
				result = new HidUsagePageEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		[CLSCompliant(false)]
		public static implicit operator HidUsagePage(HidUsagePageEx page)
		{
			return ((HidUsagePage)page.UnderlyingEnum);
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public static implicit operator HidUsagePageEx(HidUsagePage page)
		{
			return (new HidUsagePageEx(page));
		}

		/// <summary></summary>
		public static implicit operator int(HidUsagePageEx page)
		{
			return (page.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator HidUsagePageEx(int page)
		{
			return (new HidUsagePageEx((HidUsagePage)page));
		}

		/// <summary></summary>
		public static implicit operator string(HidUsagePageEx page)
		{
			return (page.ToString());
		}

		/// <summary></summary>
		public static implicit operator HidUsagePageEx(string page)
		{
			return (Parse(page));
		}

		#endregion
	}

	#endregion

	/// <summary>
	/// The USB HID usage as described in http://www.usb.org/developers/devclass_docs/Hut1_12v2.pdf.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "Underlying type is given by the Win32 API.")]
	[CLSCompliant(false)]
	public enum HidUsageId : ushort // 16 bits!
	{
		Undefined                    = 0x00,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Make obvious that item defines a range.")]
		PageDefined_First            = 0x01,
		////                           0x01..0xFEFF are defined per usage page
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Make obvious that item defines a range.")]
		PageDefined_Last           = 0xFEFF,
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Make obvious that item defines a range.")]
		VendorDefined_First        = 0xFF00,
		////                         0xFF00..0xFFFF are vendor-defined
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "Make obvious that item defines a range.")]
		VendorDefined_Last         = 0xFFFF
	}

	#region HidUsageId XEnum

	/// <summary>
	/// Extended enum HidUsageEx.
	/// </summary>
	/// <remarks>
	/// This <see cref="EnumEx"/> based type is not serializable because <see cref="Enum"/> isn't.
	/// Use the underlying enum for serialization, or alternatively, a string representation.
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class HidUsageIdEx : EnumEx
	{
		/// <summary>Default is <see cref="HidUsageId.Undefined"/>.</summary>
		[CLSCompliant(false)]
		public const HidUsageId Default = HidUsageId.Undefined;

		/// <summary>Default is <see cref="Default"/>.</summary>
		public HidUsageIdEx()
			: this(Default)
		{
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public HidUsageIdEx(HidUsageId usage)
			: base(usage)
		{
		}

		#region ToString
		//==========================================================================================
		// ToString
		//==========================================================================================

		/// <summary>
		/// Converts the value of this instance to its equivalent string representation.
		/// </summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region Parse
		//==========================================================================================
		// Parse
		//==========================================================================================

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static HidUsageIdEx Parse(string s)
		{
			HidUsageIdEx result;
			if (TryParse(s, out result)) // TryParse() trims whitespace.
				return (result);
			else
				throw (new FormatException(@"""" + s + @""" is an invalid HID usage ID string! String must a HID usage number, or one of the predefined encodings."));
		}

		/// <remarks>
		/// Following the convention of the .NET framework, whitespace is trimmed from <paramref name="s"/>.
		/// </remarks>
		public static bool TryParse(string s, out HidUsageIdEx result)
		{
			HidUsageId enumResult;
			if (TryParse(s, out enumResult)) // TryParse() trims whitespace.
			{
				result = new HidUsageIdEx(enumResult);
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
		[CLSCompliant(false)]
		public static bool TryParse(string s, out HidUsageId result)
		{
			int intResult;
			if (int.TryParse(s, out intResult)) // TryParse() trims whitespace.
			{
				result = (HidUsageIdEx)intResult;
				return (true);
			}
			else // Invalid string!
			{
				result = new HidUsageIdEx(); // Default!
				return (false);
			}
		}

		#endregion

		#region Conversion Operators
		//==========================================================================================
		// Conversion Operators
		//==========================================================================================

		/// <summary></summary>
		[CLSCompliant(false)]
		public static implicit operator HidUsageId(HidUsageIdEx usage)
		{
			return ((HidUsageId)usage.UnderlyingEnum);
		}

		/// <summary></summary>
		[CLSCompliant(false)]
		public static implicit operator HidUsageIdEx(HidUsageId usage)
		{
			return (new HidUsageIdEx(usage));
		}

		/// <summary></summary>
		public static implicit operator int(HidUsageIdEx usage)
		{
			return (usage.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator HidUsageIdEx(int usage)
		{
			return (new HidUsageIdEx((HidUsageId)usage));
		}

		/// <summary></summary>
		public static implicit operator string(HidUsageIdEx usage)
		{
			return (usage.ToString());
		}

		/// <summary></summary>
		public static implicit operator HidUsageIdEx(string usage)
		{
			return (Parse(usage));
		}

		#endregion
	}

	#endregion
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
