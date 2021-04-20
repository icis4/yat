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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
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
		/// <summary></summary>
		private const int FindAvailableTcpPortAttempts = 7; // Should be sufficient, see "TcpListener" remarks below.

		/// <summary></summary>
		private static string FindAvailableTcpPortAttemptsErrorMessage
		{
			get
			{
				var sb = new StringBuilder();

				sb.Append(FindAvailableTcpPortAttempts);
				sb.Append(" attempts to retrieve an available TCP port have failed");

				return (sb.ToString());
			}
		}

		/// <summary>
		/// Finds an available TCP port chosen by the system or the system's TCP stack.
		/// </summary>
		public static int FindAvailableTcpPort()
		{
			int result;

			if (TryFindAvailableTcpPort(out result))
				return (result);
			else
				throw (new IOException(FindAvailableTcpPortAttemptsErrorMessage));
		}

		/// <summary>
		/// Finds an available TCP port chosen by the system or the system's TCP stack.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all potential exceptions are handled.")]
		public static bool TryFindAvailableTcpPort(out int result)
		{
			Exception exLast = null;

			for (int i = 1; i <= FindAvailableTcpPortAttempts; i++)
			{
				try
				{                                                          // "...you can specify 0 for the port number.
					var listener = new TcpListener(IPAddress.Loopback, 0); // "In this case, the service provider will assign
					listener.Start();                                      //  an available port number between 1024 and 5000."
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

					if (i < FindAvailableTcpPortAttempts)
						sb.Append(", trying again...");

					DebugEx.WriteException(typeof(Local), ex, sb.ToString());

					exLast = ex;
				}
			}

			DebugEx.WriteException(typeof(Local), exLast, FindAvailableTcpPortAttemptsErrorMessage);

			result = 0;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
