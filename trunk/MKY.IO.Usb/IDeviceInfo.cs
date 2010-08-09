//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
// ------------------------------------------------------------------------------------------------
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:maettu_this@users.sourceforge.net.
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
		string SerialNumber { get; }

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
