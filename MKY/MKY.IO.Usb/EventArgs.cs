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
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Usb
{
	/// <summary></summary>
	public class DeviceEventArgs : EventArgs
	{
		private DeviceClass deviceClass;
		private DeviceInfo deviceInfo;

		/// <summary></summary>
		public DeviceEventArgs(DeviceClass deviceClass, DeviceInfo deviceInfo)
		{
			this.deviceClass = deviceClass;
			this.deviceInfo  = deviceInfo;
		}

		/// <summary></summary>
		public DeviceClass DeviceClass
		{
			get { return (this.deviceClass); }
		}

		/// <summary></summary>
		public DeviceInfo DeviceInfo
		{
			get { return (this.deviceInfo); }
		}
	}

	/// <summary></summary>
	public class ErrorEventArgs : EventArgs
	{
		private string message;

		/// <summary></summary>
		public ErrorEventArgs(string message)
		{
			this.message = message;
		}

		/// <summary></summary>
		public string Message
		{
			get { return (this.message); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
