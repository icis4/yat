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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2019 Matthias Kläy.
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
		None,
		Input,
		Output,
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
