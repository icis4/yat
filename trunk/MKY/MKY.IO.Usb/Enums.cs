//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2012 Matthias Kläy.
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MKY.IO.Usb
{
	/// <summary>
	/// USB device classes as defined by the USB standard.
	/// See http://www.usb.org/developers/defined_class/ for detailed list.
	/// </summary>
	public enum DeviceClass
	{
		DeviceSpecific      = 0x00,

		Audio               = 0x01,
		CdcControl          = 0x02,
		Hid                 = 0x03,
		//// Reserved         0x04
		Physical            = 0x05,
		Image               = 0x06,
		Printer             = 0x07,
		MassStorage         = 0x08,
		Hub                 = 0x09,
		CdcData             = 0x0A,
		SmartCard           = 0x0B,
		//// Reserved         0x0C
		ContentSecurity     = 0x0D,
		Video               = 0x0E,
		PersonalHealthcare  = 0x0F,

		DiagnosticDevice    = 0xDC,

		WirelessController  = 0xE0,
		Miscellaneous       = 0xEF,

		ApplicationSpecific = 0xFE,
		VendorSpecific      = 0xFF,

		Any                 = 0x00,
	}
	
	/// <summary>
	/// Descriptor types.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by USB.")]
	public enum DescriptorType
	{
		Device    = 0x01,
		Config    = 0x02,
		String    = 0x03,
		Interface = 0x04,
		EndPoint  = 0x05,
		Hid       = 0x21,
		Report    = 0x22,
		Physical  = 0x23,
		Hub       = 0x29,
	}

	/// <summary>
	/// Descriptor sizes per descriptor type.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by USB.")]
	public enum DescriptorTypeSize
	{
		Device        = 18,
		Config        = 9,
		Interface     = 9,
		EndPoint      = 7,
		EndPointAudio = 9, // Audio extension
		HubNonVar     = 7,
	}

	/// <summary>
	/// String descriptor types.
	/// </summary>
	public enum StringDescriptorIndex
	{
		LanguageIds  = 0,
		Manufacturer = 1,
		Product      = 2,
		SerialNumber = 3,
	}

	public enum EndpointDirection
	{
		In  = 0x80,
		Out = 0x00,
	}

	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by USB.")]
	public enum TransferType
	{
		Control     = 0,
		Isochronous = 1,
		Bulk        = 2,
		Interrupt   = 3,
	}

	public enum RequestType
	{
		Standard = (0x00 << 5),
		Class    = (0x01 << 5),
		Vendor   = (0x02 << 5),
		Reserved = (0x03 << 5),
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
		SynchFrame       = 0x0C,
	}

	public enum RequestRecipient
	{
		Device    = 0x00,
		Interface = 0x01,
		Endpoint  = 0x02,
		Other     = 0x03,
	}

	public enum IsoSyncType
	{
		None     = 0,
		Async    = 1,
		Adaptive = 2,
		Sync     = 3,
	}

	public enum IsoUsageType
	{
		Data     = 0,
		Feedback = 1,
		Implicit = 2,
	}

	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values given by USB.")]
	public enum TransferFlags
	{
		ShortNotOk   = 1 << 0,
		FreeBuffer   = 1 << 1,
		FreeTransfer = 1 << 2,
	}

	public enum DeviceEvent
	{
		None,
		Connected,
		Disconnected,
	}

	/// <summary>
	/// The USB usage page as described in http://www.usb.org/developers/devclass_docs/Hut1_12.pdf.
	/// </summary>
	public enum HidUsagePage
	{
		Unknown                      =   -1,
		Undefined                    = 0x00,
		GenericDesktopControls       = 0x01,
		SimulationControls           = 0x02,
		VirtualRealityControls       = 0x03,
		SportControls                = 0x04,
		GameControls                 = 0x05,
		GenericDeviceControls        = 0x06,
		KeyboardKeypad               = 0x07,
		LEDs                         = 0x08,
		Button                       = 0x09,
		Ordinal                      = 0x0A,
		Telephony                    = 0x0B,
		Consumer                     = 0x0C,
		Digitizer                    = 0x0D,
		//// Reserved                  0x0E
		PID                          = 0x0F,
		Unicode                      = 0x10,
		//// Reserved                  0x11..0x13
		AlphanumericDisplay          = 0x14,
		//// Reserved                  0x15..0x3F
		MedicalInstruments           = 0x40,
		//// Reserved                  0x41..0x7F
		Monitor_1                    = 0x80,
		Monitor_2                    = 0x81,
		Monitor_3                    = 0x82,
		Monitor_4                    = 0x83,
		Power_1                      = 0x84,
		Power_2                      = 0x85,
		Power_3                      = 0x86,
		Power_4                      = 0x87,
		//// Reserved                  0x88..0x8B
		BarCodeScanner               = 0x8C,
		Scale                        = 0x8D,
		MagneticStripeReadingDevices = 0x8E,
		PointOfSale                  = 0x8F,
		CameraControl                = 0x90,
		Arcade                       = 0x91,
		//// Reserved                  0x92..0xFEFF
		//// Vendor-defined          0xFF00..0xFFFF
	}

	#region HidUsagePage XEnum

	/// <summary>
	/// Extended enum HidUsagePageEx.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class HidUsagePageEx : EnumEx
	{
		/// <summary>Default is <see cref="HidUsagePage.Unknown"/>.</summary>
		public HidUsagePageEx()
			: base(HidUsagePage.Unknown)
		{
		}

		/// <summary></summary>
		protected HidUsagePageEx(HidUsagePage page)
			: base(page)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(NumberFormatInfo.InvariantInfo));
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static HidUsagePageEx Parse(string page)
		{
			return ((HidUsagePageEx)int.Parse(page, NumberFormatInfo.InvariantInfo));
		}

		/// <summary></summary>
		public static bool TryParse(string page, out HidUsagePageEx result)
		{
			int intResult;

			if (int.TryParse(page, out intResult))
			{
				result = (HidUsagePageEx)intResult;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator HidUsagePage(HidUsagePageEx page)
		{
			return ((HidUsagePage)page.UnderlyingEnum);
		}

		/// <summary></summary>
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
	/// The USB HID usage as described in http://www.usb.org/developers/devclass_docs/Hut1_12.pdf.
	/// </summary>
	public enum HidUsage
	{
		Unknown     =   -1,
		Undefined   = 0x00,
		Pointer     = 0x01,
		Mouse       = 0x02,
		//// Reserved 0x03,
		Joystick    = 0x04,
		GamePad     = 0x05,
		Keyboard    = 0x06,
		Keypad      = 0x07,
		MultiAxis   = 0x08,
		TabletPC    = 0x09,
		//// See document above.
	}

	#region HidUsage XEnum

	/// <summary>
	/// Extended enum HidUsageEx.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extended enum and extends the underlying enum.")]
	public class HidUsageEx : EnumEx
	{
		/// <summary>Default is <see cref="HidUsage.Unknown"/>.</summary>
		public HidUsageEx()
			: base(HidUsage.Unknown)
		{
		}

		/// <summary></summary>
		protected HidUsageEx(HidUsage usage)
			: base(usage)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString(NumberFormatInfo.InvariantInfo));
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static HidUsageEx Parse(string usage)
		{
			return ((HidUsageEx)int.Parse(usage, NumberFormatInfo.InvariantInfo));
		}

		/// <summary></summary>
		public static bool TryParse(string usage, out HidUsageEx result)
		{
			int intResult;

			if (int.TryParse(usage, out intResult))
			{
				result = (HidUsageEx)intResult;
				return (true);
			}
			else
			{
				result = null;
				return (false);
			}
		}

		#endregion

		#region Conversion Operators

		/// <summary></summary>
		public static implicit operator HidUsage(HidUsageEx usage)
		{
			return ((HidUsage)usage.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator HidUsageEx(HidUsage usage)
		{
			return (new HidUsageEx(usage));
		}

		/// <summary></summary>
		public static implicit operator int(HidUsageEx usage)
		{
			return (usage.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator HidUsageEx(int usage)
		{
			return (new HidUsageEx((HidUsage)usage));
		}

		/// <summary></summary>
		public static implicit operator string(HidUsageEx usage)
		{
			return (usage.ToString());
		}

		/// <summary></summary>
		public static implicit operator HidUsageEx(string usage)
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
