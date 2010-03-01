//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Usb
{
    /// <summary>
    /// USB device classes as defined by the USB standard.
    /// See http://www.usb.org/developers/defined_class/ for detailed list.
    /// </summary>
    public enum DeviceClass
    {
        /// <summary></summary>
        Any = -1,
        /// <summary></summary>
        DeviceSpecific = 0x00,
        /// <summary></summary>
        Audio = 0x01,
        /// <summary></summary>
        CdcControl = 0x02,
        /// <summary></summary>
        Hid = 0x03,
        /// <summary></summary>
        Physical = 0x05,
        /// <summary></summary>
        Image = 0x06,
        /// <summary></summary>
        Printer = 0x07,
        /// <summary></summary>
        MassStorage = 0x08,
        /// <summary></summary>
        Hub = 0x09,
        /// <summary></summary>
        CdcData = 0x0A,
        /// <summary></summary>
        SmartCard = 0x0B,
        /// <summary></summary>
        ContentSecurity = 0x0D,
        /// <summary></summary>
        Video = 0x0E,
        /// <summary></summary>
        PersonalHealthcare = 0x0F,
        /// <summary></summary>
        DiagnosticDevice = 0xDC,
        /// <summary></summary>
        WirelessController = 0xE0,
        /// <summary></summary>
        Miscellaneous = 0xEF,
        /// <summary></summary>
        ApplicationSpecific = 0xFE,
        /// <summary></summary>
        VendorSpecific = 0xFF,
    }
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
