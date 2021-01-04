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
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "'Ex' emphasizes that it's an extension to an existing class and not a replacement as '2' would emphasize.")]
	public static class MessageBoxEx
	{
		/// <summary></summary>
		public static DialogResult Show(string text, string caption)
		{
			return (Show(text, caption));
		}

		/// <summary></summary>
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
		{
			return (Show(text, caption, buttons));
		}

		/// <summary></summary>
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
		{
			return (Show(null, text, caption, buttons, icon));
		}

		/// <summary></summary>
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
		{
			return (Show(null, text, caption, buttons, icon, defaultButton));
		}

		/// <summary></summary>
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
		{
			return (Show(null, text, caption, buttons, icon, defaultButton, options));
		}

		/// <summary></summary>
		public static DialogResult Show(IWin32Window owner, string text, string caption)
		{
			return (Show(owner, text, caption, (MessageBoxButtons)0));
		}

		/// <summary></summary>
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
		{
			return (Show(owner, text, caption, buttons, (MessageBoxIcon)0));
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
			var control = (owner as Control);
			if (control != null)
				return (control.RightToLeft == RightToLeft.Yes);

			// If no parent control is available, query the 'CurrentUICulture' whether we are
			// running under right-to-left:
			return (CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
