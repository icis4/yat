﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
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
