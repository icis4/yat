using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using MKY.Utilities.Recent;

namespace YAT.Model.Settings
{
	/// <summary></summary>
	[Serializable]
	public class RecentFileSettings : MKY.Utilities.Settings.Settings, IEquatable<RecentFileSettings>
	{
		/// <summary></summary>
		public const int MaximumFilePaths = 8;

		private RecentItemCollection<string> _filePaths;

		/// <summary></summary>
		public RecentFileSettings()
			: base()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public RecentFileSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public RecentFileSettings(RecentFileSettings rhs)
			: base(rhs)
		{
			FilePaths = new RecentItemCollection<string>(rhs.FilePaths);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			FilePaths = new RecentItemCollection<string>(MaximumFilePaths);
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
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
		/// generic collection <see cref="T:RecentItemCollection`1"/>.
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

		/// <summary></summary>
		public override int GetHashCode()
		{
			return (base.GetHashCode());
		}

		#endregion

		#region Comparison Operators

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
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
