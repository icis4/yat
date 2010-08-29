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
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
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

using MKY.Utilities.Types;

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
	/// Extended enum XHidUsagePage.
	/// </summary>
	public class XHidUsagePage : XEnum
	{
		/// <summary>Default is <see cref="HidUsagePage.Unknown"/>.</summary>
		public XHidUsagePage()
			: base(HidUsagePage.Unknown)
		{
		}

		/// <summary></summary>
		protected XHidUsagePage(HidUsagePage page)
			: base(page)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XHidUsagePage Parse(string page)
		{
			return ((XHidUsagePage)int.Parse(page));
		}

		/// <summary></summary>
		public static bool TryParse(string page, out XHidUsagePage result)
		{
			int intResult;

			if (int.TryParse(page, out intResult))
			{
				result = (XHidUsagePage)intResult;
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
		public static implicit operator HidUsagePage(XHidUsagePage page)
		{
			return ((HidUsagePage)page.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XHidUsagePage(HidUsagePage page)
		{
			return (new XHidUsagePage(page));
		}

		/// <summary></summary>
		public static implicit operator int(XHidUsagePage page)
		{
			return (page.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XHidUsagePage(int page)
		{
			return (new XHidUsagePage((HidUsagePage)page));
		}

		/// <summary></summary>
		public static implicit operator string(XHidUsagePage page)
		{
			return (page.ToString());
		}

		/// <summary></summary>
		public static implicit operator XHidUsagePage(string page)
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
	/// Extended enum XHidUsage.
	/// </summary>
	public class XHidUsage : XEnum
	{
		/// <summary>Default is <see cref="HidUsage.Unknown"/>.</summary>
		public XHidUsage()
			: base(HidUsage.Unknown)
		{
		}

		/// <summary></summary>
		protected XHidUsage(HidUsage usage)
			: base(usage)
		{
		}

		#region ToString

		/// <summary></summary>
		public override string ToString()
		{
			return (UnderlyingEnum.GetHashCode().ToString());
		}

		#endregion

		#region Parse

		/// <summary></summary>
		public static XHidUsage Parse(string usage)
		{
			return ((XHidUsage)int.Parse(usage));
		}

		/// <summary></summary>
		public static bool TryParse(string usage, out XHidUsage result)
		{
			int intResult;

			if (int.TryParse(usage, out intResult))
			{
				result = (XHidUsage)intResult;
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
		public static implicit operator HidUsage(XHidUsage usage)
		{
			return ((HidUsage)usage.UnderlyingEnum);
		}

		/// <summary></summary>
		public static implicit operator XHidUsage(HidUsage usage)
		{
			return (new XHidUsage(usage));
		}

		/// <summary></summary>
		public static implicit operator int(XHidUsage usage)
		{
			return (usage.GetHashCode());
		}

		/// <summary></summary>
		public static implicit operator XHidUsage(int usage)
		{
			return (new XHidUsage((HidUsage)usage));
		}

		/// <summary></summary>
		public static implicit operator string(XHidUsage usage)
		{
			return (usage.ToString());
		}

		/// <summary></summary>
		public static implicit operator XHidUsage(string usage)
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
