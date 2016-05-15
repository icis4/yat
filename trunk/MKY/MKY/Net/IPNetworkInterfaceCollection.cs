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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable verbose output:
////#define DEBUG_VERBOSE

#endif // DEBUG

#endregion

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

#endregion

namespace MKY.Net
{
	/// <summary>
	/// List containing IP network interfaces.
	/// </summary>
	[Serializable]
	public class IPNetworkInterfaceCollection : List<IPNetworkInterfaceEx>
	{
		/// <summary></summary>
		public IPNetworkInterfaceCollection()
			: base()
		{
		}

		/// <summary></summary>
		public IPNetworkInterfaceCollection(IEnumerable<IPNetworkInterfaceEx> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all interfaces.
		/// </summary>
		public virtual void FillWithAvailableInterfaces(bool prependPredefined = true)
		{
			lock (this)
			{
				Clear();

				// Add predefined interfaces:

				if (prependPredefined)
					AddRange(IPNetworkInterfaceEx.GetItems());

				// Add interfaces of current machine:

				string hostName = Dns.GetHostName();
				IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
				DebugVerboseIndent("Retrieving interfaces of local machine '" + hostName + "'...");

				foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
				{
					DebugVerboseIndent("'" + ni.Description + "' / '" + ni.Name + "' is '" + ni.OperationalStatus + "'");

					if (ni.OperationalStatus == OperationalStatus.Up)
					{
						foreach (UnicastIPAddressInformation ai in ni.GetIPProperties().UnicastAddresses)
						{
							DebugVerboseIndent("Address is '" + ai.Address + "'");

							foreach (IPAddress hostAddress in hostEntry.AddressList)
							{
								if (ai.Address.Equals(hostAddress)) // IPAddress does not override the == operator!
									Add(new IPNetworkInterfaceEx(ai.Address, ni.Description));

								// Note that ni.Name is of not much use, example:
								//  > Description = "Microsoft Wi-Fi Direct Virtual Adapter"
								//  > Name        = "LAN -Verbindung* 1"
							}

							DebugVerboseUnindent();
						} // foreach (unicast address)
					} // if (is up)

					DebugVerboseUnindent();
				} // foreach (interface)

				DebugVerboseUnindent("...done");
			}
		}

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_VERBOSE")]
		private void DebugVerboseIndent(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);

			Debug.Indent();
		}

		[Conditional("DEBUG_VERBOSE")]
		private void DebugVerboseUnindent(string message = null)
		{
			Debug.Unindent();

			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
