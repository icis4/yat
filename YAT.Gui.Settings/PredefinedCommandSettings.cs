using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Gui.Settings
{
	[Serializable]
	public class PredefinedCommandSettings : Utilities.Settings.Settings, IEquatable<PredefinedCommandSettings>
	{
		public const int MaximumCommandsPerPage = 12;

		private List<PredefinedCommandPage> _pages;

		public PredefinedCommandSettings()
			: base()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public PredefinedCommandSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public PredefinedCommandSettings(PredefinedCommandSettings rhs)
			: base(rhs)
		{
			Pages = new List<PredefinedCommandPage>(rhs.Pages);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			Pages = new List<PredefinedCommandPage>();
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("Pages")]
		public List<PredefinedCommandPage> Pages
		{
			get { return (_pages); }
			set
			{
				if (_pages != value)
				{
					_pages = value;
					SetChanged();
				}
			}
		}

		#endregion

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		public void CreateDefaultPage()
		{
			_pages = new List<PredefinedCommandPage>();
			_pages.Add(new PredefinedCommandPage("Page 1"));
			SetChanged();
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is PredefinedCommandSettings)
				return (Equals((PredefinedCommandSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(PredefinedCommandSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
				return (_pages.Equals(value._pages));

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
		public static bool operator ==(PredefinedCommandSettings lhs, PredefinedCommandSettings rhs)
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
		public static bool operator !=(PredefinedCommandSettings lhs, PredefinedCommandSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
