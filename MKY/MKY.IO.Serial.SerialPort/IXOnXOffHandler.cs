//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Development Version 1.0.8
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Serial.SerialPort
{
	/// <summary></summary>
	public interface IXOnXOffHandler
	{
		/// <summary>
		/// Returens <c>true</c> is XOn/XOff is in use, i.e. if one or the other kind of XOn/XOff
		/// flow control is active.
		/// </summary>
		bool XOnXOffIsInUse { get; }

		/// <summary>
		/// Gets the input XOn/XOff state.
		/// </summary>
		bool InputIsXOn { get; }

		/// <summary>
		/// Gets the output XOn/XOff state.
		/// </summary>
		bool OutputIsXOn { get; }

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOn state.
		/// </summary>
		void SetInputXOn();

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		void SetInputXOff();

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		void ToggleInputXOnXOff();
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
