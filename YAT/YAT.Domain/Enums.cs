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
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2012 Matthias Kläy.
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
	public enum IOErrorSeverity
	{
		/// <summary></summary>
		Acceptable = MKY.IO.Serial.ErrorSeverity.Acceptable,

		/// <summary></summary>
		Severe = MKY.IO.Serial.ErrorSeverity.Severe,

		/// <summary></summary>
		Fatal = MKY.IO.Serial.ErrorSeverity.Fatal,
	}

	/// <summary></summary>
	public enum IODirection
	{
		/// <summary></summary>
		Any = MKY.IO.Serial.Direction.Any,

		/// <summary></summary>
		Input = MKY.IO.Serial.Direction.Input,

		/// <summary></summary>
		Output = MKY.IO.Serial.Direction.Output,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
