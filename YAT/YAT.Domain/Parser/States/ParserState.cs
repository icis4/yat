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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;

using MKY;
using MKY.Diagnostics;

#endregion

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
	/// <summary></summary>
	internal abstract class ParserState : DisposableBase
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Required for recursion.")]
		public abstract bool TryParse(Parser parser, int parseChar, ref FormatException formatException);

		/// <summary></summary>
		protected static void ChangeState(Parser parser, ParserState state)
		{
			parser.State = state;
		}

		#region Disposal
		//--------------------------------------------------------------------------------------
		// Disposal
		//--------------------------------------------------------------------------------------

		/// <param name="disposing">
		/// <c>true</c> when called from <see cref="Dispose"/>,
		/// <c>false</c> when called from finalizer.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			// Dispose of managed resources:
			if (disposing)
			{
				// Nothing to do in state base (yet).
			}
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
