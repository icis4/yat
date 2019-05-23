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
// MKY Version 1.0.15
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MKY.Data
{
	/// <remarks>
	/// Attention, there currently are two similar implementations of this class:
	///  > <see cref="Data.DataItem"/>
	///  > <see cref="Settings.SettingsItem"/>
	/// The YAT feature request #3392253 "Consider replacing 'Settings' by 'DataItem'" deals with
	/// this issue, it will therefore not be forgotten. Until this feature request is implemented,
	/// changes to this class also have to be applied to <see cref="Settings.SettingsItem"/>.
	/// 
	/// Also note that this class intentionally doesn't implement <see cref="IDisposable"/>. That
	/// would unnecessarily complicate the handling of settings item, e.g. in a settings dialog,
	/// as code analysis requires that <see cref="IDisposable"/> are indeed disposed of.
	/// </remarks>
	public abstract class DataItem : IEquatable<DataItem>
	{
		private List<DataItem> nodes;          // = null;
		private bool haveChanged;              // = false;
		private int changeEventSuspendedCount; // = 0;
		private object changeEventSuspendedCountSyncObj = new object();

		/// <summary></summary>
		public event EventHandler<DataEventArgs> Changed;

		/// <summary></summary>
		protected DataItem()
		{
			this.nodes = new List<DataItem>();
		}

#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~DataItem()
		{
		////Diagnostics.DebugEventManagement.DebugNotifyAllEventRemains(this); Temporarily disabled until bug #344 has been resolved.
			Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

#endif // DEBUG

		#region Create/Destroy Properties and Methods
		//==========================================================================================
		// Create/Destroy Properties and Methods
		//==========================================================================================

		/// <summary></summary>
		protected virtual void AttachOrReplaceOrDetachNode(DataItem nodeOld, DataItem nodeNew)
		{
			if      (nodeNew == null)
			{
				DetachNode(nodeOld);
			}
			else if (nodeOld == null)
			{
				AttachNode(nodeNew);
			}
			else if (nodeNew != nodeOld)
			{
				ReplaceNode(nodeOld, nodeNew);
			}
			else // nodeNew == nodeOld
			{
				// Nothing to do.
			}
		}

		/// <summary></summary>
		protected virtual void AttachNode(DataItem node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.SetChangeEventSuspendedCount(this.changeEventSuspendedCount);
				node.SetChanged(); // Indicate new settings.
				node.Changed += node_Changed;
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

					nodeOld.Changed -= node_Changed;
					int index = this.nodes.IndexOf(nodeOld);
					this.nodes.RemoveAt(index);

					nodeNew.SetChangeEventSuspendedCount(this.changeEventSuspendedCount);
					nodeNew.SetChanged(); // Indicate potentially different settings.
					nodeNew.Changed += node_Changed;
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

				node.Changed -= node_Changed;
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
		/// This flag indicates that the item has changed. Either one of the values of the item
		/// itself, or any of the sub-items. This flag can be used to e.g. display an asterisk *
		/// indicating a change of data, settings,...
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

				if (this.nodes != null)
				{
					foreach (DataItem node in this.nodes)
						hc = hc || node.HaveChanged;
				}

				return (hc);
			}
		}

		/// <summary></summary>
		public virtual void SetChanged()
		{
			SuspendChangeEvent();

			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.SetChanged();
			}

			this.haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			SuspendChangeEvent();

			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.ClearChanged();
			}

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

			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.SetDefaults();
			}

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
		public virtual bool Equals(DataItem other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			if (GetType() == other.GetType())
			{
				if ((this.nodes != null) && (other.nodes != null))
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
			}

			return (false);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 0;

				if (this.nodes != null)
				{
					foreach (DataItem node in this.nodes)
						hashCode = (hashCode * 397) ^ node.GetHashCode();
				}

				return (hashCode);
			}
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

			lock (this.changeEventSuspendedCountSyncObj)
			{
				fire = (this.changeEventSuspendedCount == 0);
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
		/// Sets the change event suspended count.
		/// </summary>
		protected virtual void SetChangeEventSuspendedCount(int count)
		{
			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.SetChangeEventSuspendedCount(count);
			}

			lock (this.changeEventSuspendedCountSyncObj)
			{
				this.changeEventSuspendedCount = count;
			}
		}

		/// <summary>
		/// Temporarily suspends the change event for the settings and all nodes of the settings tree.
		/// </summary>
		public virtual void SuspendChangeEvent()
		{
			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.SuspendChangeEvent();
			}

			lock (this.changeEventSuspendedCountSyncObj)
			{
				this.changeEventSuspendedCount++;
			}
		}

		/// <summary>
		/// Resumes change events.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters result in cleaner code and clearly indicate the default behaviour.")]
		public virtual void ResumeChangeEvent(bool forcePendingChangeEvent = true)
		{
			lock (this.changeEventSuspendedCountSyncObj)
			{
				this.changeEventSuspendedCount--;
				if (this.changeEventSuspendedCount < 0)
					this.changeEventSuspendedCount = 0;
			}

			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.ResumeChangeEvent(forcePendingChangeEvent);
			}

			if (forcePendingChangeEvent && this.haveChanged)
				OnChanged(new DataEventArgs(this));
		}

		/// <summary>
		/// Forces a change event on the settings and all nodes of the settings tree.
		/// The event is fired even if the settings have not changed.
		/// </summary>
		public virtual void ForceChangeEvent()
		{
			if (this.nodes != null)
			{
				foreach (DataItem node in this.nodes)
					node.ForceChangeEvent();
			}

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
	public class EmptyDataItem : DataItem
	{
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================