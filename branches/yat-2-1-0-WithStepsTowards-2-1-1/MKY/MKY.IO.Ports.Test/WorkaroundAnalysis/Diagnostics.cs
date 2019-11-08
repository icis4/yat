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
// MKY Version 1.0.28 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2007-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace MKY.IO.Ports.Test.WorkaroundAnalysis
{
	public static class Diagnostics
	{
		public static void WriteErrorDetailsToConsole(Exception ex, bool includeStackTrace)
		{
			WriteErrorDetailsToConsole(ex, null, includeStackTrace);
		}

		public static void WriteErrorDetailsToConsole(Exception ex, string leadMessage = null, bool includeStackTrace = true)
		{
			foreach (var line in ComposeErrorDetailLines(ex, leadMessage, includeStackTrace))
				Console.WriteLine(line);
		}
		public static IEnumerable<string> ComposeErrorDetailLines(Exception ex, string leadMessage = null, bool includeStackTrace = true)
		{
			const string IndentationGap = "    ";

			var exception = ex;
			var exceptionLevel = 0;
			var indentation = "";

			if (!string.IsNullOrEmpty(leadMessage))
			{
				var leadMessageLines = leadMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
				if (leadMessageLines.Length <= 1) // Message is single-line:
				{
					yield return (leadMessage); // No indentation for lead message.
				}
				else // Message is multi-line:
				{
					foreach (var line in leadMessageLines)
						yield return (line); // No indentation for lead message.
				}
			}

			while (exception != null)
			{
				if (exceptionLevel > 0)
					indentation += IndentationGap;

				if (string.IsNullOrEmpty(exception.Message)) // Message is missing !?!
				{
					yield return (string.Format("{0}{1}", indentation, exception.GetType().FullName));
				}
				else
				{
					var exceptionMessageLines = exception.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
					if (exceptionMessageLines.Length <= 1) // Message is single-line:
					{
						yield return (string.Format("{0}{1}: {2}", indentation, exception.GetType().FullName, exception.Message));
					}
					else // Message is multi-line:
					{
						yield return (string.Format("{0}{1}:", indentation, exception.GetType().FullName));

						foreach (var line in exceptionMessageLines) // Additional indentation:
							yield return (string.Format("{0}{1}{2}", indentation, IndentationGap, line));
					}
				}

				if (includeStackTrace)
				{
					var stackTraceLines = ComposeStackTraceLines(exception);
					if (stackTraceLines.Count() > 0)
					{
						yield return (string.Format("{0}{1}", indentation, "Stack:"));

						// Stack trace is already indented. No need to fully indent again.
						// However, stack trace intendation is 3 instead of 4 => add space.

						foreach (var line in stackTraceLines)
							yield return (string.Format("{0} {1}", indentation, line));
					}
				}

				exception = exception.InnerException;
				exceptionLevel++;
			}
		}

		public static IEnumerable<string> ComposeStackTraceLines(Exception ex)
		{
			foreach (var line in ex.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
				yield return (line);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
