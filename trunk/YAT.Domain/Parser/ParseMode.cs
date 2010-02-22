//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	[Flags]
	public enum ParseMode
	{
		/// <summary></summary>
		Radix = 1,
		/// <summary></summary>
		Ascii = 2,
		/// <summary></summary>
		AllByteArrayResults = 3,

		/// <summary></summary>
		Keywords = 128,

		/// <summary></summary>
		All = 131,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
