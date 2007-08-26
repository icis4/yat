using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace MKY.YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryDisplaySettings : Utilities.Settings.Settings, IEquatable<BinaryDisplaySettings>
	{
		private BinaryLengthLineBreak _lengthLineBreak;
		private BinarySequenceLineBreak _sequenceLineBreak;
		private BinaryTimedLineBreak _timedLineBreak;

		/// <summary></summary>
		public BinaryDisplaySettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		public BinaryDisplaySettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public BinaryDisplaySettings(BinaryDisplaySettings rhs)
			: base(rhs)
		{
			_lengthLineBreak   = rhs.LengthLineBreak;
			_sequenceLineBreak = rhs.SequenceLineBreak;
			_timedLineBreak    = rhs.TimedLineBreak;
			ClearChanged();
		}

		/// <summary></summary>
		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			LengthLineBreak   = new BinaryLengthLineBreak(false, 16);
			SequenceLineBreak = new BinarySequenceLineBreak(false, "\\h(00)");
			TimedLineBreak    = new BinaryTimedLineBreak(false, 500);
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		[XmlElement("LengthLineBreak")]
		public BinaryLengthLineBreak LengthLineBreak
		{
			get { return (_lengthLineBreak); }
			set
			{
				if (_lengthLineBreak != value)
				{
					_lengthLineBreak = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SequenceLineBreak")]
		public BinarySequenceLineBreak SequenceLineBreak
		{
			get { return (_sequenceLineBreak); }
			set
			{
				if (_sequenceLineBreak != value)
				{
					_sequenceLineBreak = value;
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("TimedLineBreak")]
		public BinaryTimedLineBreak TimedLineBreak
		{
			get { return (_timedLineBreak); }
			set
			{
				if (_timedLineBreak != value)
				{
					_timedLineBreak = value;
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
			if (obj is BinaryDisplaySettings)
				return (Equals((BinaryDisplaySettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(BinaryDisplaySettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_lengthLineBreak.Equals(value._lengthLineBreak) &&
					_sequenceLineBreak.Equals(value._sequenceLineBreak) &&
					_timedLineBreak.Equals(value._timedLineBreak)
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
		/// Determines whether the two specified objects have reference and value equality.
		/// </summary>
		public static bool operator ==(BinaryDisplaySettings lhs, BinaryDisplaySettings rhs)
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
		public static bool operator !=(BinaryDisplaySettings lhs, BinaryDisplaySettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
