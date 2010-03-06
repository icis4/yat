//==================================================================================================
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
// $Author: maettu_this $
// $Date: 2010-02-23 00:18:29 +0100 (Di, 23 Feb 2010) $
// $Revision: 254 $
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2004 Mike Krüger.
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

/// <summary>
/// This namespace declares the native libusb elements.
/// Elements aren't documented for consistency/maintenance reasons. See libusb for documentation.
/// </summary>
namespace libusb.NET.Native
{
    public enum libusb_transfer_status
    {
        LIBUSB_TRANSFER_COMPLETED,
        LIBUSB_TRANSFER_ERROR,
        LIBUSB_TRANSFER_TIMED_OUT,
        LIBUSB_TRANSFER_CANCELLED,
        LIBUSB_TRANSFER_STALL,
        LIBUSB_TRANSFER_NO_DEVICE,
        LIBUSB_TRANSFER_OVERFLOW,
    }

    public enum libusb_error
    {
        LIBUSB_SUCCESS             =   0,
        LIBUSB_ERROR_IO            =  -1,
        LIBUSB_ERROR_INVALID_PARAM =  -2,
        LIBUSB_ERROR_ACCESS        =  -3,
        LIBUSB_ERROR_NO_DEVICE     =  -4,
        LIBUSB_ERROR_NOT_FOUND     =  -5,
        LIBUSB_ERROR_BUSY          =  -6,
        LIBUSB_ERROR_TIMEOUT       =  -7,
        LIBUSB_ERROR_OVERFLOW      =  -8,
        LIBUSB_ERROR_PIPE          =  -9,
        LIBUSB_ERROR_INTERRUPTED   = -10,
        LIBUSB_ERROR_NO_MEM        = -11,
        LIBUSB_ERROR_NOT_SUPPORTED = -12,
        LIBUSB_ERROR_OTHER         = -99,
    }
}

//==================================================================================================
// End of
// $URL: https://y-a-terminal.svn.sourceforge.net/svnroot/y-a-terminal/trunk/MKY.IO.Ports/Properties/AssemblyInfo.cs $
//==================================================================================================
