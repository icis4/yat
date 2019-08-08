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
// YAT Version 2.0.1 Development
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

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using MKY.IO;
using MKY.Xml;
using MKY.Xml.Serialization;

using YAT.Model.Settings;
using YAT.Model.Types;
using YAT.Settings.Model;

#endregion

namespace YAT.View.Utilities
{
	/// <summary></summary>
	public static class CommandClipboardHelper
	{
		/// <summary></summary>
		public static bool TextIsAvailable
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

		/// <summary></summary>
		public static bool TrySet(Command c)
		{
			var root = new CommandSettingsRoot();
			root.Command = c;

			var sb = new StringBuilder();
			XmlSerializerEx.SerializeToString(typeof(CommandSettingsRoot), root, ref sb);

			try
			{
				Clipboard.SetText(sb.ToString());
				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				return (false);
			}
		}

		/// <summary></summary>
		public static bool TrySet(PredefinedCommandSettings commandPages, int selectedPageId)
		{
			var root = new CommandPageSettingsRoot();
			root.Page = commandPages.Pages[selectedPageId - 1];

			var sb = new StringBuilder();
			XmlSerializerEx.SerializeToString(typeof(CommandSettingsRoot), root, ref sb);

			try
			{
				Clipboard.SetText(sb.ToString());
				return (true);
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				return (false);
			}
		}

		/// <summary></summary>
		public static bool TryGet(out Command c)
		{
			string s;

			try
			{
				s = Clipboard.GetText();
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				c = null;
				return (false);
			}

			// First, try to deserialize from XML:
			AlternateXmlElement[] alternateXmlElements = null; // CommandSettingsRoot does not (yet) have alternate elements.
			object root;
			if (XmlSerializerEx.TryDeserializeFromStringInsisting(typeof(CommandSettingsRoot), alternateXmlElements, s, out root))
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
			if (PathEx.IsValid(s))
			{
				c = new Command("", true, s); // Simply use whole string, no matter whether absolute or relative path.
				return (true);
			}

			// Finally, use as single-line text:
			if (!string.IsNullOrEmpty(s))
			{
				c = new Command(s);
				return (true);
			}

			c = null;
			return (false);
		}

		/// <summary></summary>
		public static bool TryInsert(IWin32Window owner, PredefinedCommandSettings commandPagesOld, int selectedPageId, out PredefinedCommandSettings commandPagesNew)
		{

		}

		/// <summary></summary>
		public static bool TryAdd(IWin32Window owner, PredefinedCommandSettings commandPagesOld, out PredefinedCommandSettings commandPagesNew)
		{

		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
