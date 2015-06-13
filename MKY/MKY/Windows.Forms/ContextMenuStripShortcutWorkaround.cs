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
// MKY Development Version 1.0.14
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Implementation of a workaround to fix a bug in Windows.Forms. The bug appears in MDI forms
	/// when using shortcuts in context menus. Such shortcuts are always passed to first MDI child
	/// instead of the active child.
	/// To workaround this bug, all potential shortcuts shall be collected when creating the form.
	/// Then, the shortcuts can manually be handled in the <see cref="Form.ProcessCmdKey"/> method.
	/// </summary>
	/// <remarks>
	/// To improve performance, the items are compiled into a dictionary, taking the shortcut keys
	/// as lookup key. Thus, this workaround object needs to be re-created in case shortcut keys
	/// are changed in a context menu.
	/// </remarks>
	public class ContextMenuStripShortcutWorkaround
	{
		private Dictionary<Keys, ToolStripMenuItem> shortcutItems = new Dictionary<Keys, ToolStripMenuItem>();

		/// <summary>
		/// Add a tool strip to the workaround and let its shortcut items get collected.
		/// </summary>
		public void Add(ToolStrip strip)
		{
			foreach (ToolStripItem tsi in strip.Items)
			{
				ToolStripMenuItem item = tsi as ToolStripMenuItem;
				if ((item != null) && (item.ShortcutKeys != Keys.None))
					this.shortcutItems.Add(item.ShortcutKeys, item);
			}
		}

		/// <summary>
		/// Optionally, add an additional menu item.
		/// </summary>
		public void Add(ToolStripMenuItem item)
		{
			if (item.ShortcutKeys != Keys.None)
				this.shortcutItems.Add(item.ShortcutKeys, item);
		}

		/// <summary>
		/// Gets the number of elements actually contained in this workaround object.
		/// </summary>
		public int Count
		{
			get { return (this.shortcutItems.Count); }
		}

		/// <summary>
		/// Optionally, add an additional menu item.
		/// </summary>
		public bool ProcessShortcut(Keys keyData)
		{
			ToolStripMenuItem item;
			if (this.shortcutItems.TryGetValue(keyData, out item))
			{
				// Ensure that item is still set to this shortcut, and is enabled:
				if ((item.ShortcutKeys == keyData) && (item.Enabled))
				{
					item.PerformClick();
					return (true);
				}
			}

			return (false);
		}
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
