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

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using MKY;

namespace YAT.Model.Types
{
	/// <summary>
	/// A <see cref="MatchEvaluator"/> that "knows" the trigger captures.
	/// </summary>
	public class AutoResponseReplaceEvaluator
	{
		private string[] triggerCaptureValues;

		/// <summary>
		/// Initializes a new instance of the <see cref="AutoResponseReplaceEvaluator"/> class.
		/// </summary>
		public AutoResponseReplaceEvaluator(string[] triggerCaptureValues)
		{
			this.triggerCaptureValues = triggerCaptureValues;
		}

		/// <summary>
		/// Evaluates the specified match.
		/// </summary>
		public string Evaluate(Match match)
		{
			string tagNumberString = match.Groups[1].Value;
			int tagNumber;
			if (int.TryParse(tagNumberString, out tagNumber))
			{
				if (tagNumber >= 1)
				{
					int tagIndex = (tagNumber - 1);
					if (this.triggerCaptureValues.Length > tagIndex)
						return (this.triggerCaptureValues[tagIndex]);

					var sb = new StringBuilder();
					sb.Append("[Automatic response replacement error: Invalid replacement number "); // Similar format as 'DisplayElement.ErrorInfo'.
					sb.Append(tagNumberString);
					sb.Append(" ($");
					sb.Append(tagNumberString);
					sb.Append(") was requested but trigger only contains ");
					sb.Append(this.triggerCaptureValues.Length);
					sb.Append((this.triggerCaptureValues.Length == 1) ? " capture!]" : " captures!]");
					return (sb.ToString());
				}
				else
				{
					var sb = new StringBuilder();
					sb.Append("[Automatic response replacement error: Invalid replacement number "); // Similar format as 'DisplayElement.ErrorInfo'.
					sb.Append(tagNumberString);
					sb.Append(" ($");
					sb.Append(tagNumberString);
					sb.Append(") was requested! Number must be must be a positive integral value (1, 2,...).]");
					return (sb.ToString());
				}
			}
			else
			{
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + tagNumberString + "' is an invalid replacement number! This indicates an erroneous regular expression pattern!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
