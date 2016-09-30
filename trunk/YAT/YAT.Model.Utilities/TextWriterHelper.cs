﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2'' Version 1.99.52
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
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

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing text writer functionality for YAT.
	/// </summary>
	public static class TextWriterHelper
	{
		/// <remarks>
		/// Pragmatic implementation of saving text to a file.
		/// </remarks>
		public static int LinesToFile(List<DisplayLine> lines, string filePath, Settings.FormatSettings formatSettings)
		{
			RichTextBox richTextProvider = RtfWriterHelper.LinesToRichTextBox(lines, formatSettings);
			richTextProvider.SaveFile(filePath, RichTextBoxStreamType.UnicodePlainText);

			return (lines.Count); // Assume success, an exception should otherwise be thrown above.
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
