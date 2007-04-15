using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings
{
	public class BinaryDisplaySettings : Utilities.Settings.Settings
	{
		private BinaryLengthLineBreak _lengthLineBreak;
		private BinarySequenceLineBreak _sequenceLineBreak;
		private BinaryTimedLineBreak _timedLineBreak;

		public BinaryDisplaySettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public BinaryDisplaySettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		public BinaryDisplaySettings(BinaryDisplaySettings rhs)
			: base(rhs)
		{
			LengthLineBreak   = rhs.LengthLineBreak;
			SequenceLineBreak = rhs.SequenceLineBreak;
			TimedLineBreak    = rhs.TimedLineBreak;
			ClearChanged();
		}

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
