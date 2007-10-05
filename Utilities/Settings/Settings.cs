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
	public abstract class Settings : IEquatable<Settings>
	{
		private SettingsType _settingsType = SettingsType.Explicit;

		private List<Settings> _nodes;
		private bool _haveChanged = false;
		private bool _changeEventIsSuspended = false;

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
		protected void AttachNode(Settings node)
		{
			SuspendChangeEvent();

			node.SuspendChangeEvent();
			node.SetChanged();
			node.Changed += new EventHandler<SettingsEventArgs>(_node_Changed);
			_nodes.Add(node);

			ResumeChangeEvent();
		}

		/// <summary></summary>
		protected void ReplaceNode(Settings nodeOld, Settings nodeNew)
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
		public SettingsType SettingsType
		{
			get { return (_settingsType); }
		}

		/// <summary></summary>
		public bool HaveChanged
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
		public bool ExplicitHaveChanged
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
			bool wasSuspended = _changeEventIsSuspended;
			if (!wasSuspended)
				SuspendChangeEvent();

			foreach (Settings node in _nodes)
				node.SetChanged();

			_haveChanged = true;

			if (!wasSuspended)
				ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void ClearChanged()
		{
			bool wasSuspended = _changeEventIsSuspended;
			if (!wasSuspended)
				SuspendChangeEvent();

			foreach (Settings node in _nodes)
				node.ClearChanged();

			_haveChanged = false;

			if (!wasSuspended)
				ResumeChangeEvent();
		}

		/// <summary></summary>
		public virtual void SetDefaults()
		{
			bool wasSuspended = _changeEventIsSuspended;
			if (!wasSuspended)
				SuspendChangeEvent();

			foreach (Settings node in _nodes)
				node.SetDefaults();

			SetMyDefaults();

			if (!wasSuspended)
				ResumeChangeEvent();
		}

		/// <summary></summary>
		protected virtual void SetMyDefaults()
		{
			// default implementation has nothing to do
		}

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is Settings)
				return (Equals((Settings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(Settings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				for (int i = 0; (i < _nodes.Count) && (i < value._nodes.Count); i++)
				{
					// ensure that overridden object.Equals() is called
					if (!_nodes[i].Equals((object)value._nodes[i]))
						return (false);
				}
				return (true);
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
			if (!_changeEventIsSuspended)
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
		public void SuspendChangeEvent()
		{
			foreach (Settings node in _nodes)
				node.SuspendChangeEvent();

			_changeEventIsSuspended = true;
		}

		/// <summary>
		/// Resumes change events.
		/// </summary>
		/// <remarks>
		/// Calling the ResumeChangeEvent method forces changed events if there are any pending events.
		/// </remarks>
		public void ResumeChangeEvent()
		{
			ResumeChangeEvent(true);
		}

		/// <summary></summary>
		public void ResumeChangeEvent(bool forcePendingChangeEvent)
		{
			_changeEventIsSuspended = false;

			foreach (Settings node in _nodes)
				node.ResumeChangeEvent(forcePendingChangeEvent);

			if (forcePendingChangeEvent && _haveChanged)
				OnChanged(new SettingsEventArgs(this));
		}

		/// <summary>
		/// Forces a change event on the settings and all nodes of the settings tree.
		/// The event is fired even if the settings have not changed.
		/// </summary>
		public void ForceChangeEvent()
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
