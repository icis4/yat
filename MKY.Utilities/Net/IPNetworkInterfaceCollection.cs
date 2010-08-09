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
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace MKY.Utilities.Net
{
	/// <summary>
	/// List containing IP network interfaces.
	/// </summary>
	[Serializable]
	public class IPNetworkInterfaceCollection : List<IPNetworkInterface>
	{
		/// <summary></summary>
		public IPNetworkInterfaceCollection()
			: base()
		{
		}

		/// <summary></summary>
		public IPNetworkInterfaceCollection(IEnumerable<IPNetworkInterface> rhs)
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
			AddRange(IPNetworkInterface.GetItems());

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

						if (description.Length > 0)
							break;
					}
				}
				Add(new IPNetworkInterface(address, description));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
