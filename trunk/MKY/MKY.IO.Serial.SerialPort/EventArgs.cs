//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.14
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
		public override string PortStamp
		{
			get { return (this.portId.ToShortString()); }
		}
	}

	/// <summary></summary>
	public class SerialDataSentEventArgs : DataSentEventArgs
	{
		private SerialPortId portId;

		/// <summary></summary>
		public SerialDataSentEventArgs(byte data, SerialPortId portId)
			: this(new byte[] { data }, portId)
		{
		}

		/// <summary></summary>
		public SerialDataSentEventArgs(byte[] data, SerialPortId portId)
			: base(data)
		{
			this.portId = portId;
		}

		/// <summary></summary>
		public override string PortStamp
		{
			get { return (this.portId.ToShortString()); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
