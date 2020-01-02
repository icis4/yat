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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Usb
{
	/// <summary>
	/// Interface for USB devices.
	/// </summary>
	public interface IDevice
	{
		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary>
		/// Event raised after the device has been connected or reconnected.
		/// </summary>
		event EventHandler Connected;

		/// <summary>
		/// Event raised after the device has been disconnected.
		/// </summary>
		event EventHandler Disconnected;

		/// <summary>
		/// Event raised after an I/O error has occurred.
		/// </summary>
		event EventHandler<ErrorEventArgs> IOError;

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// Indicates whether the device is connected to the computer.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the device is connected to the computer; otherwise, <c>false</c>.
		/// </returns>
		bool IsConnected { get; }

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
