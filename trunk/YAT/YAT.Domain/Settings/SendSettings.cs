﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 3 Development Version 1.99.53
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class SendSettings : MKY.Settings.SettingsItem, IEquatable<SendSettings>
	{
		/// <summary></summary>
		public const int LineRepeatInfinite = -1;

		/// <summary></summary>
		public const bool UseExplicitDefaultRadixDefault = false;

		/// <summary></summary>
		public const bool KeepCommandDefault = true;

		/// <summary></summary>
		public const bool CopyPredefinedDefault = false;

		/// <summary></summary>
		public const bool SendImmediatelyDefault = false;

		/// <summary></summary>
		public const int DefaultDelayDefault = 100;

		/// <summary></summary>
		public const int DefaultLineDelayDefault = 100;

		/// <summary></summary>
		public const int DefaultLineIntervalDefault = 100;

		/// <summary></summary>
		public const int DefaultLineRepeatDefault = LineRepeatInfinite;

		/// <summary></summary>
		public const bool DisableEscapesDefault = false;

		/// <summary></summary>
		public const bool SignalXOnBeforeEachTransmissionDefault = false;

		/// <remarks>
		/// Must be implemented as property that creates a new object on each call to ensure that
		/// there aren't multiple clients referencing (and modifying) the same object.
		/// </remarks>
		public static PeriodicSetting SignalXOnPeriodicallyDefault
		{
			get { return (new PeriodicSetting(false, 1000)); }
		}

		private bool useExplicitDefaultRadix;
		private bool keepCommand;
		private bool copyPredefined;
		private bool sendImmediately;
		private int  defaultDelay;
		private int  defaultLineDelay;
		private int  defaultLineInterval;
		private int  defaultLineRepeat;
		private bool disableEscapes;

		// Serial port specific send settings. Located here (and not in 'SerialPortSettings) as they are endemic to YAT.
		private bool signalXOnBeforeEachTransmission;
		private PeriodicSetting signalXOnPeriodically;

		/// <summary></summary>
		public SendSettings()
			: this(MKY.Settings.SettingsType.Explicit)
		{
		}

		/// <summary></summary>
		public SendSettings(MKY.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties even though changed flag will be cleared anyway.
		/// There potentially is additional code that needs to be run within the property method.
		/// </remarks>
		public SendSettings(SendSettings rhs)
			: base(rhs)
		{
			UseExplicitDefaultRadix = rhs.UseExplicitDefaultRadix;
			KeepCommand             = rhs.KeepCommand;
			CopyPredefined          = rhs.CopyPredefined;
			SendImmediately         = rhs.SendImmediately;
			DefaultDelay            = rhs.DefaultDelay;
			DefaultLineDelay        = rhs.DefaultLineDelay;
			DefaultLineInterval     = rhs.DefaultLineInterval;
			DefaultLineRepeat       = rhs.DefaultLineRepeat;
			DisableEscapes          = rhs.DisableEscapes;

			SignalXOnBeforeEachTransmission = rhs.SignalXOnBeforeEachTransmission;
			SignalXOnPeriodically           = rhs.SignalXOnPeriodically;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			base.SetMyDefaults();

			UseExplicitDefaultRadix = UseExplicitDefaultRadixDefault;
			KeepCommand             = KeepCommandDefault;
			CopyPredefined          = CopyPredefinedDefault;
			SendImmediately         = SendImmediatelyDefault;
			DefaultDelay            = DefaultDelayDefault;
			DefaultLineDelay        = DefaultLineDelayDefault;
			DefaultLineInterval     = DefaultLineIntervalDefault;
			DefaultLineRepeat       = DefaultLineRepeatDefault;
			DisableEscapes          = DisableEscapesDefault;

			SignalXOnBeforeEachTransmission = SignalXOnBeforeEachTransmissionDefault;
			SignalXOnPeriodically           = SignalXOnPeriodicallyDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
		[XmlElement("UseExplicitDefaultRadix")]
		public virtual bool UseExplicitDefaultRadix
		{
			get { return (this.useExplicitDefaultRadix); }
			set
			{
				if (this.useExplicitDefaultRadix != value)
				{
					this.useExplicitDefaultRadix = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("KeepCommand")]
		public virtual bool KeepCommand
		{
			get { return (this.keepCommand); }
			set
			{
				if (this.keepCommand != value)
				{
					this.keepCommand = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("CopyPredefined")]
		public virtual bool CopyPredefined
		{
			get { return (this.copyPredefined); }
			set
			{
				if (this.copyPredefined != value)
				{
					this.copyPredefined = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SendImmediately")]
		public virtual bool SendImmediately
		{
			get { return (this.sendImmediately); }
			set
			{
				if (this.sendImmediately != value)
				{
					this.sendImmediately = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DefaultDelay")]
		public virtual int DefaultDelay
		{
			get { return (this.defaultDelay); }
			set
			{
				if (this.defaultDelay != value)
				{
					this.defaultDelay = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DefaultLineDelay")]
		public virtual int DefaultLineDelay
		{
			get { return (this.defaultLineDelay); }
			set
			{
				if (this.defaultLineDelay != value)
				{
					this.defaultLineDelay = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DefaultLineInterval")]
		public virtual int DefaultLineInterval
		{
			get { return (this.defaultLineInterval); }
			set
			{
				if (this.defaultLineInterval != value)
				{
					this.defaultLineInterval = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DefaultLineRepeat")]
		public virtual int DefaultLineRepeat
		{
			get { return (this.defaultLineRepeat); }
			set
			{
				if (this.defaultLineRepeat != value)
				{
					this.defaultLineRepeat = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DisableEscapes")]
		public virtual bool DisableEscapes
		{
			get { return (this.disableEscapes); }
			set
			{
				if (this.disableEscapes != value)
				{
					this.disableEscapes = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SignalXOnBeforeEachTransmission")]
		public virtual bool SignalXOnBeforeEachTransmission
		{
			get { return (this.signalXOnBeforeEachTransmission); }
			set
			{
				if (this.signalXOnBeforeEachTransmission != value)
				{
					this.signalXOnBeforeEachTransmission = value;
					SetMyChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("SignalXOnPeriodically")]
		public virtual PeriodicSetting SignalXOnPeriodically
		{
			get { return (this.signalXOnPeriodically); }
			set
			{
				if (this.signalXOnPeriodically != value)
				{
					this.signalXOnPeriodically = value;
					SetMyChanged();
				}
			}
		}

		#endregion

		#region Methods
		//==========================================================================================
		// Methods
		//==========================================================================================

		/// <summary></summary>
		public virtual Parser.Modes ToParseMode()
		{
			if (DisableEscapes)
				return (Parser.Modes.NoEscapes);
			else
				return (Parser.Modes.AllEscapes);
		}

		#endregion

		#region Object Members
		//==========================================================================================
		// Object Members
		//==========================================================================================

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode(); // Get hash code of all settings nodes.

				hashCode = (hashCode * 397) ^ UseExplicitDefaultRadix        .GetHashCode();
				hashCode = (hashCode * 397) ^ KeepCommand                    .GetHashCode();
				hashCode = (hashCode * 397) ^ CopyPredefined                 .GetHashCode();
				hashCode = (hashCode * 397) ^ SendImmediately                .GetHashCode();
				hashCode = (hashCode * 397) ^ DefaultDelay;
				hashCode = (hashCode * 397) ^ DefaultLineDelay;
				hashCode = (hashCode * 397) ^ DefaultLineInterval;
				hashCode = (hashCode * 397) ^ DefaultLineRepeat;
				hashCode = (hashCode * 397) ^ DisableEscapes                 .GetHashCode();

				hashCode = (hashCode * 397) ^ SignalXOnBeforeEachTransmission.GetHashCode();
				hashCode = (hashCode * 397) ^ SignalXOnPeriodically          .GetHashCode();

				return (hashCode);
			}
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (Equals(obj as SendSettings));
		}

		/// <summary>
		/// Determines whether this instance and the specified object have reference or value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public bool Equals(SendSettings other)
		{
			if (ReferenceEquals(other, null)) return (false);
			if (ReferenceEquals(this, other)) return (true);
			if (GetType() != other.GetType()) return (false);

			return
			(
				base.Equals(other) && // Compare all settings nodes.

				UseExplicitDefaultRadix.Equals(other.UseExplicitDefaultRadix) &&
				KeepCommand            .Equals(other.KeepCommand)             &&
				CopyPredefined         .Equals(other.CopyPredefined)          &&
				SendImmediately        .Equals(other.SendImmediately)         &&
				DefaultDelay           .Equals(other.DefaultDelay)            &&
				DefaultLineDelay       .Equals(other.DefaultLineDelay)        &&
				DefaultLineInterval    .Equals(other.DefaultLineInterval)     &&
				DefaultLineRepeat      .Equals(other.DefaultLineRepeat)       &&
				DisableEscapes         .Equals(other.DisableEscapes)          &&

				SignalXOnBeforeEachTransmission.Equals(other.SignalXOnBeforeEachTransmission) &&
				SignalXOnPeriodically          .Equals(other.SignalXOnPeriodically)
			);
		}

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(SendSettings lhs, SendSettings rhs)
		{
			if (ReferenceEquals(lhs, rhs))  return (true);
			if (ReferenceEquals(lhs, null)) return (false);
			if (ReferenceEquals(rhs, null)) return (false);

			object obj = (object)lhs; // Operators are not virtual! Calling object.Equals() ensures
			return (obj.Equals(rhs)); // that a potential virtual <Derived>.Equals() is called.
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

//==================================================================================================
// End of
// $URL$
//==================================================================================================
