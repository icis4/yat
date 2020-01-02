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
// Copyright © 2003-2020 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;

namespace YAT.Log
{
	/// <summary></summary>
	public enum LogType
	{
		/// <summary></summary>
		Control,

		/// <summary></summary>
		Raw,

		/// <summary></summary>
		Neat
	}

	/// <summary></summary>
	public enum LogDirection
	{
		/// <summary></summary>
		None,

		/// <summary></summary>
		Tx,

		/// <summary></summary>
		Bidir,

		/// <summary></summary>
		Rx
	}

	/// <summary></summary>
	public enum LogChannel
	{
		/// <summary></summary>
		Control = 0,

		/// <summary></summary>
		RawTx = 1,

		/// <summary></summary>
		RawBidir = 2,

		/// <summary></summary>
		RawRx = 3,

		/// <summary></summary>
		NeatTx = 4,

		/// <summary></summary>
		NeatBidir = 5,

		/// <summary></summary>
		NeatRx = 6
	}

	/// <summary></summary>
	public enum LogFileWriteMode
	{
		/// <summary></summary>
		Create,

		/// <summary></summary>
		Append
	}

	/// <summary></summary>
	public enum TextEncoding
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "UTF", Justification = "Same spelling as 'Encoding.UTF8'.")]
		UTF8,

		/// <summary></summary>
		Terminal
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
