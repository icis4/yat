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
// MKY Development Version 1.0.13
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// This improved version of <see cref="MessageBox"/> implements an RTL (right-to-left)
	/// aware message box as suggested by FxCop rule CA1300 "Specify MessageBoxOptions" in
	/// the MSDN at http://msdn.microsoft.com/library/ms182191.aspx.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop doesn't seem to be able to skip URLs...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class MessageBoxEx
	{
		/// <summary></summary>
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return (Show(null, text, caption, buttons, icon));
		}

		/// <summary></summary>
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return (Show(owner, text, caption, buttons, icon, (MessageBoxDefaultButton)0));
		}

		/// <summary></summary>
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return (Show(owner, text, caption, buttons, icon, defaultButton, (MessageBoxOptions)0));
		}

		/// <summary></summary>
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		{
			if (IsRightToLeft(owner))
				options |= (MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

			return (MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options));
		}

		private static bool IsRightToLeft(IWin32Window owner)
		{
			Control control = owner as Control;
			if (control != null)
				return (control.RightToLeft == RightToLeft.Yes);

			// If no parent control is available, query the CurrentUICulture whether we are running
			// under right-to-left:
			return (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
