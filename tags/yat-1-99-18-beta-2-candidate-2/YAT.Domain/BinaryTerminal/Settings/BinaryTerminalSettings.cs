using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace YAT.Domain.Settings
{
	/// <summary></summary>
	public class BinaryTerminalSettings : MKY.Utilities.Settings.Settings, IEquatable<BinaryTerminalSettings>
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		private bool _separateTxRxDisplay;
		private BinaryDisplaySettings _txDisplay;
		private BinaryDisplaySettings _rxDisplay;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		/// <summary></summary>
		public BinaryTerminalSettings()
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		/// <summary></summary>
		public BinaryTerminalSettings(MKY.Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			InitializeNodes();
			ClearChanged();
		}

		private void InitializeNodes()
		{
			TxDisplay = new BinaryDisplaySettings(SettingsType);
			RxDisplay = new BinaryDisplaySettings(SettingsType);
		}

		/// <summary></summary>
		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public BinaryTerminalSettings(BinaryTerminalSettings rhs)
			: base(rhs)
		{
			_separateTxRxDisplay = rhs.SeparateTxRxDisplay;
			TxDisplay = new BinaryDisplaySettings(rhs.TxDisplay);
			RxDisplay = new BinaryDisplaySettings(rhs.RxDisplay);
			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			SeparateTxRxDisplay = false;
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary></summary>
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

		/// <summary></summary>
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

		/// <summary></summary>
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
		//==========================================================================================
		// Object Members
		//==========================================================================================

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
					_separateTxRxDisplay.Equals(value._separateTxRxDisplay) &&
					base.Equals((MKY.Utilities.Settings.Settings)value) // compares all settings nodes
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
		//==========================================================================================
		// Comparison Operators
		//==========================================================================================

		/// <summary>
		/// Determines whether the two specified objects have reference or value equality.
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
