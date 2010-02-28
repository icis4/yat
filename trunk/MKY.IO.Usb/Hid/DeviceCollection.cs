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

namespace MKY.IO.Usb.Hid
{
	/// <summary>
	/// List containing USB Ser/HID device IDs.
	/// </summary>
	[Serializable]
	public class DeviceCollection : Usb.DeviceCollection
	{
        /// <summary></summary>
        public DeviceCollection()
            : base(Utilities.Win32.Hid.GetHidGuid())
        {
        }
    }
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
