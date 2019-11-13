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
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

namespace YAT.Domain
{
	// Disable warning 1591 "Missing XML comment for publicly visible type or member" to avoid
	// warnings for each undocumented member below. Documenting each member makes little sense
	// since they pretty much tell their purpose and documentation tags between the members
	// makes the code less readable.
	#pragma warning disable 1591

	/// <summary></summary>
	public enum IODirection
	{
		/// <remarks>Also usable with meaning 'Unknown' (yet).</remarks>
		None = MKY.IO.Serial.Direction.None,

		/// <remarks>YAT uses term 'Tx' instead of 'Output'.</remarks>
		/// <remarks>YAT sorts 'Tx' before 'Rx'.</remarks>
		Tx = MKY.IO.Serial.Direction.Output,

		/// <remarks>YAT uses term 'Rx' instead of 'Input'.</remarks>
		/// <remarks>YAT sorts 'Rx' after 'Tx'.</remarks>
		Rx = MKY.IO.Serial.Direction.Input,

		/// <remarks>Usable for I/O operations not tied to a direction.</remarks>
		Bidir = 3 // Explicit value is needed because Output = 2 / Input = 1.
	}

	/// <summary></summary>
	public enum IOErrorSeverity
	{
		Acceptable = MKY.IO.Serial.ErrorSeverity.Acceptable,
		Severe     = MKY.IO.Serial.ErrorSeverity.Severe,
		Fatal      = MKY.IO.Serial.ErrorSeverity.Fatal
	}

	#pragma warning restore 1591
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
