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
// MKY Development Version 1.0.6
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace MKY.IO.Serial
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
		/// Sets the output into XOn state.
		/// </summary>
		void SetOutputXOn();

		/// <summary>
		/// Sets the output into XOff state.
		/// </summary>
		void SetOutputXOff();

		/// <summary>
		/// Toggles the output XOn/XOff state.
		/// </summary>
		void ToggleOutputXOnXOff();
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
