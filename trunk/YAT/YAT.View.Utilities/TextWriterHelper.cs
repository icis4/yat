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
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Collections.Generic;
using System.Windows.Forms;

using YAT.Domain;
using YAT.Format.Settings;

#endregion

namespace YAT.View.Utilities
{
	/// <summary>
	/// Static utility class providing text writer functionality for YAT.
	/// </summary>
	public static class TextWriterHelper
	{
		/// <remarks>
		/// Pragmatic implementation of saving text to a file.
		/// </remarks>
		public static int LinesToFile(List<DisplayLine> lines, string filePath, FormatSettings formatSettings)
		{
			var richTextProvider = RtfWriterHelper.LinesToRichTextBox(lines, formatSettings);
			richTextProvider.SaveFile(filePath, RichTextBoxStreamType.UnicodePlainText);

			return (lines.Count); // Assume success, an exception should otherwise be thrown above.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
