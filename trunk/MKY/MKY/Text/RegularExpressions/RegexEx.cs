﻿//==================================================================================================
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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2019 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace MKY.Text.RegularExpressions
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class RegexEx
	{
		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern)
		{
			string errorMessage;
			return (TryValidatePattern(pattern, out errorMessage));
		}

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern, out string errorMessage)
		{
			try
			{
				var regex = new Regex(pattern);
				UnusedLocal.PreventAnalysisWarning(regex);

				errorMessage = null;
				return (true);
			}
			catch (ArgumentException ex)
			{
				errorMessage = ex.Message;
				return (false);
			}
		}

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern, RegexOptions options)
		{
			string errorMessage;
			return (TryValidatePattern(pattern, options, out errorMessage));
		}

		/// <summary>
		/// Validates the given regular expression pattern.
		/// </summary>
		public static bool TryValidatePattern(string pattern, RegexOptions options, out string errorMessage)
		{
			try
			{
				var regex = new Regex(pattern, options);
				UnusedLocal.PreventAnalysisWarning(regex);

				errorMessage = null;
				return (true);
			}
			catch (ArgumentException ex)
			{
				errorMessage = ex.Message;
				return (false);
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
