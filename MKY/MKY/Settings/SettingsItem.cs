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
using System.Diagnostics.CodeAnalysis;

namespace MKY.Settings
{
	/// <summary></summary>
	public enum SettingsType
	{
		/// <summary>
		/// Explicit (normal) user settings, user gets notified as soon as setting changes.
		/// </summary>
		Explicit,

		/// <summary>
		/// Implicit (hidden) user settings, user doesn't get notified when setting changes.
		/// </summary>
		Implicit
	}

	/// <remarks>
	/// Attention, there currently are two similar implementations of this class:
	///  > <see cref="Settings.SettingsItem"/>
	///  > <see cref="Data.DataItem"/>
	/// The YAT feature request #158 "Consider replacing 'Settings' by 'DataItem'" deals with
	/// this issue, it will therefore not be forgotten. Until this feature request is implemented,
	/// changes to this class also have to be applied to <see cref="Data.DataItem"/>.
	///
	/// Note the additional YAT feature request #159 "Consider replacing SettingsItem/​DataItem
	/// with attributes" and feature request #286 "Consider upgrade from XML serialization to
	/// .NET 3.0 DataContract serialization" to switch to that serialization, the preferred way
	/// to handle persistence since the .NET 3.0 framework version.
	///
	/// Also note that this class intentionally doesn't implement <see cref="IDisposable"/>. That
	/// would unnecessarily complicate the handling of settings item, e.g. in a settings dialog,
	/// as code analysis requires that <see cref="IDisposable"/> are indeed disposed of.
	///
	/// Finally note that the 'Serializable' attribute is required to allow reflection using e.g.
	/// <see cref="System.Runtime.Serialization.FormatterServices.GetSerializableMembers(Type)"/>.
	/// </remarks>
	[Serializable]
	public abstract class SettingsItem : IEquatable<SettingsItem>
	{
		private SettingsType settingsType;

		private List<SettingsItem> nodes;      // = null;
		private bool haveChanged;              // = false;
		private int changeEventSuspendedCount; // = 0;
		private object changeEventSuspendedCountSyncObj = new object();

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

	#if (DEBUG)

		/// <remarks>
		/// Note that it is not possible to mark a finalizer with [Conditional("DEBUG")].
		/// </remarks>
		[SuppressMessage("Microsoft.Performance", "CA1821:RemoveEmptyFinalizers", Justification = "See remarks.")]
		~SettingsItem()
		{
			// Do not Diagnostics.DebugEventManagement.DebugNotifyAllEventRemains(this) as XML deserialization doesn't Free() items (didn't Dispose() either...).
			Diagnostics.DebugFinalization.DebugNotifyFinalizerAndCheckWhetherOverdue(this);
		}

	#endif // DEBUG

		#region Create/Destroy Properties and Methods
		//==========================================================================================
		// Create/Destroy Properties and Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual SettingsType SettingsType
		{
			get { return (this.settingsType); }
		}

		/// <summary></summary>
		protected virtual void AttachOrReplaceOrDetachNode(SettingsItem nodeOld, SettingsItem nodeNew)
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
		protected virtual void AttachNode(SettingsItem node)
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
		protected virtual void ReplaceNode(SettingsItem nodeOld, SettingsItem nodeNew)
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
		protected virtual void DetachNode(SettingsItem node)
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
		/// itself, or any of the sub-items. Either explicit or implicit settings.
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
					foreach (SettingsItem node in this.nodes)
						hc = hc || node.HaveChanged;
				}

				return (hc);
			}
		}

		/// <summary>
		/// This flag indicates that the item has changed. Either one of the values of the item
		/// itself, or any of the sub-items. This flag can be used to e.g. display an asterisk *
		/// indicating a change of data, settings,...
		/// </summary>
		/// <remarks>
		/// To clear this flag, <see cref="ClearChanged"/> must be called. The flag is never cleared
		/// automatically.
		/// </remarks>
		public virtual bool ExplicitHaveChanged
		{
			get
			{
				if (this.settingsType == SettingsType.Implicit)
					return (false);

				bool hc = this.haveChanged;

				if (this.nodes != null)
				{
					foreach (SettingsItem node in this.nodes)
						hc = hc || node.ExplicitHaveChanged;
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
				foreach (SettingsItem node in this.nodes)
					node.SetChanged();
			}

			this.haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		protected virtual void SetMyChanged()
		{
			SuspendChangeEvent();

			this.haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			SuspendChangeEvent();

			if (this.nodes != null)
			{
				foreach (SettingsItem node in this.nodes)
					node.ClearChanged();
			}

			this.haveChanged = false;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		protected virtual void ClearMyChanged()
		{
			SuspendChangeEvent();

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
				foreach (SettingsItem node in this.nodes)
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
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
					foreach (SettingsItem node in this.nodes)
						hashCode = (hashCode * 397) ^ node.GetHashCode();
				}

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SettingsItem));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public bool Equals(SettingsItem other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);

			if (GetType() == other.GetType())
			{
				if ((this.nodes != null) && (other.nodes != null))
				{
					// Compare all nodes, settings values are compared by inheriting class:
					if (this.nodes.Count == other.nodes.Count)
					{
						for (int i = 0; i < this.nodes.Count; i++)
						{
							if (!ObjectEx.Equals(this.nodes[i], other.nodes[i]))
								return (false);
						}

						return (true);
					}
				}
			}

			return (false);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SettingsItem lhs, SettingsItem rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(SettingsItem lhs, SettingsItem rhs)
		{
			return (!(lhs == rhs));
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

		#region Event Raising
		//------------------------------------------------------------------------------------------
		// Event Raising
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void OnChanged(SettingsEventArgs e)
		{
			bool doRaise = false;

			lock (this.changeEventSuspendedCountSyncObj)
				doRaise = (this.changeEventSuspendedCount == 0);

			if (doRaise)
				EventHelper.RaiseSync<SettingsEventArgs>(Changed, this, e);
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
				foreach (SettingsItem node in this.nodes)
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
				foreach (SettingsItem node in this.nodes)
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
		[SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameters may result in cleaner code and clearly indicate the default behavior.")]
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
				foreach (SettingsItem node in this.nodes)
					node.ResumeChangeEvent(forcePendingChangeEvent);
			}

			if (forcePendingChangeEvent && this.haveChanged)
				OnChanged(new SettingsEventArgs(this));
		}

		/// <summary>
		/// Forces a change event on the settings and all nodes of the settings tree.
		/// The event is raised even if the settings have not changed.
		/// </summary>
		public virtual void ForceChangeEvent()
		{
			if (this.nodes != null)
			{
				foreach (SettingsItem node in this.nodes)
					node.ForceChangeEvent();
			}

			OnChanged(new SettingsEventArgs(this));
		}

		#endregion
	}

	/// <summary></summary>
	public class EmptySettingsItem : SettingsItem
	{
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
