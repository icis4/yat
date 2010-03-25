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

namespace MKY.IO.Usb
{
	/// <summary></summary>
	public class ErrorEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly string Message;

		/// <summary></summary>
		public ErrorEventArgs(string message)
		{
			Message = message;
		}
	}

	/// <summary></summary>
	public class DeviceEventArgs : EventArgs
	{
		/// <summary></summary>
		public readonly DeviceClass DeviceClass;

		/// <summary></summary>
		public readonly string DevicePath;

		/// <summary></summary>
		public DeviceEventArgs(DeviceClass deviceClass, string devicePath)
		{
			DeviceClass = deviceClass;
			DevicePath = devicePath;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
