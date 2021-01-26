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
// YAT Version 2.4.0
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;

#endregion

namespace YAT.Domain.Utilities
{
	/// <summary>
	/// Static utility class providing RTF reader functionality for YAT.
	/// </summary>
	public static class RtfReaderHelper
	{
		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public static int LinesFromRtfFile(string filePath, out string[] lines)
		{
			var richTextProvider = new RichTextBox();
			using (var fs = File.OpenRead(filePath))
			{
				richTextProvider.LoadFile(fs, RichTextBoxStreamType.RichText);
			}
			lines = richTextProvider.Lines;
			return (lines.Length);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
