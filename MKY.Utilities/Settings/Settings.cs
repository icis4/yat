//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2010 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;

using MKY.Utilities.Event;

namespace MKY.Utilities.Settings
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
	/// <remarks>
	/// IEquatable is not implemented because this abstract class cannot check
	/// for value equality.
	/// </remarks>
	public abstract class Settings
	{
		private SettingsType _settingsType = SettingsType.Explicit;

		private List<Settings> _nodes;
		private bool _haveChanged = false;
		private int _changeEventSuspendedCount = 0;

		/// <summary></summary>
		public event EventHandler<SettingsEventArgs> Changed;

		/// <summary></summary>
		public Settings()
		{
			_nodes = new List<Settings>();
		}

		/// <summary></summary>
		public Settings(SettingsType type)
		{
			_settingsType = type;
			_nodes = new List<Settings>();
		}

		/// <summary></summary>
		public Settings(Settings settings)
		{
			_settingsType = settings._settingsType;
			_nodes = new List<Settings>();         // do not copy nodes
		}

		/// <summary></summary>
		protected virtual void AttachNode(Settings node)
		{
			SuspendChangeEvent();

			node.SuspendChangeEvent();
			node.SetChanged();
			node.Changed += new EventHandler<SettingsEventArgs>(_node_Changed);
			_nodes.Add(node);

			ResumeChangeEvent();
		}

		/// <summary></summary>
		protected virtual void ReplaceNode(Settings nodeOld, Settings nodeNew)
		{
			SuspendChangeEvent();

			nodeOld.Changed -= new EventHandler<SettingsEventArgs>(_node_Changed);
			_nodes.Remove(nodeOld);

			nodeNew.SuspendChangeEvent();
			nodeNew.SetChanged();
			nodeNew.Changed += new EventHandler<SettingsEventArgs>(_node_Changed);
			_nodes.Add(nodeNew);

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual SettingsType SettingsType
		{
			get { return (_settingsType); }
		}

		/// <summary></summary>
		public virtual bool HaveChanged
		{
			get
			{
				bool hc = _haveChanged;
				foreach (Settings node in _nodes)
					hc = hc || node.HaveChanged;
				return (hc);
			}
		}

		/// <summary></summary>
		public virtual bool ExplicitHaveChanged
		{
			get
			{
				if (_settingsType == SettingsType.Implicit)
					return (false);

				bool hc = _haveChanged;
				foreach (Settings node in _nodes)
					hc = hc || node.ExplicitHaveChanged;
				return (hc);
			}
		}

		/// <summary></summary>
		public virtual void SetChanged()
		{
			SuspendChangeEvent();

			foreach (Settings node in _nodes)
				node.SetChanged();

			_haveChanged = true;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			SuspendChangeEvent();

			foreach (Settings node in _nodes)
				node.ClearChanged();

			_haveChanged = false;

			ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void SetDefaults()
		{
			SuspendChangeEvent();

			foreach (Settings node in _nodes)
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
		/// <remarks>
		/// IEquatable and "public bool Equals(Settings value)"  is not implemented
		/// because this abstract class cannot check for value equality.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (obj is Settings)
			{
				Settings value = (Settings)obj;

				// Ensure that object.operator!=() is called.
				if ((object)value != null)
				{
					if (this.GetType() == value.GetType())
					{
						// compare all nodes, settings values have already been compared by inheriting class
						if (_nodes.Count == value._nodes.Count)
						{
							for (int i = 0; i < _nodes.Count; i++)
							{
								if (_nodes[i] != value._nodes[i])
									return (false);
							}
							return (true);
						}
					}
				}
				return (false);
			}

			return (false);
		}

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Node Events
		//------------------------------------------------------------------------------------------
		// Node Events
		//------------------------------------------------------------------------------------------
		private void _node_Changed(object sender, SettingsEventArgs e)
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
			if (_changeEventSuspendedCount == 0)
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
			foreach (Settings node in _nodes)
				node.SuspendChangeEvent();

			_changeEventSuspendedCount++;
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
			_changeEventSuspendedCount--;

			foreach (Settings node in _nodes)
				node.ResumeChangeEvent(forcePendingChangeEvent);

			if (forcePendingChangeEvent && _haveChanged)
				OnChanged(new SettingsEventArgs(this));
		}

		/// <summary>
		/// Forces a change event on the settings and all nodes of the settings tree.
		/// The event is fired even if the settings have not changed.
		/// </summary>
		public virtual void ForceChangeEvent()
		{
			foreach (Settings node in _nodes)
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
			if (ReferenceEquals(lhs, rhs))
				return (true);

			if ((object)lhs != null)
				return (lhs.Equals(rhs));
			
			return (false);
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
