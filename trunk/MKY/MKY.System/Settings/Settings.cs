//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright � 2003-2004 HSR Hochschule f�r Technik Rapperswil.
// Copyright � 2003-2010 Matthias Kl�y.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

using MKY.System.Event;

namespace MKY.System.Settings
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

	/// <summary></summary>
	public abstract class Settings : IEquatable<Settings>
	{
		private SettingsType settingsType = SettingsType.Explicit;

		private List<Settings> nodes;
		private bool haveChanged = false;
		private int changeEventSuspendedCount = 0;

		/// <summary></summary>
		public event EventHandler<SettingsEventArgs> Changed;

		/// <summary></summary>
		public Settings()
		{
			this.nodes = new List<Settings>();
		}

		/// <summary></summary>
		public Settings(SettingsType type)
		{
			this.settingsType = type;
			this.nodes = new List<Settings>();
		}

		/// <summary></summary>
		public Settings(Settings settings)
		{
			this.settingsType = settings.settingsType;
			this.nodes = new List<Settings>();         // do not copy nodes
		}

		/// <summary></summary>
		protected virtual void AttachNode(Settings node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.SuspendChangeEvent();
				node.SetChanged();
				node.Changed += new EventHandler<SettingsEventArgs>(this.node_Changed);
				this.nodes.Add(node);

				ResumeChangeEvent();
			}
		}

		/// <summary></summary>
		protected virtual void ReplaceNode(Settings nodeOld, Settings nodeNew)
		{
			if ((nodeOld != null) && (nodeNew != null))
			{
				if (this.nodes.Contains(nodeOld))
				{
					SuspendChangeEvent();

					nodeOld.Changed -= new EventHandler<SettingsEventArgs>(this.node_Changed);
					int index = this.nodes.IndexOf(nodeOld);
					this.nodes.RemoveAt(index);

					nodeNew.SuspendChangeEvent();
					nodeNew.SetChanged();
					nodeNew.Changed += new EventHandler<SettingsEventArgs>(this.node_Changed);
					this.nodes.Insert(index, nodeNew);

					ResumeChangeEvent();
				}
			}
		}

		/// <summary></summary>
		protected virtual void DetachNode(Settings node)
		{
			if (node != null)
			{
				SuspendChangeEvent();

				node.Changed -= new EventHandler<SettingsEventArgs>(this.node_Changed);
				this.nodes.Remove(node);

				ResumeChangeEvent();
			}
		}

		/// <summary></summary>
		public virtual SettingsType SettingsType
		{
			get { return (this.settingsType); }
		}

		/// <summary></summary>
		public virtual bool HaveChanged
		{
			get
			{
				bool hc = this.haveChanged;
				foreach (Settings node in this.nodes)
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
				foreach (Settings node in this.nodes)
					hc = hc || node.ExplicitHaveChanged;
				return (hc);
			}
		}

		/// <summary></summary>
		public virtual void SetChanged()
		{
			SuspendChangeEvent();

			foreach (Settings node in this.nodes)
				node.SetChanged();

			this.haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			SuspendChangeEvent();

			foreach (Settings node in this.nodes)
				node.ClearChanged();

			this.haveChanged = false;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void SetDefaults()
		{
			SuspendChangeEvent();

			foreach (Settings node in this.nodes)
				node.SetDefaults();

			SetMyDefaults();

			ResumeChangeEvent();
		}

		/// <summary></summary>
		protected virtual void SetMyDefaults()
		{
			// Default implementation has nothing to do (yet).
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as Settings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(Settings other)
		{
			if (ReferenceEquals(other, null))
				return (false);

			if (GetType() != other.GetType())
				return (false);

			if (this.GetType() == other.GetType())
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

		/// <summary></summary>
		public override int GetHashCode()
		{
			int hashCode = 0;
			foreach (Settings node in this.nodes)
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
			foreach (Settings node in this.nodes)
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

			foreach (Settings node in this.nodes)
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
			foreach (Settings node in this.nodes)
				node.ForceChangeEvent();

			OnChanged(new SettingsEventArgs(this));
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(Settings lhs, Settings rhs)
		{
			// Base reference type implementation of operator ==.
			// See MKY.System.Test.EqualityTest for details.

			if (ReferenceEquals(lhs, rhs)) return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			// Ensure that object.Equals() is called.
			// Thus, ensure that potential <Derived>.Equals() is called.
			object obj = (object)lhs;
			return (obj.Equals(rhs));
		}

		/// <summary>
		/// Determines whether the two specified objects have reference and value inequality.
		/// </summary>
		public static bool operator !=(Settings lhs, Settings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
