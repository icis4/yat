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

using System.Net.Sockets;

namespace MKY.Net.Sockets
{
	/// <summary>Hard coded default values</summary>
	public static class SocketEx
	{
		/// <remarks>
		/// The hard-coded default value of <see cref="Socket.SendBufferSize"/>.
		/// </remarks>
		/// <remarks>
		/// Value cannot be retrieved from <see cref="Socket"/>.
		/// Value is hard-coded, there is no need to create an object and retrieve the value.
		/// </remarks>
		public const int SendBufferSizeDefault = 8192;

		/// <remarks>
		/// The hard-coded default value of <see cref="Socket.ReceiveBufferSize"/>.
		/// </remarks>
		public const int ReceiveBufferSizeDefault = 8192;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
