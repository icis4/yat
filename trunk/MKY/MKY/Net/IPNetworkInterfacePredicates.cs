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
// Copyright © 2010-2020 Matthias Kläy.
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
		/// <summary></summary>
		public IPNetworkInterfaceEx NetworkInterface { get; set; }

		/// <summary></summary>
		public EqualsDescription(IPNetworkInterfaceEx networkInterface)
		{
			NetworkInterface = networkInterface;
		}

		/// <summary></summary>
		public Predicate<IPNetworkInterfaceEx> Match
		{
			get { return (IsMatch); }
		}

		private bool IsMatch(IPNetworkInterfaceEx other)
		{
			return (NetworkInterface.EqualsDescription(other));
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
