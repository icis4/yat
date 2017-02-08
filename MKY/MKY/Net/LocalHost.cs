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
// MKY Development Version 1.0.18
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

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace MKY.Net
{
	/// <summary>
	/// LocalHost utility methods.
	/// </summary>
	public static class LocalHost
	{
		/// <summary>
		/// Gets an available TCP port chosen by the system or the system's TCP stack.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		public static int AvailableTcpPort
		{
			get
			{
				try
				{
					TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
					listener.Start();
					int port = ((IPEndPoint)listener.LocalEndpoint).Port;
					listener.Stop();
					return (port);
				}
				catch
				{
					return (0);
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
