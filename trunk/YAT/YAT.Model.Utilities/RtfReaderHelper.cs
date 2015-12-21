﻿//==================================================================================================
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.IO;
using System.Windows.Forms;

#endregion

namespace YAT.Model.Utilities
{
	/// <summary>
	/// Static utility class providing RTF reader functionality for YAT.
	/// </summary>
	public static class RtfReaderHelper
	{
		/// <summary></summary>
		public static string[] LinesFromRtfFile(string rtfFilePath)
		{
			RichTextBox richTextProvider = new RichTextBox();
			using (FileStream fs = File.OpenRead(rtfFilePath))
			{
				richTextProvider.LoadFile(fs, RichTextBoxStreamType.RichText);
			}
			return (richTextProvider.Lines);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
