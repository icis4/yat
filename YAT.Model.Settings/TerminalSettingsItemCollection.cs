using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Model.Settings
{
	/// <summary>
	/// Collection for terminal settings items, provides methods to handle the items.
	/// </summary>
	public class TerminalSettingsItemCollection : List<TerminalSettingsItem>
	{
		/// <summary></summary>
		public TerminalSettingsItemCollection()
			: base()
		{
		}

		/// <summary></summary>
		public TerminalSettingsItemCollection(IEnumerable<TerminalSettingsItem> collection)
			: base(collection)
		{
		}

		/// <summary></summary>
		public TerminalSettingsItemCollection(int capacity)
			: base(capacity)
		{
		}

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Add or replaces the item that has the same GUID as tsi.
		/// </summary>
		public void AddOrReplaceGuid(TerminalSettingsItem tsi)
		{
			TerminalSettingsItemCollection clone = new TerminalSettingsItemCollection(this);

			for (int i = 0; i < clone.Count; i++)
			{
				if (this[i].Guid.Equals(tsi.Guid))
				{
					this[i] = tsi;
					return;
				}
			}

			// add if not contained yet
			Add(tsi);
		}

		/// <summary>
		/// Removes all items that have the specified GUID.
		/// </summary>
		public void RemoveGuid(Guid guid)
		{
			TerminalSettingsItemCollection obsoleteItems = new TerminalSettingsItemCollection();

			foreach (TerminalSettingsItem item in this)
			{
				if (item.Guid == guid)
					obsoleteItems.Add(item);
			}

			foreach (TerminalSettingsItem item in obsoleteItems)
			{
				Remove(item);
			}
		}

		#endregion
	}
}
