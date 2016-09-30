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
// MKY Version 1.0.17
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2016 Matthias Kläy.
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
