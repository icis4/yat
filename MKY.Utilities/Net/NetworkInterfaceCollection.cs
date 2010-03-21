//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace MKY.Utilities.Net
{
	/// <summary>
	/// List containing network interfaces.
	/// </summary>
	[Serializable]
	public class NetworkInterfaceCollection : List<XNetworkInterface>
	{
		/// <summary></summary>
		public NetworkInterfaceCollection()
			: base()
		{
		}

		/// <summary></summary>
		public NetworkInterfaceCollection(IEnumerable<XNetworkInterface> rhs)
			: base(rhs)
		{
		}

		/// <summary>
		/// Fills list with all interfaces.
		/// </summary>
		public virtual void FillWithAvailableInterfaces()
		{
			Clear();

			// Add common interfaces such as <Any> to the collection
			AddRange(XNetworkInterface.GetItems());

			// Add interfaces of current machine to the collection
			NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
			foreach (IPAddress address in Dns.GetHostAddresses(""))
			{
				string description = "";
				foreach (NetworkInterface ni in nis)
				{
					if (ni.OperationalStatus == OperationalStatus.Up)
					{
						foreach (UnicastIPAddressInformation ai in ni.GetIPProperties().UnicastAddresses)
						{
							if (address.Equals(ai.Address))
							{
								description = ni.Description;
								break;
							}
						}

						if (description != "")
							break;
					}
				}
				Add(new XNetworkInterface(address, description));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
