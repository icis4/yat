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
// YAT Version 2.2.0 Development
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

// This code is intentionally placed into the parser base namespace even though the file is located
// in a "\States" sub-directory. The sub-directory shall only group the implementation but not open
// another namespace. The state classes already contain "State" in their name.
namespace YAT.Domain.Parser
{
	/// <summary>
	/// This state handles a nested context, i.e. is something similar to a stack. The state
	/// terminates when the nested context has terminated.
	/// </summary>
	internal class NestedState : ParserState
	{
		/// <summary></summary>
		public override bool TryParse(Parser parser, int parseChar, ref FormatException formatException)
		{
			bool parseCharHasBeenParsed = parser.NestedParser.State.TryParse(parser.NestedParser, parseChar, ref formatException);

			if (parser.NestedParser.HasFinished) // Regain parser "focus".
			{
				parser.NestedParser.Close(); // Close to release references to parent.
				parser.NestedParser = null; // Property will dispose the nested parser.

				ChangeState(parser, new DefaultState());

				if (!parseCharHasBeenParsed) // Again try to parse the character with the 'new' parser:
					parseCharHasBeenParsed = parser.State.TryParse(parser, parseChar, ref formatException);
			}

			return (parseCharHasBeenParsed);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
