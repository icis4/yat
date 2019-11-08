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
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
		public virtual void FillWithAvailableLocalInterfaces(bool prependPredefined = true)
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

							if ((ai.Address.AddressFamily == AddressFamily.InterNetwork) ||
							    (ai.Address.AddressFamily == AddressFamily.InterNetworkV6))
							{
								foreach (IPAddress hostAddress in hostEntry.AddressList)
								{                  // IPAddress does not override the ==/!= operators, thanks Microsoft guys...
									if (ai.Address.Equals(hostAddress))
										Add(new IPNetworkInterfaceEx(ai.Address, ni.Description));

									// Note that ni.Name is of not much use, example:
									//  > Description = "Microsoft Wi-Fi Direct Virtual Adapter"
									//  > Name        = "LAN-Verbindung 1"
								}
							} // if (is IP)

							DebugVerboseUnindent();
						} // foreach (unicast address)
					} // if (is up)

					DebugVerboseUnindent();
				} // foreach (interface)

				DebugVerboseUnindent("...done");
			}
		}

		/// <summary>
		/// Determines whether an element is in the collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// <c>true</c> if item is found in the collection; otherwise, <c>false</c>.
		/// </returns>
		public virtual bool ContainsDescription(IPNetworkInterfaceEx item)
		{
			lock (this)
			{
				foreach (IPNetworkInterfaceEx ni in this)
				{
					if (ni.EqualsDescription(item))
						return (true);
				}

				return (false);
			}
		}

		/// <summary>
		/// Searches for an element that matches the <paramref name="item"/>, and returns the
		/// first occurrence within the entire collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// The first element that matches the <paramref name="item"/>, if found; otherwise, –1.
		/// </returns>
		public virtual IPNetworkInterfaceEx FindDescription(IPNetworkInterfaceEx item)
		{
			lock (this)
			{
				var predicate = new EqualsDescription(item);
				return (Find(predicate.Match));
			}
		}

		/// <summary>
		/// Searches for an element that matches the <paramref name="item"/>, and returns the
		/// zero-based index of the first occurrence within the collection.
		/// </summary>
		/// <param name="item">
		/// The object to locate in the collection. The value can be null for reference types.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of an element that matches the
		/// <paramref name="item"/>, if found; otherwise, –1.
		/// </returns>
		public virtual int FindIndexDescription(IPNetworkInterfaceEx item)
		{
			lock (this)
			{
				var predicate = new EqualsDescription(item);
				return (FindIndex(predicate.Match));
			}
		}

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseIndent(string message = null)
		{
			if (!string.IsNullOrEmpty(message))
				Debug.WriteLine(message);

			Debug.Indent();
		}

		[Conditional("DEBUG_VERBOSE")]
		private static void DebugVerboseUnindent(string message = null)
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
