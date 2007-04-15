using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace HSR.YAT.Domain.Settings
{
	public class BinaryTerminalSettings : Utilities.Settings.Settings
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		private bool _directionLineBreakEnabled;

		private bool _separateTxRxDisplay;
		private BinaryDisplaySettings _txDisplay;
		private BinaryDisplaySettings _rxDisplay;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		public BinaryTerminalSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public BinaryTerminalSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		public BinaryTerminalSettings(BinaryTerminalSettings rhs)
			: base(rhs)
		{
			DirectionLineBreakEnabled = rhs.DirectionLineBreakEnabled;
			SeparateTxRxDisplay = rhs.SeparateTxRxDisplay;
			TxDisplay = new BinaryDisplaySettings(rhs.TxDisplay);
			RxDisplay = new BinaryDisplaySettings(rhs.RxDisplay);
			ClearChanged();
		}

		private void InitializeNodes()
		{
			DirectionLineBreakEnabled = true;
			TxDisplay = new BinaryDisplaySettings(SettingsType);
			RxDisplay = new BinaryDisplaySettings(SettingsType);
		}

		protected override void SetMyDefaults()
		{
			SeparateTxRxDisplay = false;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("DirectionLineBreakEnabled")]
		public bool DirectionLineBreakEnabled
		{
			get { return (_directionLineBreakEnabled); }
			set
			{
				if (_directionLineBreakEnabled != value)
				{
					_directionLineBreakEnabled = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SeparateTxRxDisplay")]
		public bool SeparateTxRxDisplay
		{
			get { return (_separateTxRxDisplay); }
			set
			{
				if (_separateTxRxDisplay != value)
				{
					_separateTxRxDisplay = value;
					SetChanged();
				}
			}
		}

		[XmlElement("TxDisplay")]
		public BinaryDisplaySettings TxDisplay
		{
			get { return (_txDisplay); }
			set
			{
				if (_txDisplay == null)
				{
					_txDisplay = value;
					AttachNode(_txDisplay);
				}
				else if (_txDisplay != value)
				{
					BinaryDisplaySettings old = _txDisplay;
					_txDisplay = value;
					ReplaceNode(old, _txDisplay);
				}
			}
		}

		[XmlElement("RxDisplay")]
		public BinaryDisplaySettings RxDisplay
		{
			get
			{
				if (_separateTxRxDisplay)
					return (_rxDisplay);
				else
					return (_txDisplay);
			}
			set
			{
				if (_rxDisplay == null)
				{
					_rxDisplay = value;
					AttachNode(_rxDisplay);
				}
				else if (_rxDisplay != value)
				{
					BinaryDisplaySettings old = _rxDisplay;
					_rxDisplay = value;
					ReplaceNode(old, _rxDisplay);
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
			if (obj is BinaryTerminalSettings)
				return (Equals((BinaryTerminalSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(BinaryTerminalSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_directionLineBreakEnabled.Equals(value._directionLineBreakEnabled) &&
					_separateTxRxDisplay.Equals(value._separateTxRxDisplay) &&
					base.Equals((Utilities.Settings.Settings)value) // compares all settings nodes
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
		public static bool operator ==(BinaryTerminalSettings lhs, BinaryTerminalSettings rhs)
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
		public static bool operator !=(BinaryTerminalSettings lhs, BinaryTerminalSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
