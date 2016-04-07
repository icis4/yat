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
using System.Net;

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	public class SocketDataReceivedEventArgs : DataReceivedEventArgs
	{
		IPEndPoint remoteEndPoint;

		/// <summary></summary>
		public SocketDataReceivedEventArgs(byte data, IPEndPoint remoteEndPoint)
			: this(new byte[] { data }, remoteEndPoint)
		{
		}

		/// <summary></summary>
		public SocketDataReceivedEventArgs(byte[] data, IPEndPoint remoteEndPoint)
			: this (new ReadOnlyCollection<byte>(data), remoteEndPoint)
		{
		}

		/// <summary></summary>
		public SocketDataReceivedEventArgs(ReadOnlyCollection<byte> data, IPEndPoint remoteEndPoint)
			: base(data)
		{
			this.remoteEndPoint = remoteEndPoint;
		}

		/// <summary></summary>
		public override string PortStamp
		{
			get { return (this.remoteEndPoint.ToString()); }
		}
	}

	/// <summary></summary>
	public class SocketDataSentEventArgs : DataSentEventArgs
	{
		IPEndPoint remoteEndPoint;

		/// <summary></summary>
		public SocketDataSentEventArgs(byte data, IPEndPoint remoteEndPoint)
			: this(new byte[] { data }, remoteEndPoint)
		{
		}

		/// <summary></summary>
		public SocketDataSentEventArgs(byte[] data, IPEndPoint remoteEndPoint)
			: this (new ReadOnlyCollection<byte>(data), remoteEndPoint)
		{
		}

		/// <summary></summary>
		public SocketDataSentEventArgs(ReadOnlyCollection<byte> data, IPEndPoint remoteEndPoint)
			: base(data)
		{
			this.remoteEndPoint = remoteEndPoint;
		}

		/// <summary></summary>
		public override string PortStamp
		{
			get { return (this.remoteEndPoint.ToString()); }
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
