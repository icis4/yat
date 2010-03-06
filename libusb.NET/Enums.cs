//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2004 Mike Kr�ger.
// Copyright � 2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace libusb.NET
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
        //                    0x04
        Physical            = 0x05,
        Image               = 0x06,
        Printer             = 0x07,
        MassStorage         = 0x08,
        Hub                 = 0x09,
        CdcData             = 0x0A,
        SmartCard           = 0x0B,
        //                    0x0C
        ContentSecurity     = 0x0D,
        Video               = 0x0E,
        PersonalHealthcare  = 0x0F,

        DiagnosticDevice    = 0xDC,

        WirelessController  = 0xE0,
        Miscellaneous       = 0xEF,

        ApplicationSpecific = 0xFE,
        VendorSpecific      = 0xFF,
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
        //                 0x02 is reserved 
        SetFeature       = 0x03,
        //                 0x04 is reserved
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
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
//==================================================================================================
