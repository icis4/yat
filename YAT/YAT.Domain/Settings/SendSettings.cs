//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	[Serializable]
	public class SendSettings : MKY.Settings.SettingsItem
	{
		/// <summary></summary>
		public const int LineRepeatInfinite = -1;

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
		public const int DefaultLineRepeatDefault = LineRepeatInfinite;

		/// <summary></summary>
		public const bool DisableKeywordsDefault = false;

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

		private bool keepCommand;
		private bool copyPredefined;
		private bool sendImmediately;
		private int  defaultDelay;
		private int  defaultLineDelay;
		private int  defaultLineRepeat;
		private bool disableKeywords;

		// Serial port specific send settings. Located here (and not in 'SerialPortSettings) as they are endemic to YAT.
		private bool signalXOnBeforeEachTransmission;
		private PeriodicSetting signalXOnPeriodically;

		/// <summary></summary>
		public SendSettings()
		{
			SetMyDefaults();
			ClearChanged();
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
			KeepCommand       = rhs.keepCommand;
			CopyPredefined    = rhs.copyPredefined;
			SendImmediately   = rhs.sendImmediately;
			DefaultDelay      = rhs.DefaultDelay;
			DefaultLineDelay  = rhs.DefaultLineDelay;
			DefaultLineRepeat = rhs.DefaultLineRepeat;
			DisableKeywords   = rhs.DisableKeywords;

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

			KeepCommand       = KeepCommandDefault;
			CopyPredefined    = CopyPredefinedDefault;
			SendImmediately   = SendImmediatelyDefault;
			DefaultDelay      = DefaultDelayDefault;
			DefaultLineDelay  = DefaultLineDelayDefault;
			DefaultLineRepeat = DefaultLineRepeatDefault;
			DisableKeywords   = DisableKeywordsDefault;

			SignalXOnBeforeEachTransmission = SignalXOnBeforeEachTransmissionDefault;
			SignalXOnPeriodically           = SignalXOnPeriodicallyDefault;
		}

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
					SetChanged();
				}
			}
		}

		/// <summary></summary>
		[XmlElement("DisableKeywords")]
		public virtual bool DisableKeywords
		{
			get { return (this.disableKeywords); }
			set
			{
				if (this.disableKeywords != value)
				{
					this.disableKeywords = value;
					SetChanged();
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
					SetChanged();
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
					SetChanged();
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
			if (DisableKeywords)
				return (Parser.Modes.AllExceptKeywords);
			else
				return (Parser.Modes.All);
		}

		#endregion

		#region Object Members

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to determine equality. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(obj, null))
				return (false);

			if (GetType() != obj.GetType())
				return (false);

			SendSettings other = (SendSettings)obj;
			return
			(
				base.Equals(other) && // Compare all settings nodes.

				(KeepCommand       == other.KeepCommand) &&
				(CopyPredefined    == other.CopyPredefined) &&
				(SendImmediately   == other.SendImmediately) &&
				(DefaultDelay      == other.DefaultDelay) &&
				(DefaultLineDelay  == other.DefaultLineDelay) &&
				(DefaultLineRepeat == other.DefaultLineRepeat) &&
				(DisableKeywords   == other.DisableKeywords) &&

				(SignalXOnBeforeEachTransmission == other.SignalXOnBeforeEachTransmission) &&
				(SignalXOnPeriodically           == other.SignalXOnPeriodically)
			);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <remarks>
		/// Use properties instead of fields to calculate hash code. This ensures that 'intelligent'
		/// properties, i.e. properties with some logic, are also properly handled.
		/// </remarks>
		public override int GetHashCode()
		{
			return
			(
				base.GetHashCode() ^ // Get hash code of all settings nodes.

				KeepCommand      .GetHashCode() ^
				CopyPredefined   .GetHashCode() ^
				SendImmediately  .GetHashCode() ^
				DefaultDelay     .GetHashCode() ^
				DefaultLineDelay .GetHashCode() ^
				DefaultLineRepeat.GetHashCode() ^
				DisableKeywords  .GetHashCode() ^

				SignalXOnBeforeEachTransmission.GetHashCode() ^
				SignalXOnPeriodically          .GetHashCode()
			);
		}

		#endregion

		#region Comparison Operators

		// Use of base reference type implementation of operators ==/!=.
		// See MKY.Test.EqualityTest for details.

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
