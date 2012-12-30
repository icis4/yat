//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
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
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
// warnings for each undocumented member below. Documenting each member makes little sense
// since they pretty much tell their purpose and documentation tags between the members
// makes the code less readable.
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;

namespace MKY.IO.Serial
{
	/// <summary></summary>
	public enum ErrorSeverity
	{
		Acceptable,
		Severe,
		Fatal,
	}

	/// <summary></summary>
	public enum Direction
	{
		Any,
		Input,
		Output,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
