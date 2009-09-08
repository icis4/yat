//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Ports
{
	/// <summary>
	/// Serial port control pins.
	/// </summary>
	public struct SerialPortControlPins
	{
		/// <summary>
		/// Request To Send.
		/// </summary>
		public bool Rts;

		/// <summary>
		/// Clear To Send.
		/// </summary>
		public bool Cts;

		/// <summary>
		/// Data Terminal Ready.
		/// </summary>
		public bool Dtr;

		/// <summary>
		/// Data Set Ready.
		/// </summary>
		public bool Dsr;

		/// <summary>
		/// Carrier Detect.
		/// </summary>
		public bool Cd;
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
