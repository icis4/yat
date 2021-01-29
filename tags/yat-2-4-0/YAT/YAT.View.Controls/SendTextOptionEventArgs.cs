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
// YAT Version 2.4.0
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

using System;

namespace YAT.View.Controls
{
	/// <remarks>
	/// \remind (2017-07-23 / MKY)
	/// <see cref="MKY.EventArgs{T}"/> could be used instead of this class. However, the VS2015
	/// designer cannot cope with generic event args! Findings:
	///  > Designer cannot display the 'SendCommandRequest' of 'SendText' as well as 'Send'.
	///  > It crashes again and again! But is this indeed the root cause?
	///
	/// \todo
	/// Check again with VS2017+.
	/// </remarks>
	public class SendTextOptionEventArgs : EventArgs
	{
		/// <summary></summary>
		public SendTextOption Value { get; }

		/// <summary></summary>
		public SendTextOptionEventArgs(SendTextOption value)
		{
			Value = value;
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
