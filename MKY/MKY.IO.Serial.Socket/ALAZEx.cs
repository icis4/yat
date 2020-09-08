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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ALAZ", Justification = "ALAZ is a name.")]
	public static class ALAZEx
	{
		/// <remarks>
		/// Value by default used by
		/// <see cref="BaseSocketConnectionHost"/>,
		/// <see cref="SocketClient"/> and
		/// <see cref="SocketServer"/>.
		///
		/// Note that
		/// <see cref="System.Net.Sockets.Socket.SendBufferSize"/> and
		/// <see cref="System.Net.Sockets.Socket.ReceiveBufferSize"/>
		/// use a default value of 8192.
		/// </remarks>
		public const int SocketBufferSizeDefault = 2048;

		/// <remarks>
		/// Value by default used by
		/// <see cref="BaseSocketConnectionHost"/>,
		/// <see cref="SocketClient"/> and
		/// <see cref="SocketServer"/>.
		///
		/// Note that
		/// <see cref="System.Net.Sockets.Socket.SendBufferSize"/> and
		/// <see cref="System.Net.Sockets.Socket.ReceiveBufferSize"/>
		/// use a default value of 8192.
		/// </remarks>
		public const int MessageBufferSizeDefault = 2048;

		// Measurements 2020-08-13 using two interconnected YAT TCP/IP AutoSocket terminals with
		// default settings, transmitting 'Huge.txt' (~1 MiB):
		//
		// [Debug]
		//  > Small buffers and safe maximum payload lengths result in ~19.5 kiB/s.
		//  > Large buffers and large maximum payload lengths result in ~22.5 kiB/s.
		//    (32768 bytes)                    (30000 bytes)
		//
		// [Release]
		//  > Small buffers and safe maximum payload lengths result in ~43.6 kiB/s.
		//  > Large buffers and large maximum payload lengths result in ~51.9 kiB/s.
		//    (32768 bytes)                    (30000 bytes)
		//
		// Decided to use the safe sizes, i.e. value robustness over maximum throughput. If this
		// ever becomes an issue, the payload (and buffer) sizes could be made configurable.
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
