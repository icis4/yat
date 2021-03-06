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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

namespace YAT.Domain.Parser
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "'NoEscapes' actually means 'None' but is more obvious.")]
	[SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames", Justification = "I know, actually used a plural name for a while, but usage simply isn't natural at many locations, thus reverted to sigular name.")]
	[Flags]
	public enum Mode
	{
		/// <summary></summary>
		None = NoEscapes,

		/// <summary></summary>
		NoEscapes = 0,

		/// <summary></summary>
		RadixEscapes = 1,

		/// <summary></summary>
		AsciiEscapes = 2,

		/// <summary></summary>
		RadixAndAsciiEscapes = (RadixEscapes | AsciiEscapes),

		/// <summary></summary>
		AllEscapesExceptKeywords = RadixAndAsciiEscapes,

		/// <summary></summary>
		KeywordEscapes = 128,

		/// <summary></summary>
		AllEscapes = (RadixAndAsciiEscapes | KeywordEscapes),

		/// <summary></summary>
		Default = AllEscapes
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
