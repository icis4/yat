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

using ALAZ.SystemEx.NetEx.SocketsEx;

namespace MKY.IO.Serial.Socket
{
	/// <summary></summary>
	public static class SocketDefaults
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
		/// use a default value of 8196.
		/// </remarks>
		public const int SocketBufferSize = 2048;

		/// <remarks>
		/// Value by default used by
		/// <see cref="BaseSocketConnectionHost"/>,
		/// <see cref="SocketClient"/> and
		/// <see cref="SocketServer"/>.
		///
		/// Note that
		/// <see cref="System.Net.Sockets.Socket.SendBufferSize"/> and
		/// <see cref="System.Net.Sockets.Socket.ReceiveBufferSize"/>
		/// use a default value of 8196.
		/// </remarks>
		public const int MessageBufferSize = 2048;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
