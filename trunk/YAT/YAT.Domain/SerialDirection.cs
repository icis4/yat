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

// The YAT.Domain namespace contains all raw/neutral/binary/text terminal infrastructure. This code
// is intentionally placed into the YAT.Domain namespace even though the file is located in the
// YAT.Domain\RawTerminal for better separation of the implementation files.
namespace YAT.Domain
{
	/// <summary></summary>
	public enum SerialDirection
	{
		/// <summary></summary>
		None,

		/// <summary></summary>
		Tx,

		/// <summary></summary>
		Rx,
	}

	/// <summary></summary>
	public static class SerialDirectionEx
	{
		/// <summary></summary>
		public static string ToString(SerialDirection direction)
		{
			switch (direction)
			{
				case SerialDirection.Tx:
					return "<<"; // Same as C++ stream out operator.

				case SerialDirection.Rx:
					return ">>"; // Same as C++ stream in operator.

				case SerialDirection.None:
				default:
					return "--";
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
