//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// MKY Version 1.0.9
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2013 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

namespace MKY.Settings
{
	/// <summary></summary>
	public enum SettingsType
	{
		/// <summary>
		/// Explicit (normal) user settings, user gets notified as soon as setting changes.
		/// E.g. communication settings or command definitions.
		/// </summary>
		Explicit,

		/// <summary>
		/// Implicit (hidden) user settings, user doesn't get notified when setting changes.
		/// E.g. window or layout settings that are automatically saved.
		/// </summary>
		Implicit,
	}

	/// <remarks>
	/// Attention, there currently are two similar implementations of this class:
	///  > <see cref="Settings.SettingsItem"/>
	///  > <see cref="Data.DataItem"/>
	/// The YAT feature request #3392253 "Consider replacing 'Settings' by 'DataItem'" deals with
	/// this issue, it will therefore not be forgotten. Until this feature request is implemented,
	/// changes to this class also have to be applied to <see cref="Data.DataItem"/>.
	/// </remarks>
	[Serializable]
	public abstract class SettingsItem : IEquatable<SettingsItem>
	{
		private SettingsType settingsType;

		private List<SettingsItem> nodes;
		private bool haveChanged;
		private int changeEventSuspendedCount;

		/// <summary></summary>
		public event EventHandler<SettingsEventArgs> Changed;

		/// <summary></summary>
		protected SettingsItem()
			: this(SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		protected SettingsItem(SettingsItem settings)
			: this(settings.settingsType)
		{
			// Do not copy nodes.
		}

		/// <summary></summary>
		protected SettingsItem(SettingsType type)
		{
			this.settingsType = type;
			this.nodes = new List<SettingsItem>();
		}

		#region Setup/Teardown Properties and Methods
		//==========================================================================================
		// Setup/Teardown Properties and Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual SettingsType SettingsType
		{
			get { return (this.settingsType); }
		}

		/// <summary></summary>
		protected virtual void AttachNode(SettingsItem node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.SuspendChangeEvent();
				node.SetChanged();
				node.Changed += new EventHandler<SettingsEventArgs>(node_Changed);
				this.nodes.Add(node);

				ResumeChangeEvent();
			}
		}

		/// <summary></summary>
		protected virtual void ReplaceNode(SettingsItem nodeOld, SettingsItem nodeNew)
		{
			if ((nodeOld != null) && (nodeNew != null))
			{
				if (this.nodes.Contains(nodeOld))
				{
					SuspendChangeEvent();

					nodeOld.Changed -= new EventHandler<SettingsEventArgs>(node_Changed);
					int index = this.nodes.IndexOf(nodeOld);
					this.nodes.RemoveAt(index);

					nodeNew.SuspendChangeEvent();
					nodeNew.SetChanged();
					nodeNew.Changed += new EventHandler<SettingsEventArgs>(node_Changed);
					this.nodes.Insert(index, nodeNew);

					ResumeChangeEvent();
				}
			}
		}

		/// <summary></summary>
		protected virtual void DetachNode(SettingsItem node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.Changed -= new EventHandler<SettingsEventArgs>(node_Changed);
				this.nodes.Remove(node);

				ResumeChangeEvent();
			}
		}

		#endregion

		#region Changed Properties and Methods
		//==========================================================================================
		// Changed Properties and Methods
		//==========================================================================================

		/// <summary>
		/// This flag indicates that the data item has changed. Either the data of the item itself
		/// or any of the sub-items. The flag can be used to e.g. display an asterisk * indicating
		/// a change of data, settings,...
		/// </summary>
		/// <remarks>
		/// To clear this flag, <see cref="ClearChanged"/> must be called. The flag is never cleared
		/// automatically.
		/// </remarks>
		public virtual bool HaveChanged
		{
			get
			{
				bool hc = this.haveChanged;
				foreach (SettingsItem node in this.nodes)
					hc = hc || node.HaveChanged;
				return (hc);
			}
		}

		/// <summary></summary>
		public virtual bool ExplicitHaveChanged
		{
			get
			{
				if (this.settingsType == SettingsType.Implicit)
					return (false);

				bool hc = this.haveChanged;
				foreach (SettingsItem node in this.nodes)
					hc = hc || node.ExplicitHaveChanged;
				return (hc);
			}
		}

		/// <summary></summary>
		public virtual void SetChanged()
		{
			SuspendChangeEvent();

			foreach (SettingsItem node in this.nodes)
				node.SetChanged();

			this.haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			SuspendChangeEvent();

			foreach (SettingsItem node in this.nodes)
				node.ClearChanged();

			this.haveChanged = false;

			ResumeChangeEvent();
		}

		#endregion

		#region Defaults Methods
		//==========================================================================================
		// Defaults Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual void SetDefaults()
		{
			SuspendChangeEvent();

			foreach (SettingsItem node in this.nodes)
				node.SetDefaults();

			SetMyDefaults();
			SetNodeDefaults();

			ResumeChangeEvent();
		}

		/// <summary></summary>
		protected virtual void SetMyDefaults()
		{
			// Default implementation has nothing to do (yet).
		}

		/// <summary></summary>
		protected virtual void SetNodeDefaults()
		{
			// Default implementation has nothing to do (yet).
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SettingsItem));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SettingsItem other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			if (GetType() == other.GetType())
			{
				// Compare all nodes, settings values are compared by inheriting class.
				if (this.nodes.Count == other.nodes.Count)
				{
					for (int i = 0; i < this.nodes.Count; i++)
					{
						if (this.nodes[i] != other.nodes[i])
							return (false);
					}
					return (true);
				}
			}
			return (false);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (SettingsItem node in this.nodes)
				hashCode ^= node.GetHashCode();

			return (hashCode);
		}

		#endregion

		#region Node Events
		//------------------------------------------------------------------------------------------
		// Node Events
		//------------------------------------------------------------------------------------------
		private void node_Changed(object sender, SettingsEventArgs e)
		{
			OnChanged(new SettingsEventArgs(this, e));
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void OnChanged(SettingsEventArgs e)
		{
			if (this.changeEventSuspendedCount == 0)
				EventHelper.FireSync<SettingsEventArgs>(Changed, this, e);
		}

		#endregion

		#region Change Event Methods
		//------------------------------------------------------------------------------------------
		// Change Event Methods
		//------------------------------------------------------------------------------------------

		/// <summary>
		/// Temporarily suspends the change event for the settings and all nodes of the settings tree.
		/// </summary>
		public virtual void SuspendChangeEvent()
		{
			foreach (SettingsItem node in this.nodes)
				node.SuspendChangeEvent();

			this.changeEventSuspendedCount++;
		}

		/// <summary>
		/// Resumes change events.
		/// </summary>
		/// <remarks>
		/// Calling the ResumeChangeEvent method forces changed events if there are any pending events.
		/// </remarks>
		public virtual void ResumeChangeEvent()
		{
			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public virtual void ResumeChangeEvent(bool forcePendingChangeEvent)
		{
			this.changeEventSuspendedCount--;

			foreach (SettingsItem node in this.nodes)
				node.ResumeChangeEvent(forcePendingChangeEvent);

			if (forcePendingChangeEvent && this.haveChanged)
				OnChanged(new SettingsEventArgs(this));
		}

		/// <summary>
		/// Forces a change event on the settings and all nodes of the settings tree.
		/// The event is fired even if the settings have not changed.
		/// </summary>
		public virtual void ForceChangeEvent()
		{
			foreach (SettingsItem node in this.nodes)
				node.ForceChangeEvent();

			OnChanged(new SettingsEventArgs(this));
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SettingsItem lhs, SettingsItem rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that potiential <Derived>.Equals() is called.
			// Thus, ensure that object.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(SettingsItem lhs, SettingsItem rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public class EmptySettingsItem : SettingsItem
	{
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
