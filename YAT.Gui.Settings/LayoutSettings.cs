using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace YAT.Gui.Settings
{
	[Serializable]
	public class LayoutSettings : Utilities.Settings.Settings, IEquatable<LayoutSettings>
	{
		private bool _txMonitorPanelIsVisible;
		private bool _bidirMonitorPanelIsVisible;
		private bool _rxMonitorPanelIsVisible;
		private Orientation _monitorOrientation;
		private float _txMonitorSplitterRatio;
		private float _rxMonitorSplitterRatio;

		private bool _predefinedPanelIsVisible;
		private float _predefinedSplitterRatio;

		private bool _sendCommandPanelIsVisible;
		private bool _sendFilePanelIsVisible;

		public LayoutSettings()
		{
			SetMyDefaults();
			ClearChanged();
		}

		public LayoutSettings(Utilities.Settings.SettingsType settingsType)
			: base(settingsType)
		{
			SetMyDefaults();
			ClearChanged();
		}

		/// <remarks>
		/// Directly set value-type fields to improve performance, changed flag will be cleared anyway.
		/// </remarks>
		public LayoutSettings(LayoutSettings rhs)
			: base(rhs)
		{
			_txMonitorPanelIsVisible    = rhs.TxMonitorPanelIsVisible;
			_bidirMonitorPanelIsVisible = rhs.BidirMonitorPanelIsVisible;
			_rxMonitorPanelIsVisible    = rhs.RxMonitorPanelIsVisible;
			_monitorOrientation         = rhs.MonitorOrientation;
			_txMonitorSplitterRatio     = rhs.TxMonitorSplitterRatio;
			_rxMonitorSplitterRatio     = rhs.RxMonitorSplitterRatio;

			_predefinedPanelIsVisible = rhs.PredefinedPanelIsVisible;
			_predefinedSplitterRatio  = rhs.PredefinedSplitterRatio;

			_sendCommandPanelIsVisible = rhs.SendCommandPanelIsVisible;
			_sendFilePanelIsVisible = rhs.SendFilePanelIsVisible;

			ClearChanged();
		}

		/// <remarks>
		/// Set fields through properties to ensure correct setting of changed flag.
		/// </remarks>
		protected override void SetMyDefaults()
		{
			TxMonitorPanelIsVisible = false;
			BidirMonitorPanelIsVisible = true;
			RxMonitorPanelIsVisible = false;
			MonitorOrientation = Orientation.Vertical;
			TxMonitorSplitterRatio = (float)1 / 3;
			RxMonitorSplitterRatio = (float)1 / 2;

			PredefinedPanelIsVisible = true;
			PredefinedSplitterRatio = (float)3 / 4;

			SendCommandPanelIsVisible = true;
			SendFilePanelIsVisible = true;
		}

		#region Properties
		//------------------------------------------------------------------------------------------
		// Properties
		//------------------------------------------------------------------------------------------

		[XmlElement("TxMonitorPanelIsVisible")]
		public bool TxMonitorPanelIsVisible
		{
			get { return (_txMonitorPanelIsVisible); }
			set
			{
				if (_txMonitorPanelIsVisible != value)
				{
					_txMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("BidirMonitorPanelIsVisible")]
		public bool BidirMonitorPanelIsVisible
		{
			get { return (_bidirMonitorPanelIsVisible); }
			set
			{
				if (_bidirMonitorPanelIsVisible != value)
				{
					_bidirMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("RxMonitorPanelIsVisible")]
		public bool RxMonitorPanelIsVisible
		{
			get { return (_rxMonitorPanelIsVisible); }
			set
			{
				if (_rxMonitorPanelIsVisible != value)
				{
					_rxMonitorPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("MonitorOrientation")]
		public Orientation MonitorOrientation
		{
			get { return (_monitorOrientation); }
			set
			{
				if (_monitorOrientation != value)
				{
					_monitorOrientation = value;
					SetChanged();
				}
			}
		}

		[XmlElement("TxMonitorSplitterRatio")]
		public float TxMonitorSplitterRatio
		{
			get { return (_txMonitorSplitterRatio); }
			set
			{
				if (_txMonitorSplitterRatio != value)
				{
					_txMonitorSplitterRatio = value;
					SetChanged();
				}
			}
		}

		[XmlElement("RxMonitorSplitterRatio")]
		public float RxMonitorSplitterRatio
		{
			get { return (_rxMonitorSplitterRatio); }
			set
			{
				if (_rxMonitorSplitterRatio != value)
				{
					_rxMonitorSplitterRatio = value;
					SetChanged();
				}
			}
		}

		[XmlElement("PredefinedPanelIsVisible")]
		public bool PredefinedPanelIsVisible
		{
			get { return (_predefinedPanelIsVisible); }
			set
			{
				if (_predefinedPanelIsVisible != value)
				{
					_predefinedPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("PredefinedSplitterRatio")]
		public float PredefinedSplitterRatio
		{
			get { return (_predefinedSplitterRatio); }
			set
			{
				if (_predefinedSplitterRatio != value)
				{
					_predefinedSplitterRatio = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendCommandPanelIsVisible")]
		public bool SendCommandPanelIsVisible
		{
			get { return (_sendCommandPanelIsVisible); }
			set
			{
				if (_sendCommandPanelIsVisible != value)
				{
					_sendCommandPanelIsVisible = value;
					SetChanged();
				}
			}
		}

		[XmlElement("SendFilePanelIsVisible")]
		public bool SendFilePanelIsVisible
		{
			get { return (_sendFilePanelIsVisible); }
			set
			{
				if (_sendFilePanelIsVisible != value)
				{
					_sendFilePanelIsVisible = value;
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
			if (obj is LayoutSettings)
				return (Equals((LayoutSettings)obj));

			return (false);
		}

		/// <summary>
		/// Determines whether this instance and the specified object have value equality.
		/// </summary>
		public bool Equals(LayoutSettings value)
		{
			// ensure that object.operator!=() is called
			if ((object)value != null)
			{
				return
					(
					_txMonitorPanelIsVisible.Equals(value._txMonitorPanelIsVisible) &&
					_bidirMonitorPanelIsVisible.Equals(value._bidirMonitorPanelIsVisible) &&
					_rxMonitorPanelIsVisible.Equals(value._rxMonitorPanelIsVisible) &&
					_monitorOrientation.Equals(value._monitorOrientation) &&
					_txMonitorSplitterRatio.Equals(value._txMonitorSplitterRatio) &&
					_rxMonitorSplitterRatio.Equals(value._rxMonitorSplitterRatio) &&

					_predefinedPanelIsVisible.Equals(value._predefinedPanelIsVisible) &&
					_predefinedSplitterRatio.Equals(value._predefinedSplitterRatio) &&

					_sendCommandPanelIsVisible.Equals(value._sendCommandPanelIsVisible) &&
					_sendFilePanelIsVisible.Equals(value._sendFilePanelIsVisible)
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
		/// Determines whether the two specified objects have reference or value equality.
		/// </summary>
		public static bool operator ==(LayoutSettings lhs, LayoutSettings rhs)
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
		public static bool operator !=(LayoutSettings lhs, LayoutSettings rhs)
		{
			return (!(lhs == rhs));
		}

		#endregion
	}
}
