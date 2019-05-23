﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Collections.ObjectModel;

using MKY.IO.Usb;

namespace MKY.IO.Serial.Usb
{
	/// <summary></summary>
	public class SerialDataReceivedEventArgs : DataReceivedEventArgs
	{
		private DeviceInfo deviceInfo;

		/// <summary></summary>
		public SerialDataReceivedEventArgs(byte data, DeviceInfo deviceInfo)
			: this(new byte[] { data }, deviceInfo)
		{
		}

		/// <summary></summary>
		public SerialDataReceivedEventArgs(byte[] data, DeviceInfo deviceInfo)
			: base(data)
		{
			this.deviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public override string PortStamp
		{
			get { return (this.deviceInfo.ToShortString()); }
		}
	}

	/// <summary></summary>
	public class SerialDataSentEventArgs : DataSentEventArgs
	{
		private DeviceInfo deviceInfo;

		/// <summary></summary>
		public SerialDataSentEventArgs(byte data, DeviceInfo deviceInfo)
			: this(new byte[] { data }, deviceInfo)
		{
		}

		/// <summary></summary>
		public SerialDataSentEventArgs(byte[] data, DeviceInfo deviceInfo)
			: base(data)
		{
			this.deviceInfo = deviceInfo;
		}

		/// <summary></summary>
		public override string PortStamp
		{
			get { return (this.deviceInfo.ToShortString()); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================