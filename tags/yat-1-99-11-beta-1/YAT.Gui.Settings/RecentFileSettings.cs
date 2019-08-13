using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using HSR.Utilities.Recent;

namespace HSR.YAT.Gui.Settings
{
	[Serializable]
	public class RecentFileSettings : Utilities.Settings.Settings
	{
		public const int MaximumFilePaths = 8;

		private RecentItemCollection<string> _filePaths;

		public RecentFileSettings()
			: base()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public RecentFileSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public RecentFileSettings(RecentFileSettings rhs)
			: base(rhs)
		{
			FilePaths = new RecentItemCollection<string>(rhs.FilePaths);
			ClearChanged();
		}

		protected override void SetMyDefaults()
		{
			FilePaths = new RecentItemCollection<string>(MaximumFilePaths);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("FilePaths")]
		public RecentItemCollection<string> FilePaths
		{
			get { return (_filePaths); }
			set
			{
				if (_filePaths != value)
				{
					_filePaths = value;
					SetChanged();
				}
			}
		}

		/// <remarks>
		/// This property allows standard XML serialization which is not provided for
		/// generic collection <see cref="RecentItemCollection"/>.
		/// </remarks>
		[XmlElement("FilePathsMaximumCapacity")]
		public int FilePathsMaximumCapacity
		{
			get { return (_filePaths.MaximumCapacity); }
			set { _filePaths.MaximumCapacity = value; }
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			if (obj is RecentFileSettings)
				return (Equals((RecentFileSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(RecentFileSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_filePaths.Equals(value._filePaths)
					);
			}
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
		public static bool operator ==(RecentFileSettings lhs, RecentFileSettings rhs)
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
		public static bool operator !=(RecentFileSettings lhs, RecentFileSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}