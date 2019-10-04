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
// YAT Version 2.1.0
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

using System.Diagnostics.CodeAnalysis;

namespace YAT.Domain
{
	/// <summary></summary>
	public enum SendMode
	{
		/// <summary></summary>
		Text,

		/// <summary></summary>
		File
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

	/// <remarks>
	/// So far there can only be one attribute, thus named "Attribute" and not marked [Flags].
	/// </remarks>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Enum actually implements an attribute, an attribute related to display elements.")]
	public enum LineChunkAttribute
	{
		/// <summary></summary>
		None =  0,

		/// <summary>Resulting line shall be highlighted.</summary>
		Highlight,

		/// <summary>Filtering is active; resulting line shall be included.</summary>
		Filter,

		/// <summary>Filtering is active; resulting line may be excluded.</summary>
		SuppressIfNotFiltered,

		/// <summary>Suppressing is active; resulting line may be excluded.</summary>
		SuppressIfSubsequentlyTriggered,

		/// <summary>Suppressing is active; resulting line shall be excluded.</summary>
		Suppress
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
