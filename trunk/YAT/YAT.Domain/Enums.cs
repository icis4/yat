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

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public enum IORequest
	{
		/// <summary></summary>
		Start = MKY.IO.Serial.IORequest.Open,

		/// <summary></summary>
		Stop = MKY.IO.Serial.IORequest.Close,
	}

	/// <summary></summary>
	public enum IOErrorSeverity
	{
		/// <summary></summary>
		Acceptable = MKY.IO.Serial.IOErrorSeverity.Acceptable,

		/// <summary></summary>
		Severe = MKY.IO.Serial.IOErrorSeverity.Severe,

		/// <summary></summary>
		Fatal = MKY.IO.Serial.IOErrorSeverity.Fatal,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
