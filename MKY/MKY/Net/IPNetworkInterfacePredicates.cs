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
// Copyright © 2010-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.Net
{
	/// <summary></summary>
	public class EqualsDescription
	{
		private IPNetworkInterfaceEx networkInterface;

		/// <summary></summary>
		public EqualsDescription(IPNetworkInterfaceEx networkInterface)
		{
			this.networkInterface = networkInterface;
		}

		/// <summary></summary>
		public IPNetworkInterfaceEx NetworkInterface
		{
			get { return (this.networkInterface); }
			set { this.networkInterface = value;  }
		}

		/// <summary></summary>
		public Predicate<IPNetworkInterfaceEx> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(IPNetworkInterfaceEx other)
		{
			return (this.networkInterface.EqualsDescription(other));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
