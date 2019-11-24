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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using YAT.Model.Types;

#endregion

namespace YAT.Model
{
	/// <remarks>
	/// This partial class implements the automatic common part of <see cref="Terminal"/>.
	/// </remarks>
	public partial class Terminal
	{
		// Note that Auto[Action|Response] is intentionally implemented in 'Model'
		// instead of 'Domain' since Triggers/Patterns/Actions/Responses relate to 'Model' items
		// SendText/SendFile/PredefinedCommand/....
		//
		// If needed one day, trigger evaluation could be moved to 'Domain', same as EOL evaluation.
		// Moving this to 'Domain' would e.g. allow for coloring. However, this would require two or
		// even four more colors (Tx/Rx data/control highlight). This becomes too complicated...

		/// <summary>
		/// Tries to parse the given command into the corresponding byte sequence, taking the current settings into account.
		/// </summary>
		private bool TryParseCommandToSequence(Command c, out byte[] sequence)
		{
			if ((c != null) && (this.terminal != null))
			{
				if (c.IsSingleLineText)
				{
					byte[] lineResult;
					if (this.terminal.TryParseText(c.SingleLineText, out lineResult, c.DefaultRadix))
					{
						sequence = lineResult;
						return (true);
					}
				}
				else if (c.IsMultiLineText)
				{
					List<byte> commandResult = new List<byte>(256); // Preset the initial capacity to improve memory management; 256 is an arbitrary value.

					foreach (string line in c.MultiLineText)
					{
						byte[] lineResult;
						if (this.terminal.TryParseText(line, out lineResult, c.DefaultRadix))
							commandResult.AddRange(lineResult);
					}

					if (commandResult.Count > 0)
					{
						sequence = commandResult.ToArray();
						return (true);
					}
				}
			}

			sequence = null;
			return (false);
		}

		/// <summary>
		/// Tries to parse the given command into the corresponding byte sequence, taking the current settings into account.
		/// </summary>
		private bool TryValidateCommandForTriggerText(Command c, out string triggerText)
		{
			if ((c != null) && (this.terminal != null))
			{
				if (c.IsSingleLineText)
				{
					triggerText = c.SingleLineText;
					return (true);
				}
				else if (c.IsMultiLineText)
				{
					// Not (yet) supported.
				}
			}

			triggerText = null;
			return (false);
		}

		/// <summary>
		/// Tries to parse the given command into a <see cref="Regex"/>.
		/// </summary>
		private bool TryCreateTriggerRegexFromCommand(Command c, out string triggerPattern, out Regex triggerRegex)
		{
			if ((c != null) && (this.terminal != null))
			{
				if (c.IsSingleLineText)
				{
					try
					{
						triggerPattern = c.SingleLineText;
						triggerRegex = new Regex(triggerPattern);
						return (true);
					}
					catch (ArgumentException ex)
					{
						triggerPattern = null;
						triggerRegex = null;
						return (false);
					}
				}
				else if (c.IsMultiLineText)
				{
					// Not (yet) supported.
				}
			}

			triggerPattern = null;
			triggerRegex = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
