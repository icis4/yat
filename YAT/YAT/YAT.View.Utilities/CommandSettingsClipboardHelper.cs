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
// YAT Version 2.4.1
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Windows.Forms;
using MKY.Xml;
using MKY.Xml.Serialization;

using YAT.Model.Types;
using YAT.Settings.Model;

#endregion

namespace YAT.View.Utilities
{
	/// <summary></summary>
	public static class CommandSettingsClipboardHelper
	{
		/// <summary></summary>
		public static bool ClipboardContainsText
		{
			get
			{
				try
				{
					return (Clipboard.ContainsText());
				}
				catch (ExternalException) // The clipboard could not be cleared. This typically
				{                         // occurs when it is being used by another process.
					return (false);
				}
			}
		}

		/// <remarks>Named 'Set' same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		public static bool TrySet(IWin32Window owner, Command c)
		{
			var root = new CommandSettingsRoot();
			root.Command = c;

			var sb = new StringBuilder();
			XmlSerializerEx.SerializeToString(typeof(CommandSettingsRoot), root, CultureInfo.CurrentCulture, ref sb);

			try
			{
				Clipboard.SetText(sb.ToString());
				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				var text = new StringBuilder();
				text.AppendLine("Failed to copy to clipboard!");
				text.AppendLine();
				text.Append    ("Make sure the clipboard is not blocked by another process.");

				MessageBoxEx.Show
				(
					owner,
					text.ToString(),
					"Clipboard Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <summary>Copies the command text or file path to the clipboard.</summary>
		/// <remarks>Named 'Set' same as e.g. <see cref="Clipboard.SetText(string)"/>.</remarks>
		public static bool TrySetTextOrFilePath(IWin32Window owner, Command c)
		{
			var sb = new StringBuilder();

			if (c.IsText)
			{
				if (c.IsSingleLineText)
				{
					sb.Append(c.SingleLineText);
				}
				else if (c.IsMultiLineText)
				{
					for (int i = 0; i < c.MultiLineText.Length; i++)
					{
						var isNotLastLine = (i != (c.MultiLineText.Length - 1));
						if (isNotLastLine)
							sb.AppendLine(c.MultiLineText[i]);
						else
							sb.Append(    c.MultiLineText[i]);
					}
				}
				else
				{
					throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known text command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, nameof(c)));
				}
			}
			else if (c.IsFilePath)
			{
				sb.Append(c.FilePath);
			}
			else
			{
				throw (new ArgumentException(MessageHelper.InvalidExecutionPreamble + "Command '" + c + "' does not specify a known command type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug, nameof(c)));
			}

			try
			{
				Clipboard.SetText(sb.ToString());
				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				var text = new StringBuilder();
				text.AppendLine("Failed to copy to clipboard!");
				text.AppendLine();
				text.Append    ("Make sure the clipboard is not blocked by another process.");

				MessageBoxEx.Show
				(
					owner,
					text.ToString(),
					"Clipboard Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				return (false);
			}
		}

		/// <remarks>Named 'Get' same as e.g. <see cref="Clipboard.GetText()"/>.</remarks>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		public static bool TryGet(IWin32Window owner, out Command c)
		{
			string s;

			try
			{
				s = Clipboard.GetText();
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				var text = new StringBuilder();
				text.AppendLine("Failed to paste from clipboard!");
				text.AppendLine();
				text.Append    ("Make sure the clipboard is not blocked by another process.");

				MessageBoxEx.Show
				(
					owner,
					text.ToString(),
					"Clipboard Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				c = null;
				return (false);
			}

			// First, try to deserialize from XML:
			object root = null;
			try
			{
				AlternateXmlElement[] alternateXmlElements = null; // CommandSettingsRoot does not (yet) have alternate elements.
				root = XmlSerializerEx.DeserializeFromStringInsisting(typeof(CommandSettingsRoot), alternateXmlElements, s);
			}
			catch (Exception ex)
			{
				DebugEx.WriteException(typeof(CommandPagesSettingsClipboardHelper), ex, "Deserialization from XML has failed, trying to make use of plain text.");
			}

			if (root != null)
			{
				var rootCasted = (CommandSettingsRoot)root;
				c = rootCasted.Command;
				return (true);
			}

			// Then, probe for multi-line text:
			if (s.Contains(Environment.NewLine))
			{
				string[] lines = s.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
				c = new Command("", lines);
				return (true);
			}

			// Then, probe for file path:
			if (PathEx.IsValid(s) && File.Exists(s)) // IsValid() is not sufficient, as "ABC" will successfully be resolved to
			{                                        // a path relative to the current working directory, e.g. "C:\MyWork\ABC".
				c = new Command("", true, s); // Simply use whole string, no matter whether absolute or relative path.
				return (true);
			}

			// Finally, use as single-line text:
			if (!string.IsNullOrEmpty(s))
			{
				c = new Command(s);
				return (true);
			}

			MessageBoxEx.Show
			(
				owner,
				"Clipboard does not contain valid " + ApplicationEx.CommonName + " command definition content.",
				"Clipboard Content Not Valid",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation
			);

			c = null;
			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
