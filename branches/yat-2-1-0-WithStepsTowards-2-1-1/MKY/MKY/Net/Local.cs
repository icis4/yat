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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;

using MKY.Diagnostics;

namespace MKY.Net
{
	/// <summary>
	/// Local utility methods.
	/// </summary>
	public static class Local
	{
		/// <summary>
		/// Finds an available TCP port chosen by the system or the system's TCP stack.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryFindAvailableTcpPort(out int result)
		{
			const int Attempts = 7; // Should be sufficient...

			Exception exInner = null;

			for (int i = 1; i <= Attempts; i++)
			{
				try
				{
					var listener = new TcpListener(IPAddress.Loopback, 0);
					listener.Start();
					result = ((IPEndPoint)listener.LocalEndpoint).Port;
					listener.Stop();
					return (true);
				}
				catch (Exception ex)
				{
					var sb = new StringBuilder();

					sb.Append(i);
					sb.Append(Int32Ex.ToEnglishSuffix(i));
					sb.Append(" attempt to retrieve an available TCP port has failed");

					if (i < Attempts)
						sb.Append(", trying again...");

					DebugEx.WriteException(typeof(Local), ex, sb.ToString());

					exInner = ex;
				}
			}

			// Local scope for dedicated 'StringBuilder':
			{
				var sb = new StringBuilder();

				sb.Append(Attempts);
				sb.Append(" attempt to retrieve an available TCP port have failed");

				DebugEx.WriteException(typeof(Local), exInner, sb.ToString());

				result = 0;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
