using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class PredefinedSettings : Utilities.Settings.Settings, IEquatable<PredefinedSettings>
	{
		private int _selectedPage;

		public PredefinedSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public PredefinedSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public PredefinedSettings(PredefinedSettings rhs)
			: base(rhs)
		{
			_selectedPage = rhs.SelectedPage;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			SelectedPage = 1;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("SelectedPage")]
		public int SelectedPage
		{
			get { return (_selectedPage); }
			set
			{
				if (_selectedPage != value)
				{
					_selectedPage = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is PredefinedSettings)
				return (Equals((PredefinedSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PredefinedSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
				return (_selectedPage.Equals(value._selectedPage));

			return (false);
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(PredefinedSettings lhs, PredefinedSettings rhs)
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
		public static bool operator !=(PredefinedSettings lhs, PredefinedSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
