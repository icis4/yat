//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
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
		Rx = MKY.IO.Serial.Direction.Input
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
