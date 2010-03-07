//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
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
    // typedef void (*libusb_pollfd_added_cb)(int fd, short events, void *user_data);
    public delegate void libusb_pollfd_added_cb(int fd, short events, object user_param);

    // typedef void (*libusb_pollfd_removed_cb)(int fd, void *user_data);
    public delegate void libusb_pollfd_removed_cb(int fd, object user_param);

    // typedef void (*libusb_transfer_cb_fn)(struct libusb_transfer *transfer);
    public delegate void libusb_transfer_cb_fn(libusb_transfer transfer);
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
