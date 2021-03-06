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
// MKY Version 1.0.30
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

using MKY.IO.Ports;

namespace MKY.IO.Serial.SerialPort
{
	/// <summary></summary>
	public class SerialDataReceivedEventArgs : DataReceivedEventArgs
	{
		private SerialPortId portId;

		/// <summary></summary>
		public SerialDataReceivedEventArgs(byte data, SerialPortId portId)
			: this(new byte[] { data }, portId)
		{
		}

		/// <summary></summary>
		public SerialDataReceivedEventArgs(byte[] data, SerialPortId portId)
			: base(data)
		{
			this.portId = portId;
		}

		/// <summary></summary>
		public override string Device
		{
			get { return (this.portId.Name); }
		}
	}

	/// <summary></summary>
	public class SerialDataSentEventArgs : DataSentEventArgs
	{
		private SerialPortId portId;

		/// <summary></summary>
		public SerialDataSentEventArgs(byte data, DateTime timeStamp, SerialPortId portId)
			: this(new byte[] { data }, timeStamp, portId)
		{
		}

		/// <summary></summary>
		public SerialDataSentEventArgs(byte[] data, DateTime timeStamp, SerialPortId portId)
			: base(data, timeStamp)
		{
			this.portId = portId;
		}

		/// <summary></summary>
		public override string Device
		{
			get { return (this.portId.Name); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
