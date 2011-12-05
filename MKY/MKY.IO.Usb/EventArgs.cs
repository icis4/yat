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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
		public readonly DeviceInfo DeviceInfo;

		/// <summary></summary>
		public DeviceEventArgs(DeviceClass deviceClass, DeviceInfo deviceInfo)
		{
			DeviceClass = deviceClass;
			DeviceInfo = deviceInfo;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
