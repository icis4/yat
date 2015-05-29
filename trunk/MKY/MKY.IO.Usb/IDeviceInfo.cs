//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.11
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace MKY.IO.Usb
{
	/// <summary>
	/// Information that every USB device provides.
	/// </summary>
	public interface IDeviceInfo
	{
		/// <summary>
		/// Returns the complete device info. To read a specific device property, use the property
		/// members below.
		/// </summary>
		DeviceInfo Info { get; }

		/// <summary></summary>
		int VendorId { get; }

		/// <summary></summary>
		string VendorIdString { get; }

		/// <summary></summary>
		int ProductId { get; }

		/// <summary></summary>
		string ProductIdString { get; }

		/// <summary></summary>
		string Manufacturer { get; }

		/// <summary></summary>
		string Product { get; }

		/// <summary></summary>
		string Serial { get; }

		/// <summary></summary>
		string ToString();

		/// <summary></summary>
		string ToShortString();
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
