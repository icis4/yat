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
// MKY Development Version 1.0.14
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace MKY.Drawing
{
	/// <summary>
	/// An extension to the <see cref="ColorTranslator"/> class.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class ColorTranslatorEx
	{
		/// <summary>
		/// Translates an HTML or Win32 color representation to a GDI+ <see cref="Color"/> structure.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Emphasizes the color's representation.")]
		public static Color FromHtmlOrWin32(string colorString)
		{
			try
			{
				return (ColorTranslator.FromHtml(colorString));
			}
			catch (Exception exHtml)
			{
				int colorValue;
				if (int.TryParse(colorString, out colorValue))
				{
					return (ColorTranslator.FromWin32(colorValue));
				}
				else
				{
					throw (new ArgumentException("Color string could not be converted into a color structure!", "colorString", exHtml));
				}
			}
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
