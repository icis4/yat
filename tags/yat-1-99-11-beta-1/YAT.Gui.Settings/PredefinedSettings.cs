using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Gui.Settings
{
	public class PredefinedSettings : Utilities.Settings.Settings
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

		public PredefinedSettings(PredefinedSettings rhs)
			: base(rhs)
		{
			SelectedPage = rhs.SelectedPage;
			ClearChanged();
		}

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