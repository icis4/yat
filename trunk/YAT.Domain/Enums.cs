//==================================================================================================
// URL       : $URL$
// Author    : $Author$
// Date      : $Date$
// Revision  : $Rev$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Domain
{
	/// <summary></summary>
	public enum IORequest
	{
		/// <summary></summary>
		StartIO = MKY.IO.Serial.IORequest.Open,
		/// <summary></summary>
		StopIO = MKY.IO.Serial.IORequest.Close,
	}
}

//==================================================================================================
// End of $URL$
//==================================================================================================
