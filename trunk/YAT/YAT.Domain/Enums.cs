﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.0.1 Development
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
	/// <summary></summary>
	public enum IOErrorSeverity
	{
		/// <summary></summary>
		Acceptable = MKY.IO.Serial.ErrorSeverity.Acceptable,

		/// <summary></summary>
		Severe = MKY.IO.Serial.ErrorSeverity.Severe,

		/// <summary></summary>
		Fatal = MKY.IO.Serial.ErrorSeverity.Fatal
	}

	/// <summary></summary>
	public enum IODirection
	{
		/// <summary></summary>
		None = MKY.IO.Serial.Direction.None,

		/// <remarks>YAT uses term 'Tx' instead of 'Output'.</remarks>
		/// <remarks>YAT sorts 'Tx' before 'Rx'.</remarks>
		Tx = MKY.IO.Serial.Direction.Output,

		/// <remarks>YAT uses term 'Rx' instead of 'Input'.</remarks>
		/// <remarks>YAT sorts 'Rx' after 'Tx'.</remarks>
		Rx = MKY.IO.Serial.Direction.Input,

		/// <summary></summary>
		Bidir = 3
	}

	/// <remarks>
	/// So far there can only be one attribute, thus named "Attribute" and not marked [Flags].
	/// </remarks>
	public enum LineChunkAttribute
	{
		/// <summary></summary>
		None =  0,

		/// <summary>Resulting line shall be highlighted.</summary>
		Highlight,

		/// <summary>Resulting line shall be included when filtering is active.</summary>
		Filter,

		/// <summary>Resulting line shall may be excluded when filtering is active.</summary>
		PotentiallySuppress,

		/// <summary>Resulting line shall be excluded when suppressing is active.</summary>
		SuppressForSure
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
