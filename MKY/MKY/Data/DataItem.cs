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

using System;
using System.Collections.Generic;
using System.Threading;

namespace MKY.Data
{
	/// <remarks>
	/// Attention, there currently are two similar implementations of this class:
	///  > <see cref="Data.DataItem"/>
	///  > <see cref="Settings.SettingsItem"/>
	/// The YAT feature request #3392253 "Consider replacing 'Settings' by 'DataItem'" deals with
	/// this issue, it will therefore not be forgotten. Until this feature request is implemented,
	/// changes to this class also have to be applied to <see cref="Settings.SettingsItem"/>.
	/// </remarks>
	public abstract class DataItem : IEquatable<DataItem>
	{
		private List<DataItem> nodes;          // = null;
		private bool haveChanged;              // = false;
		private int changeEventSuspendedCount; // = 0;
		private ReaderWriterLockSlim changeEventSuspendedCountLock = new ReaderWriterLockSlim();

		/// <summary></summary>
		public event EventHandler<DataEventArgs> Changed;

		/// <summary></summary>
		protected DataItem()
		{
			this.nodes = new List<DataItem>();
		}

		#region Setup/Teardown Properties and Methods
		//==========================================================================================
		// Setup/Teardown Properties and Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void AttachNode(DataItem node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.SuspendChangeEvent();
				node.SetChanged();
				node.Changed += new EventHandler<DataEventArgs>(node_Changed);
				this.nodes.Add(node);

				ResumeChangeEvent();
			}
		}

		/// <summary></summary>
		protected virtual void ReplaceNode(DataItem nodeOld, DataItem nodeNew)
		{
			if ((nodeOld != null) && (nodeNew != null))
			{
				if (this.nodes.Contains(nodeOld))
				{
					SuspendChangeEvent();

					nodeOld.Changed -= new EventHandler<DataEventArgs>(node_Changed);
					int index = this.nodes.IndexOf(nodeOld);
					this.nodes.RemoveAt(index);

					nodeNew.SuspendChangeEvent(nodeOld.changeEventSuspendedCount);
					nodeNew.SetChanged();
					nodeNew.Changed += new EventHandler<DataEventArgs>(node_Changed);
					this.nodes.Insert(index, nodeNew);

					ResumeChangeEvent();
				}
			}
		}

		/// <summary></summary>
		protected virtual void DetachNode(DataItem node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.Changed -= new EventHandler<DataEventArgs>(node_Changed);
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
		/// or any of the sub-items. This flag can be used to e.g. display an asterisk * indicating
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
				foreach (DataItem node in this.nodes)
					hc = hc || node.HaveChanged;
				return (hc);
			}
		}

		/// <summary></summary>
		public virtual void SetChanged()
		{
			SuspendChangeEvent();

			foreach (DataItem node in this.nodes)
				node.SetChanged();

			this.haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			SuspendChangeEvent();

			foreach (DataItem node in this.nodes)
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

			foreach (DataItem node in this.nodes)
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
			return (Equals(obj as DataItem));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(DataItem other)
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
			foreach (DataItem node in this.nodes)
				hashCode ^= node.GetHashCode();

			return (hashCode);
		}

		#endregion

		#region Node Events
		//------------------------------------------------------------------------------------------
		// Node Events
		//------------------------------------------------------------------------------------------
		private void node_Changed(object sender, DataEventArgs e)
		{
			OnChanged(new DataEventArgs(this, e));
		}

		#endregion

		#region Event Invoking
		//------------------------------------------------------------------------------------------
		// Event Invoking
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void OnChanged(DataEventArgs e)
		{
			bool fire = false;

			this.changeEventSuspendedCountLock.EnterReadLock();
			try
			{
				fire = (this.changeEventSuspendedCount == 0);
			}
			finally
			{
				this.changeEventSuspendedCountLock.ExitReadLock();
			}

			if (fire)
				EventHelper.FireSync<DataEventArgs>(Changed, this, e);
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
			SuspendChangeEvent(1);
		}

		/// <summary>
		/// Temporarily suspends the change event for the settings and all nodes of the settings tree.
		/// </summary>
		protected virtual void SuspendChangeEvent(int suspendedCount)
		{
			foreach (DataItem node in this.nodes)
				node.SuspendChangeEvent(suspendedCount);

			this.changeEventSuspendedCountLock.EnterWriteLock();
			try
			{
				this.changeEventSuspendedCount += suspendedCount;
			}
			finally
			{
				this.changeEventSuspendedCountLock.ExitWriteLock();
			}
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
			this.changeEventSuspendedCountLock.EnterWriteLock();
			try
			{
				this.changeEventSuspendedCount--;
				if (this.changeEventSuspendedCount < 0)
					this.changeEventSuspendedCount = 0;
			}
			finally
			{
				this.changeEventSuspendedCountLock.ExitWriteLock();
			}

			foreach (DataItem node in this.nodes)
				node.ResumeChangeEvent(forcePendingChangeEvent);

			if (forcePendingChangeEvent && this.haveChanged)
				OnChanged(new DataEventArgs(this));
		}

		/// <summary>
		/// Forces a change event on the settings and all nodes of the settings tree.
		/// The event is fired even if the settings have not changed.
		/// </summary>
		public virtual void ForceChangeEvent()
		{
			foreach (DataItem node in this.nodes)
				node.ForceChangeEvent();

			OnChanged(new DataEventArgs(this));
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(DataItem lhs, DataItem rhs)
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
		public static bool operator !=(DataItem lhs, DataItem rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}

	/// <summary></summary>
	[Serializable]
	public class EmptyDataItem : DataItem
	{
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
