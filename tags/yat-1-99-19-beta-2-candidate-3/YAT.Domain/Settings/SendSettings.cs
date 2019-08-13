using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class SendSettings : MKY.Utilities.Settings.Settings, IEquatable<SendSettings>
	{
		/// <summary></summary>
		public const bool KeepCommandDefault = true;
		/// <summary></summary>
		public const bool CopyPredefinedDefault = false;

		private bool _keepCommand;
		private bool _copyPredefined;

		/// <summary></summary>
		public SendSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public SendSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public SendSettings(SendSettings rhs)
			: base(rhs)
		{
			_keepCommand = rhs._keepCommand;
			_copyPredefined = rhs._copyPredefined;
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			KeepCommand = KeepCommandDefault;
			CopyPredefined = CopyPredefinedDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("KeepCommand")]
		public bool KeepCommand
		{
			get { return (_keepCommand); }
			set
			{
				if (_keepCommand != value)
				{
					_keepCommand = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CopyPredefined")]
		public bool CopyPredefined
		{
			get { return (_copyPredefined); }
			set
			{
				if (_copyPredefined != value)
				{
					_copyPredefined = value;
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
			if (obj is SendSettings)
				return (Equals((SendSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(SendSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_keepCommand.Equals(value._keepCommand) &&
					_copyPredefined.Equals(value._copyPredefined)
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
		public static bool operator ==(SendSettings lhs, SendSettings rhs)
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
		public static bool operator !=(SendSettings lhs, SendSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}