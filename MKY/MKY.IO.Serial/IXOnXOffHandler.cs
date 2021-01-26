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
// MKY Version 1.0.29
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace MKY.IO.Serial
{
	/// <summary></summary>
	public interface IXOnXOffHandler
	{
		/// <summary>
		/// Returns <c>true</c> is XOn/XOff is in use, i.e. if one or the other kind of XOn/XOff
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
		void SignalInputXOn();

		/// <summary>
		/// Signals the other communication endpoint that this device is in XOff state.
		/// </summary>
		void SignalInputXOff();

		/// <summary>
		/// Toggles the input XOn/XOff state.
		/// </summary>
		void ToggleInputXOnXOff();

		/// <summary>
		/// Returns the number of sent XOn bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		int SentXOnCount { get; }

		/// <summary>
		/// Returns the number of sent XOff bytes, i.e. the count of input XOn/XOff signaling.
		/// </summary>
		int SentXOffCount { get; }

		/// <summary>
		/// Returns the number of received XOn bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		int ReceivedXOnCount { get; }

		/// <summary>
		/// Returns the number of received XOff bytes, i.e. the count of output XOn/XOff signaling.
		/// </summary>
		int ReceivedXOffCount { get; }

		/// <summary>
		/// Resets the XOn/XOff signaling counts.
		/// </summary>
		void ResetXOnXOffCount();
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
