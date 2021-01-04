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

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MKY.Windows.Forms
{
	/// <summary>
	/// Implementation of a workaround to fix a bug in Windows.Forms. The bug appears in MDI forms
	/// when using shortcuts in context menus. Such shortcuts are always passed to the first MDI
	/// child instead of the active child.
	/// To workaround this bug, all potential shortcuts shall be collected when creating the form.
	/// Then, the shortcuts can manually be handled in the <see cref="Form.ProcessCmdKey"/> method.
	/// </summary>
	/// <remarks>
	/// To improve performance, the items are collected in a dictionary, taking the shortcut keys
	/// as lookup key. Thus, this workaround object needs to be re-created in case shortcut keys
	/// are changed in a context menu.
	/// </remarks>
	public class ContextMenuStripShortcutTargetWorkaround
	{
		private Dictionary<Keys, ToolStripMenuItem> shortcutItems = new Dictionary<Keys, ToolStripMenuItem>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ContextMenuStripShortcutTargetWorkaround"/>
		/// class that is empty and has the default initial capacity.
		/// </summary>
		public ContextMenuStripShortcutTargetWorkaround()
		{
			this.shortcutItems = new Dictionary<Keys, ToolStripMenuItem>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContextMenuStripShortcutTargetWorkaround"/>
		/// class that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">
		/// The initial number of items that the underlying collection can contain.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="capacity"/>capacity is less than 0.
		/// </exception>
		public ContextMenuStripShortcutTargetWorkaround(int capacity)
		{
			this.shortcutItems = new Dictionary<Keys, ToolStripMenuItem>(capacity);
		}

		/// <summary>
		/// Add a tool strip to the workaround and let its shortcut items get collected.
		/// </summary>
		public void Add(ToolStrip strip)
		{
			foreach (ToolStripItem tsi in strip.Items)
			{
				var item = tsi as ToolStripMenuItem;
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
		/// Gets the number of items actually contained in this workaround object.
		/// </summary>
		public int Count
		{
			get { return (this.shortcutItems.Count); }
		}

		/// <summary>
		/// Evaluates the shortcuts and invokes the assigned event if a shortcut is
		/// assigned to the <paramref name="keyData"/>.
		/// </summary>
		/// <returns>
		/// <c>true</c> if a shortcut has been invoked; otherwise, <c>false</c>.
		/// </returns>
		public bool ProcessCmdKey(Keys keyData)
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
