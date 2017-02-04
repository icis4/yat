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
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	[Flags]
	public enum Modes
	{
		/// <summary></summary>
		Radix = 1,

		/// <summary></summary>
		Ascii = 2,

		/// <summary></summary>
		AllByteArrayResults = Radix | Ascii,

		/// <summary></summary>
		AllExceptKeywords = AllByteArrayResults,

		/// <summary></summary>
		Keywords = 128,

		/// <summary></summary>
		All = AllByteArrayResults | Keywords,

		/// <summary></summary>
		Default = All,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
